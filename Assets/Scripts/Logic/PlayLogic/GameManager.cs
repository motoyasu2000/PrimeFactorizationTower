using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Common;

/// <summary>
/// ゲームを管理するクラス。Originの生成や表示、生成されたブロックが条件を満たすかの判定、ゲームオーバーの判定、ターンの管理などなど。
/// </summary>
public class GameManager : MonoBehaviour
{
    //UI
    TextMeshProUGUI nowScoreText;
    TextMeshProUGUI maxScoreText;
    UpperUIManager upperUIManager;
    NowTurnTextManager nowTurnTextManager; //現在誰のターンであるのかを表すUI
    StandingStillTimerVisualizer standingStillTimerVisualizer; //ブロックが静止している間に表示するUIを管理するクラス

    //スコアの管理
    bool areAllBlocksGrounded = false; //全てのブロックが地面に設置しているか。スコア計算の条件やターンの切り替えの条件で使う
    bool wereAllBlocksGroundedLastFrame = false; //1フレーム前のareAllBlocksGrounded
    ScoreManager scoreManager;
    public bool AreAllBlocksGrounded => areAllBlocksGrounded;
    public bool WereAllBlocksGroundedLastFrame => wereAllBlocksGroundedLastFrame;

    //画面中央の合成数の管理や、素因数分解ができているかのチェック
    int prePrimeNumberCheckFieldCount = -1; //primeNumberCheckFieldの子オブジェクト数の変化に応じて処理を行うため
    OriginManager originManager;
    EarthQuakeManager earthQuakeManager; //素因数分解に失敗した際、ペナルティとして地震を発生させるクラス

    //ブロックの親オブジェクト候補
    GameObject blockField; //下二つの様なブロックの親オブジェクトをまとめる親オブジェクト
    GameObject primeNumberCheckField; //ブロックを落下させた瞬間、そのブロックは、このゲームオブジェクトの子要素となる
    GameObject completedField; //primeNumberCheckField内のブロックの積が画面上部の合成数と一致したら、それらのブロックはこのゲームオブジェクトの子要素になる

    //ターンの切り替え
    bool isDropBlockNowTurn = false;
    float afterDropTotalTimer = 0; //ブロックを落下させてから合計でどれだけ時間が経過したか
    float standingStillTimer = 0; //全てのゲームオブジェクトが連続で静止している時間
    static readonly float changeTurnTime = 1f; //全てのゲームオブジェクトがどれだけの時間静止すればターンが切り替わるのか
    static readonly float judgeStillStandingTime = 0.005f; //ゲームオブジェクトの速度がどのくらいなら静止しているとみなすか
    static readonly float additionalJudgeStillStandingScale = 0.005f; //ブロックドロップ後の経過時間に併せて、静止とみなす速度の閾値をどのくらい上昇させるか
    public bool IsDropBlockNowTurn => isDropBlockNowTurn;
    //全てのゲームオブジェクトの静止時間が基準を超えており、かつ、このターン内にブロックが生成されていればターンを切り替える
    bool chengesNextTurn => (standingStillTimer > changeTurnTime) && isDropBlockNowTurn;
    float AdditionalJudgeStillStandingTime => afterDropTotalTimer * additionalJudgeStillStandingScale;

    MaxHeightCalculator maxHeightCalculator; //高さの計算を行う

    //初期化処理
    private void Awake()
    {
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        nowScoreText = GameObject.Find("NowScoreText").GetComponent<TextMeshProUGUI>();
        maxScoreText = GameObject.Find("MaxScoreText").GetComponent<TextMeshProUGUI>();
        nowTurnTextManager = FindObjectOfType<NowTurnTextManager>();
        standingStillTimerVisualizer = GameObject.Find("StandingStillTimerVisualizer").GetComponent<StandingStillTimerVisualizer>();
        blockField = GameObject.Find("BlockField");
        primeNumberCheckField = blockField.transform.Find("PrimeNumberCheckField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
        scoreManager = ScoreManager.Ins;
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        earthQuakeManager = GameObject.Find("EarthQuakeManager").GetComponent<EarthQuakeManager>();
        maxHeightCalculator = GameObject.Find("MaxHeightCalculator").GetComponent<MaxHeightCalculator>();
        DisplayMaxScore();
    }

    void Update()
    {
        //全てのブロックが地面に設置しているかのチェック
        CheckAllBlocksOnGround();

        //PrimeNumberCheckField内部の合成数を計算。Originの条件を満たしていなければゲームオーバー
        CheckMatchingOrigin();

        //素数ブロックの積が、画面上部の合成数と一致しているかのチェック。一致していたら上の数字の消去
        CheckFactorizationPerfect();

        //Originを適切に素因数分解できていれば、PrimeNumberCheckFieldのブロックをすべてCompletedFieldに送る
        MoveToCompletedField();

        //全てのブロックが静止している時間(allBlocksStandingStillTimer)を計算
        CountAllBlocksStandingStillTime();

        //ブロックを落下させてからの総時間を計算する
        CountTotalTimer();

        //ターンの切り替え条件をチェックし、必要であればターンを切り替える
        ChangeNextTurnProcess();
    }

    /// <summary>
    /// 全てのゲームオブジェクトが地面に設置しているかのチェック
    /// </summary>
    void CheckAllBlocksOnGround()
    {
        wereAllBlocksGroundedLastFrame = areAllBlocksGrounded; //1フレーム前のisGroundAllの保存
        areAllBlocksGrounded = true; //初期はtrueにしておく

        //primeNumberCheckField、completedField内のブロックが全て地面に設置しているか　設置していなければisGroundAllがfalseとなる
        CheckSingleFieldBlocksOnGround(primeNumberCheckField.transform);
        CheckSingleFieldBlocksOnGround(completedField.transform);
    }

    /// <summary>
    /// 引数で指定されたTransform上の子要素のすべてが、地面に設置しているのかをチェックする
    /// </summary>
    /// <param name="fieldTransform">どの親のtransformか</param>
    void CheckSingleFieldBlocksOnGround(Transform fieldTransform)
    {
        foreach (Transform block in fieldTransform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (blockInfo.enabled && !blockInfo.CheckIsGround()) //一つでも地面に接地してなければ
            {
                areAllBlocksGrounded = false; //isGroundAllはfalse
            }
        }
    }

    //生成したブロックの塊がOriginのものになっているかチェック、なっていなければ地震を発生させる primeNumberCheckFieldの子要素の数が変化したときに呼ばれる
    void CheckMatchingOrigin()
    {
        if (primeNumberCheckField.transform.childCount == prePrimeNumberCheckFieldCount) return;
        if (primeNumberCheckField.transform.childCount <= 0) return;
        if (!isDropBlockNowTurn) return;
        Transform lastBlock = primeNumberCheckField.transform.GetChild(primeNumberCheckField.transform.childCount - 1);
        int lastBlockNumber = lastBlock.GetComponent<BlockInfo>().GetPrimeNumber();
        
        //もし、画面上部の合成数がprimeNumberCheckField内の素数の積で割り切れるなら、割った値を表示
        if (originManager.CurrentOriginNumberDict.ContainsKey(lastBlockNumber))
        {
            originManager.RemovePrimeCurrentOriginNumberDict(lastBlockNumber);
            upperUIManager.ChangeDisplayText(UpperUIManager.KindOfUI.Origin, originManager.CurrentOriginNumber.ToString());
            prePrimeNumberCheckFieldCount = primeNumberCheckField.transform.childCount;//新しいブロックを正しく追加した場合のみ、1フレーム前のブロックの数を保存する
        }
        //素因数分解を間違えてしまった場合は地震を発生させ、最後に生成したブロックを生成しなかったことにする
        else
        {
            earthQuakeManager.TriggerEarthQuake();
            Destroy(lastBlock.gameObject);
            isDropBlockNowTurn = false;
        }
    }

    //素数ブロックの積が、画面上部の合成数と一致しているかのチェック。一致していたら上の数字の消去
    void CheckFactorizationPerfect()
    {
        //辞書の中身が全て取り出せていれば
        if (originManager.CurrentOriginNumberDict == null || originManager.CurrentOriginNumberDict.Count == 0)
        {
            SoundManager.Ins.PlayAudio(SoundManager.Ins.SE_DONE); //doneの再生
        }
    }

    //primeNumberCheckField内のブロックを全てcompletedFieldに移動させる。originの素因数分解が終了したときに呼ばれる
    void MoveToCompletedField()
    {
        if (originManager.CurrentOriginNumberDict == null || originManager.CurrentOriginNumberDict.Count == 0)
        {
            //まずは、blockFieldから移動する。
            List<Transform> blocksToMove = new List<Transform>();
            //すべての子オブジェクトを一時的なリストに追加。Transformをイテレートしながらtransformを変更しないように、一旦リストに追加。
            foreach (Transform block in primeNumberCheckField.transform)
            {
                blocksToMove.Add(block);
            }
            //一時的なリストを使用して子オブジェクトの親を変更
            foreach (Transform block in blocksToMove)
            {
                block.SetParent(completedField.transform);
            }
        }
    }

    //全てのゲームオブジェクトが連続で静止した時間をカウントする
    void CountAllBlocksStandingStillTime()
    {
        //ブロックの落下したターンであり、全てのゲームオブジェクトが動いておらず、地面にくっついていればTimerをカウント
        if (isDropBlockNowTurn && CheckAllBlocksStandingStill() && areAllBlocksGrounded)
        {
            standingStillTimer += Time.deltaTime;
            standingStillTimerVisualizer.SetActiveUI(true);
            standingStillTimerVisualizer.UpdateTimer(standingStillTimer);
        }
        //地面にくっついていなかったり、動いていたらタイマーをリセットする
        else
        {
            standingStillTimer = 0;
            standingStillTimerVisualizer.SetActiveUI(false);
        }
    }

    void CountTotalTimer()
    {
        if (isDropBlockNowTurn)
        {
            afterDropTotalTimer += Time.deltaTime;
        }
    }

    //全てのブロックがほぼ静止していればtrue
    bool CheckAllBlocksStandingStill()
    {
        foreach(Transform child in primeNumberCheckField.transform)
        {
            if (!CheckBlockStandStill(child)) return false;
        }
        return true;
    }

    //引数で受け取ったblock(Transform型)がほぼ静止していればtrue
    bool CheckBlockStandStill(Transform block)
    {
        if (block.GetComponent<Rigidbody2D>().velocity.magnitude < judgeStillStandingTime + AdditionalJudgeStillStandingTime)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 次のターンに進んでよいと判断されれば、高さの更新やスコアの更新も行い、対戦モードであれば、現在のターンの人の名前を切り替え、
    /// 次のターンに進めるか計測するためのタイマーやフラグを初期化する。
    /// </summary>
    void ChangeNextTurnProcess()
    {
        if(chengesNextTurn)
        {
            //高さの更新
            maxHeightCalculator.CalculateAllGameObjectsMaxHeight();

            //スコアを計算し、UIを更新
            CalculateAndDisplayScore();

            //ターンを切り替えて
            TurnMangaer.ChangeNextTurn();

            //対戦モードであれば、ターンが切り替わった後の人の名前を表示。
            if (nowTurnTextManager != null) nowTurnTextManager.DisplayNowTurnName();

            //初期化
            standingStillTimer = 0;
            afterDropTotalTimer = 0;
            isDropBlockNowTurn = false;
            standingStillTimerVisualizer.SetActiveUI(false);
        }
    }

    /// <summary>
    /// 各ゲームモードでのスコア計算
    /// すべてのブロックが地面に設置していれば、計算する
    /// </summary>
    void CalculateAndDisplayScore()
    {
        switch (GameModeManager.Ins.NowGameMode)
        {
            //もし積み上げモードで、すべてのブロックが地面に設置しており、ターンのチェンジ時であればスコアの更新。
            case GameModeManager.GameMode.PileUp:
                GameInfo.Variables.SetNowScore(scoreManager.CalculatePileUpScore());
                nowScoreText.text = GameInfo.Variables.GetNowScore().ToString();
                break;
            case GameModeManager.GameMode.PileUp_60s:
                GameInfo.Variables.SetNowScore(scoreManager.CalculatePileUpScore());
                nowScoreText.text = GameInfo.Variables.GetNowScore().ToString();
                break;
        }
    }

    //各ゲームモードでの最大スコアの表示(Awakeで呼ばれる。)
    void DisplayMaxScore()
    {
        switch (GameModeManager.Ins.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                maxScoreText.text = ScoreManager.Ins.AllScores[GameModeManager.Ins.NowGameMode][GameModeManager.Ins.NowDifficultyLevel][0].ToString();
                break;
            case GameModeManager.GameMode.PileUp_60s:
                maxScoreText.text = ScoreManager.Ins.AllScores[GameModeManager.Ins.NowGameMode][GameModeManager.Ins.NowDifficultyLevel][0].ToString();
                break;
        }
    }

    /// <summary>
    /// 現在のターンがブロックがドロップした後かを判定する変数(isDropBlockNowTurn)をtrueにする
    /// ブロックがドロップしたとき(primeNumberCheckFieldに送られたとき)に呼び出されるメソッド
    /// </summary>
    public void SetUpDropTurn()
    {
        isDropBlockNowTurn = true;
    }
}