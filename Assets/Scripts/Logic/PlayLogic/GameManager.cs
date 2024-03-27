using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Random = UnityEngine.Random;
using AWS;
using Common;

//ゲームを管理するクラス。画面上部の合成数の計算や表示、ゲームオーバーの管理などを行う。
public class GameManager : MonoBehaviour
{
    //UI
    TextMeshProUGUI nowUpCompositeNumberText;
    TextMeshProUGUI nextUpCompositeNumberText;
    TextMeshProUGUI nowScoreText;
    GameObject gameOverMenu;
    GameObject explainPileUp; //チュートリアル時のテキスト

    //スコアの管理
    int oldMaxScore = -1;
    int newScore = -1;
    bool areAllBlocksGrounded = false; //全てのブロックが地面に設置しているか。スコア計算の条件やターンの切り替えの条件で使う
    bool wereAllBlocksGroundedLastFrame = false; //1フレーム前のareAllBlocksGrounded
    ScoreManager scoreManager;
    DynamoDBManager ddbManager;
    public bool AreAllBlocksGrounded => areAllBlocksGrounded;
    public bool WereAllBlocksGroundedLastFrame => wereAllBlocksGroundedLastFrame;
    public bool IsBreakScore => (oldMaxScore < newScore); //スコアを更新したかを判定するフラグ
    public int OldMaxScore => oldMaxScore;
    public int NewScore => newScore;

    //画面中央の合成数の管理や、素因数分解ができているかのチェック
    bool completeCompositeNumberFlag = false;
    int currentUpCompositeNumber = 1;
    int targetUpCompositeNumber = 1;
    Dictionary<int, int> upCompositeNumberDict;
    Queue<int> upCompositeNumberqueue = new Queue<int>();
    SoundManager soundManager;

    //ゲームオーバー処理
    const float delayTime = 1.2f;
    int compositeNumber_GO; //ゲームオーバー時の合成数
    int primeNumber_GO; //ゲームオーバー時の素数
    bool isGameOver = false; //ゲームオーバーになったらこのフラグをtrueにし、falseの時のみゲームオーバーの処理を実行するようにすることで、ゲームオーバーの処理が1度しか呼ばれないようにする。
    BloomManager bloomManager; //ゲームオーバー時の演出用
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;

    //ブロックの親オブジェクト候補
    GameObject blockField; //下二つの様なブロックの親オブジェクトをまとめる親オブジェクト
    GameObject primeNumberCheckField; //ブロックを落下させた瞬間、そのブロックは、このゲームオブジェクトの子要素となる
    GameObject completedField; //afterField内のブロックの積が画面上部の合成数と一致したら、それらのブロックはこのゲームオブジェクトの子要素になる

    //ターンの切り替え
    bool isDropBlockNowTurn = false;
    float allBlocksStandingStillTimer = 0; //全てのゲームオブジェクトが連続で静止している時間
    const float changeTurnTime = 0.4f; //全てのゲームオブジェクトがどれだけの時間静止すればターンが切り替わるのか
    const float stillStandingScale = 0.05f; //ゲームオブジェクトの速度がどのくらいなら静止しているとみなすか
    public bool IsDropBlockNowTurn => isDropBlockNowTurn;

    //その他
    int nowPhase = 0; //現在いくつの合成数を素因数分解し終えたか　これが増えると上に表示される合成数の値が大きくなるなどすることが可能。
    GameModeManager gameModeManager; //難易度ごとに生成する合成数が異なるので、現在の難易度の情報を持つGamemodemanagerの情報が必要
                                     //また、スコアを保存する際、どの難易度のスコアを更新するかの情報も必要なので、そこでも使う。



    //初期化処理
    private void Awake()
    {
        ddbManager = GameObject.Find("DynamoDBManager").GetComponent<DynamoDBManager>();
        nowUpCompositeNumberText = GameObject.Find("NowUpCompositeNumberText").GetComponent<TextMeshProUGUI>();
        nextUpCompositeNumberText = GameObject.Find("NextUpCompositeNumberText").GetComponent<TextMeshProUGUI>();
        nowScoreText = GameObject.Find("NowScoreText").GetComponent<TextMeshProUGUI>();
        blockField = GameObject.Find("BlockField");
        primeNumberCheckField = blockField.transform.Find("PrimeNumberCheckField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
        soundManager = SoundManager.Ins;
        scoreManager = ScoreManager.Ins;
        gameModeManager = GameModeManager.Ins;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
        upCompositeNumberDict = GenerateUpCompositeNumberDict();
        upCompositeNumberqueue.Enqueue(Helper.CalculateCompsiteNumberForDict(upCompositeNumberDict));
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        explainPileUp = GameObject.Find("Canvas").transform.Find("ExplainPileUp").gameObject;
    }

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) explainPileUp.gameObject.SetActive(true); //セーブデータがなければ説明を行う。
    }

    void Update()
    {
        //全てのブロックが地面に設置しているかのチェック
        CheckAllBlocksOnGround();

        //PrimeNumberCheckField内部の合成数を計算
        CalculateNowPrimeNumberProduct();

        //PrimeNumberCheckField内部の合成数の素因数分解が間違っているかのチェック、間違っていたらゲームーオーバー
        CheckFactorizationIncorrect();

        //PrimeNumberCheckField内部の合成数の積が画面上部の数値と一致していたらいるかのチェック。一致していたら上の文字を消去
        CheckFactorizationPerfect();

        //画面上部に表示される合成数が消去されていたら、更新を行う
        UpCompositeNumberSetting();

        //スコアを計算し、UIを更新
        CalculateScore();

        //全てのブロックが静止している時間(allBlocksStandingStillTimer)を計算
        CountAllBlocksStandingStillTime();

        //ターンの切り替え条件をチェックし、必要であればターンを切り替える
        CheckNextTurnChangeable();
    }

    //画面上部に表示される合成数や、ネクストの合成数の設定を行う
    void UpCompositeNumberSetting()
    {
        //画面上部の合成数がが空であれば、つまり素因数分解が完了したならば
        if (string.IsNullOrWhiteSpace(nowUpCompositeNumberText.text))
        {
            completeCompositeNumberFlag = false; //これがtrueの間はblockが生成されないようになっているので、画面上部の合成数が更新された瞬間にfalseにしてあげる。
            upCompositeNumberDict = GenerateUpCompositeNumberDict();
            upCompositeNumberqueue.Enqueue(Helper.CalculateCompsiteNumberForDict(upCompositeNumberDict));
            targetUpCompositeNumber = upCompositeNumberqueue.Dequeue();
            nowUpCompositeNumberText.text = targetUpCompositeNumber.ToString();
            nextUpCompositeNumberText.text = upCompositeNumberqueue.Peek().ToString();
        }
    }

    //あらゆる難易度に合わせて、画面上部の合成数を生成する。難易度によって使われる素数プール、素数の数、合成数の最大値が異なる。
    Dictionary<int, int> GenerateUpCompositeNumberDict()
    {
        Dictionary<int, int> upCompositeNumbersDict = new Dictionary<int, int>();

        switch (GameModeManager.Ins.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                upCompositeNumbersDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.NormalPool, 3000, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                upCompositeNumbersDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, 10000, 3, 6);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                upCompositeNumbersDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, 100000, 3, 7);
                break;
            default:
                Debug.LogError("予想外の難易度で素数が生成されようとしました。");
                break;
        }

        nowPhase++;
        return upCompositeNumbersDict;
    }

    //全てのゲームオブジェクトが地面に設置しているかのチェック
    void CheckAllBlocksOnGround()
    {
        wereAllBlocksGroundedLastFrame = areAllBlocksGrounded; //1フレーム前のisGroundAllの保存
        areAllBlocksGrounded = true; //初期はtrueにしておく

        //afterField、completedField内のブロックが全て地面に設置しているか　設置していなければisGroundAllがfalseとなる
        CheckSingleFieldBlocksOnGround(primeNumberCheckField.transform);
        CheckSingleFieldBlocksOnGround(completedField.transform);
    }

    //引数で指定されたTransform上の子要素のすべてが、地面に設置しているのかをチェックする
    void CheckSingleFieldBlocksOnGround(Transform fieldTransform)
    {
        foreach (Transform block in fieldTransform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //一つでも地面に接地してなければ
            {
                areAllBlocksGrounded = false; //isGroundAllはfalse
            }
        }
    }

    //素数ブロックの積が、画面上部の合成数と一致しているかのチェック。一致していたら上の数字の消去
    void CheckFactorizationPerfect()
    {
        //もしブロックの数値の積が、上部の合成数と一致していたなら
        if (currentUpCompositeNumber == targetUpCompositeNumber)
        {
            completeCompositeNumberFlag = true;
            soundManager.PlayAudio(soundManager.SE_DONE); //doneの再生
            RemoveUpCompositeNumber(); //上の数字の消去
        }
    }

    //PrimeNumberCheckField内の合成数に応じてゲームオーバーにするかのチェックを行う。
    void CheckFactorizationIncorrect()
    {
        //PrimeNumberCheckField内の素因数分解が間違っていないかのチェック　間違っていればゲームオーバー
        if (targetUpCompositeNumber % currentUpCompositeNumber != 0)
        {
            GameOver(true);
        }
    }

    //afterField内のブロックの積を計算、nowPrimeNumberProductを更新、テキストの描画
    void CalculateNowPrimeNumberProduct()
    {
        currentUpCompositeNumber = 1;
        foreach (Transform block in primeNumberCheckField.transform) //afterField内の全てのゲームオブジェクトのチェック
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();

            currentUpCompositeNumber *= blockInfo.GetPrimeNumber();
            //もし、画面上部の合成数がafterfield内の素数の積で割り切れるなら、割った値を表示、割り切れなかったらEと表示
            if (targetUpCompositeNumber % currentUpCompositeNumber == 0)
            {
                nowUpCompositeNumberText.text = (targetUpCompositeNumber / currentUpCompositeNumber).ToString(); //残りの数字を計算して描画。ただしafterFieldが空になるとこの中の処理が行われなくなるので
                                                                                                           //UpCompositeNumberの更新のたびに、このテキストの値も更新してあげる必要がある。
            }
            else
            {
                nowUpCompositeNumberText.text = "E";
                nowUpCompositeNumberText.color = Color.red;
            }

        }
    }

    //各ゲームモードでのスコア計算
    void CalculateScore()
    {
        //もし積み上げモードで、地面に設置しているなら高さを計算する。
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                if (areAllBlocksGrounded)
                {
                    newScore = scoreManager.CalculatePileUpScore();
                    nowScoreText.text = newScore.ToString();
                }
                break;
        }
    }

    //画面上部中央の合成数を消去し、afterField内のブロックを全てcompletedFieldに移動させる。
    void RemoveUpCompositeNumber()
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
        nowUpCompositeNumberText.text = "";
        currentUpCompositeNumber = 1;
    }

    //全てのゲームオブジェクトが連続で静止した時間をカウントする
    void CountAllBlocksStandingStillTime()
    {
        //全てのゲームオブジェクトが動いておらず、地面にくっついていればTimerをカウント
        if (CheckAllBlocksStandingStill() && areAllBlocksGrounded)
        {
            allBlocksStandingStillTimer += Time.deltaTime;
        }
        else
        {
            allBlocksStandingStillTimer = 0;
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
        if (block.GetComponent<Rigidbody2D>().velocity.magnitude < stillStandingScale)
            return true;
        else
            return false;
    }

    //次のターンに進んでよいか判断し、進んでよければ進んで初期化
    void CheckNextTurnChangeable()
    {
        //全てのゲームオブジェクトの静止時間が基準を超えており、かつ、このターン内にブロックが生成されていれば
        if(allBlocksStandingStillTimer > changeTurnTime && isDropBlockNowTurn)
        {
            //ターンを切り替えて
            TurnMangaer.NextTurn();

            //初期化
            allBlocksStandingStillTimer = 0;
            isDropBlockNowTurn = false;
        }
    }

    public async void GameOver(bool isFactorizationIncorrect)
    {
        //このメソッドが1度しか呼ばれないように
        if (isGameOver) return;
        else isGameOver = true;

        Debug.Log("GameOver");

        //素因数分解を間違えてしまった場合、最後のゲームオーバー理由の出力の際に、元の合成数とその時選択してしまった素数の情報が必要なので、変数に入れておく。
        if (isFactorizationIncorrect)
        {
            compositeNumber_GO = targetUpCompositeNumber * primeNumberCheckField.transform.GetChild(primeNumberCheckField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber() / currentUpCompositeNumber;
            primeNumber_GO = primeNumberCheckField.transform.GetChild(primeNumberCheckField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber();
        }

        //スコアの更新とゲームオーバー時の演出、後処理の呼び出し。
        oldMaxScore = scoreManager.PileUpScores[gameModeManager.NowDifficultyLevel][0]; //ソート前に過去の最高スコアの情報を取得しておく(のちにこのゲームで最高スコアを更新したかを確認するため)
        scoreManager.InsertPileUpScoreAndSort(newScore);
        scoreManager.SaveScoreData();
        bloomManager.LightUpStart();
        soundManager.FadeOutVolume();
        //スコアを更新していれば、データベースの更新
        if (IsBreakScore) await ddbManager.SaveScoreAsyncHandler(GameModeManager.Ins.ModeAndLevel, newScore);

        StartCoroutine(PostGameOver(delayTime));
    }


    //ゲームオーバー後、一定時間後にゲームオーバーメニューを表示し、bgmのストップ。ゲームオーバーの後処理
    IEnumerator PostGameOver(float time)
    {
        yield return new WaitForSeconds(time);
        gameOverMenu.SetActive(true);
        soundManager.StopAudio(soundManager.BGM_PLAY);
        SoundManager.LoadSoundSettingData();
    }

    public bool GetCompleteNumberFlag()
    {
        return completeCompositeNumberFlag;
    }

    //ブロックがドロップしたとき(afterFieldに送られたとき)に呼び出されるメソッド
    public void DropBlockProcess()
    {
        isDropBlockNowTurn = true;
    }
}