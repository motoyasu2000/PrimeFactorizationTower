using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Common;

//ゲームを管理するクラス。Originの生成や表示、生成されたブロックが条件を満たすかの判定、ゲームオーバーの判定、ターンの管理などなど。
public class GameManager : MonoBehaviour
{
    //UI
    TextMeshProUGUI nowScoreText;
    TextMeshProUGUI maxScoreText;
    GameObject explainPileUp; //チュートリアル時のテキスト
    UpperUIManager upperUIManager;
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
    float standingStillTimer = 0; //全てのゲームオブジェクトが連続で静止している時間
    static readonly float changeTurnTime = 1f; //全てのゲームオブジェクトがどれだけの時間静止すればターンが切り替わるのか
    static readonly float judgeStillStanding = 0.01f; //ゲームオブジェクトの速度がどのくらいなら静止しているとみなすか
    public bool IsDropBlockNowTurn => isDropBlockNowTurn;
    //全てのゲームオブジェクトの静止時間が基準を超えており、かつ、このターン内にブロックが生成されていればターンを切り替える
    bool ChengeNextTurnFlag => (standingStillTimer > changeTurnTime) && isDropBlockNowTurn;

    //その他
    int nowPhase = 0; //現在いくつの合成数を素因数分解し終えたか　これが増えると上に表示される合成数の値が大きくなるなどすることが可能。
    MaxHeightCalculator maxHeightCalculator; //高さの計算を行う

    //初期化処理
    private void Awake()
    {
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        nowScoreText = GameObject.Find("NowScoreText").GetComponent<TextMeshProUGUI>();
        maxScoreText = GameObject.Find("MaxScoreText").GetComponent<TextMeshProUGUI>();
        standingStillTimerVisualizer = GameObject.Find("StandingStillTimerVisualizer").GetComponent<StandingStillTimerVisualizer>();
        blockField = GameObject.Find("BlockField");
        primeNumberCheckField = blockField.transform.Find("PrimeNumberCheckField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
        scoreManager = ScoreManager.Ins;
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        earthQuakeManager = GameObject.Find("EarthQuakeManager").GetComponent<EarthQuakeManager>();
        explainPileUp = GameObject.Find("Canvas").transform.Find("ExplainPileUp").gameObject;
        maxHeightCalculator = GameObject.Find("MaxHeightCalculator").GetComponent<MaxHeightCalculator>();
        DisplayerMaxScore();
    }

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) explainPileUp.gameObject.SetActive(true); //セーブデータがなければ説明を行う。
    }

    void Update()
    {
        //全てのブロックが地面に設置しているかのチェック
        CheckAllBlocksOnGround();

        //PrimeNumberCheckField内部の合成数を計算。Originの条件を満たしていなければゲームオーバー
        CheckMatchingOrigin();

        //PrimeNumberCheckField内部の合成数の積が画面上部の数値と一致していたらいるかのチェック。
        CheckFactorizationPerfect();

        //Originを適切に素因数分解できていれば、PrimeNumberCheckFieldのブロックをすべてCompletedFieldに送る
        MoveToCompletedField();

        //全てのブロックが静止している時間(allBlocksStandingStillTimer)を計算
        CountAllBlocksStandingStillTime();

        //ターンの切り替え条件をチェックし、必要であればターンを切り替える
        ChangeNextTurnProcess();
    }

    //全てのゲームオブジェクトが地面に設置しているかのチェック
    void CheckAllBlocksOnGround()
    {
        wereAllBlocksGroundedLastFrame = areAllBlocksGrounded; //1フレーム前のisGroundAllの保存
        areAllBlocksGrounded = true; //初期はtrueにしておく

        //primeNumberCheckField、completedField内のブロックが全て地面に設置しているか　設置していなければisGroundAllがfalseとなる
        CheckSingleFieldBlocksOnGround(primeNumberCheckField.transform);
        CheckSingleFieldBlocksOnGround(completedField.transform);
    }

    //引数で指定されたTransform上の子要素のすべてが、地面に設置しているのかをチェックする
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

    //テキストの描画、場合によってはゲームオーバー primeNumberCheckFieldの子要素の数が変化したときに呼ばれる
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

    //primeNumberCheckField内のブロックを全てcompletedFieldに移動させる。
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
        else
        {
            standingStillTimer = 0;
            standingStillTimerVisualizer.SetActiveUI(false);
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
        if (block.GetComponent<Rigidbody2D>().velocity.magnitude < judgeStillStanding)
            return true;
        else
            return false;
    }

    //次のターンに進んでよいか判断し、進んでよければ進んで初期化。また、高さの更新やスコアの更新も行う
    void ChangeNextTurnProcess()
    {
        if(ChengeNextTurnFlag)
        {
            //高さの更新
            maxHeightCalculator.CalculateAllGameObjectsMaxHeight();

            //スコアを計算し、UIを更新
            CalculateAndDisplayScore();

            //ターンを切り替えて
            TurnMangaer.ChangeNextTurn();

            //初期化
            standingStillTimer = 0;
            isDropBlockNowTurn = false;
        }
    }

    //各ゲームモードでのスコア計算
    void CalculateAndDisplayScore()
    {
        switch (GameModeManager.Ins.NowGameMode)
        {
            //もし積み上げモードで、すべてのブロックが地面に設置しており、ターンのチェンジ時であればスコアの更新。
            case GameModeManager.GameMode.PileUp:
                if (areAllBlocksGrounded)
                {
                    GameInfo.Variables.SetNowScore(scoreManager.CalculatePileUpScore());
                    nowScoreText.text = GameInfo.Variables.GetNowScore().ToString();
                }
                break;
        }
    }

    //各ゲームモードでの最大スコアの表示(Awakeで呼ばれる。)
    void DisplayerMaxScore()
    {
        switch (GameModeManager.Ins.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                maxScoreText.text = ScoreManager.Ins.PileUpScores[GameModeManager.Ins.NowDifficultyLevel][0].ToString();
                break;
        }
    }

    //ブロックがドロップしたとき(primeNumberCheckFieldに送られたとき)に呼び出されるメソッド
    public void DropBlockProcess()
    {
        isDropBlockNowTurn = true;
    }
}