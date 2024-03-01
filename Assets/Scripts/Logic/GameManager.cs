using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Random = UnityEngine.Random;

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
    bool isGroundAll = false; //全てのブロックが地面に設置しているかをチェックする変数。falseであれば、高さを計算しない。
    bool isGroundAll_past = false; //1フレーム前のisGroundAll
    ScoreManager scoreManager;
    public bool IsGroundAll => isGroundAll;
    public bool IsGroundAll_past => isGroundAll_past;
    public int OldMaxScore => oldMaxScore;
    public int NewScore => newScore;

    //画面中央の合成数の管理や、素因数分解ができているかのチェック
    int nowUpCompositeNumber = 1;
    int nowPrimeNumberProduct = 1;
    bool completeCompositeNumberFlag = false;
    Queue<int> upCompositeNumberqueue = new Queue<int>();
    SoundManager soundManager;

    //ゲームオーバー処理
    int compositeNumber_GO; //ゲームオーバー時の合成数
    int primeNumber_GO; //ゲームオーバー時の素数
    BloomManager bloomManager; //ゲームオーバー時の演出用
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;

    //ブロックの親オブジェクト候補
    GameObject blockField; //下二つの様なブロックの親オブジェクトをまとめる親オブジェクト
    GameObject afterField; //ブロックを落下させた瞬間、そのブロックは、このゲームオブジェクトの子要素となる
    GameObject completedField; //afterField内のブロックの積が画面上部の合成数と一致したら、それらのブロックはこのゲームオブジェクトの子要素になる

    //その他
    GameModeManager gameModeManager; //難易度ごとに生成する合成数が異なるので、現在の難易度の情報を持つGamemodemanagerの情報が必要
                                     //また、スコアを保存する際、どの難易度のスコアを更新するかの情報も必要なので、そこでも使う。
    int nowPhase = 0; //現在いくつの合成数を素因数分解し終えたか　これが増えると上に表示される合成数の値が大きくなるなどすることが可能。

    //初期化処理
    private void Awake()
    {
        nowUpCompositeNumberText = GameObject.Find("NowUpCompositeNumberText").GetComponent<TextMeshProUGUI>();
        nextUpCompositeNumberText = GameObject.Find("NextUpCompositeNumberText").GetComponent<TextMeshProUGUI>();
        nowScoreText = GameObject.Find("NowScoreText").GetComponent<TextMeshProUGUI>();
        blockField = GameObject.Find("BlockField");
        afterField = blockField.transform.Find("AfterField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
        soundManager = SoundManager.SoundManagerInstance;
        scoreManager = ScoreManager.ScoreManagerInstance;
        gameModeManager = GameModeManager.GameModemanagerInstance;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
        upCompositeNumberqueue.Enqueue(GenerateCompositeNumber());
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        explainPileUp = GameObject.Find("Canvas").transform.Find("ExplainPileUp").gameObject;
    }

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) explainPileUp.gameObject.SetActive(true); //セーブデータがなければ説明を行う。
    }

    void Update()
    {
        UpCompositeNumberSetting();
        CheckAllBlocksOnGround(); 
        CheckPrimeNumberProduct();
        CalculateScore();
    }


    //画面上部に表示される合成数や、ネクストの合成数の設定を行う
    void UpCompositeNumberSetting()
    {
        //画面上部の合成数がが空であれば、つまり素因数分解が完了したならば
        if (string.IsNullOrWhiteSpace(nowUpCompositeNumberText.text))
        {
            completeCompositeNumberFlag = false; //これがtrueの間はblockが生成されないようになっているので、画面上部の合成数が更新された瞬間にfalseにしてあげる。
            upCompositeNumberqueue.Enqueue(GenerateCompositeNumber());
            nowUpCompositeNumber = upCompositeNumberqueue.Dequeue();
            nowUpCompositeNumberText.text = nowUpCompositeNumber.ToString();
            nextUpCompositeNumberText.text = upCompositeNumberqueue.Peek().ToString();
        }
    }

    //現在の難易度がどの様になっていたとしても、その難易度に合った合成数を生成する
    int GenerateCompositeNumber()
    {
        int upCompositeNumber = -1;

        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                upCompositeNumber = GenerateCompositeNumberForDifficultyLevel(gameModeManager.NormalPool, 3000, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                upCompositeNumber = GenerateCompositeNumberForDifficultyLevel(gameModeManager.DifficultPool, 10000, 3, 6);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                upCompositeNumber = GenerateCompositeNumberForDifficultyLevel(gameModeManager.DifficultPool, 100000, 3, 7);
                break;
        }

        nowPhase++;
        return upCompositeNumber;
        return 16 * 27 * 125 * 343;
    }

    //指定した素数プールから合成数を生成する。合成数の上限値や、素数の数も乱数の上限値を書くことで指定することができる。
    int GenerateCompositeNumberForDifficultyLevel(List<int> primeNumberPool, int maxCompositeNumber ,int minRand, int maxRand)
    {
        int randomIndex;
        int randomPrimeNumber;
        int upCompositeNumber = 1;
        int numberOfPrimeNumber = Random.Range(minRand, maxRand);

        for(int i=0; i<numberOfPrimeNumber; i++)
        {
            randomIndex = Random.Range(0, primeNumberPool.Count);
            randomPrimeNumber = primeNumberPool[randomIndex];
            if(upCompositeNumber * randomPrimeNumber < maxCompositeNumber) upCompositeNumber *= randomPrimeNumber;
        }

        return upCompositeNumber;
    }

    //全てのゲームオブジェクトが地面に設置しているかのチェック
    void CheckAllBlocksOnGround()
    {
        isGroundAll_past = isGroundAll; //1フレーム前のisGroundAllの保存
        isGroundAll = true; //初期はtrueにしておく

        //afterField、completedField内のブロックが全て地面に設置しているか　設置していなければisGroundAllがfalseとなる
        CheckSingleFieldBlocksOnGround(afterField.transform);
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
                isGroundAll = false; //isGroundAllはfalse
            }
        }
    }

    //素数ブロックの積が、画面上部の合成数の因数になっているかのチェック
    void CheckPrimeNumberProduct()
    {
        //AfterField内の合成数を計算し、素因数分解が間違っていないかのチェック　間違っていればゲームオーバー
        CalculateNowPrimeNumberProduct();
        if (nowUpCompositeNumber % nowPrimeNumberProduct != 0)
        {
            GameOver(true);
        }

        //もしブロックの数値の積が、上部の合成数と一致していたなら
        if (nowPrimeNumberProduct == nowUpCompositeNumber)
        {
            completeCompositeNumberFlag = true;
            RemoveUpCompositeNumber(); //上の数字の消去
            soundManager.PlayAudio(soundManager.SE_DONE); //doneの再生
        }
    }

    //afterField内のブロックの積を計算、nowPrimeNumberProductを更新、テキストの描画
    void CalculateNowPrimeNumberProduct()
    {
        nowPrimeNumberProduct = 1;
        foreach (Transform block in afterField.transform) //afterField内の全てのゲームオブジェクトのチェック
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();

            nowPrimeNumberProduct *= blockInfo.GetPrimeNumber();
            //もし、画面上部の合成数がafterfield内の素数の積で割り切れるなら、割った値を表示、割り切れなかったらEと表示
            if(nowUpCompositeNumber % nowPrimeNumberProduct == 0)
            {
                nowUpCompositeNumberText.text = (nowUpCompositeNumber / nowPrimeNumberProduct).ToString(); //残りの数字を計算して描画。ただしafterFieldが空になるとこの中の処理が行われなくなるので
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
                if (isGroundAll)
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
        foreach (Transform block in afterField.transform)
        {
            blocksToMove.Add(block);
        }
        //一時的なリストを使用して子オブジェクトの親を変更
        foreach (Transform block in blocksToMove)
        {
            block.SetParent(completedField.transform);
        }
        nowUpCompositeNumberText.text = "";
        nowPrimeNumberProduct = 1; 
    }

    public void GameOver(bool missPrimeNumberfactorization)
    {
        //素因数分解を間違えてしまった場合、最後のゲームオーバー理由の出力の際に、元の合成数とその時選択してしまった素数の情報が必要なので、変数に入れておく。
        if(missPrimeNumberfactorization){
            compositeNumber_GO = nowUpCompositeNumber * afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber() / nowPrimeNumberProduct;
            primeNumber_GO = afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber();
        }

        //スコアの更新とゲームオーバー時の演出、後処理の呼び出し。
        oldMaxScore = scoreManager.PileUpScores[gameModeManager.NowDifficultyLevel][0]; //ソート前に過去の最高スコアの情報を取得しておく(のちにこのゲームで最高スコアを更新したかを確認するため)
        scoreManager.InsertPileUpScoreAndSort(newScore);
        scoreManager.SaveScoreData();
        bloomManager.LightUpStart();
        soundManager.FadeOutVolume();

        const float delayTime = 1.2f;
        StartCoroutine(PostGameOver(delayTime));
    }


    //ゲームオーバー後、一定時間後にゲームオーバーメニューを表示し、bgmのストップ。ゲームオーバー後の後処理
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
}