using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;

public class GameManager : MonoBehaviour
{
    //UI
    TextMeshProUGUI nowUpCompositeNumberText;
    TextMeshProUGUI nextUpCompositeNumberText;
    TextMeshProUGUI nowScoreText;
    GameObject gameOverMenu;
    GameObject explainPileUp; //チュートリアル時のテキストの表示

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
    int nowUpNumber = 1;
    int nowPrimeNumberProduct = 1;
    bool completeCompositeNumberFlag = false;
    Queue<int> upNumberqueue = new Queue<int>();
    SoundManager soundManager;

    //ゲームオーバー処理
    int compositeNumber_GO; //ゲームオーバー時の合成数
    int primeNumber_GO; //ゲームオーバー時の素数
    float gameOverTimer = 0;
    bool isGameOver = false;
    BloomManager bloomManager; //ゲームオーバー時の演出用
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;
    public bool IsGameOver => isGameOver;

    //ブロックの親オブジェクト候補
    GameObject blockField; //下二つの様なブロックの親オブジェクトをまとめる親オブジェクト
    GameObject afterField; //ブロックを落下させた瞬間、そのブロックは、このゲームオブジェクトの子要素となる
    GameObject completedField; //afterField内のブロックの積が画面上部の合成数と一致したら、それらのブロックはこのゲームオブジェクトの子要素になる

    //その他
    GameModeManager gameModeManager; //難易度ごとに生成する合成数が異なるので、現在の難易度の情報を持つGamemodemanagerの情報が必要
                                     //また、スコアを保存する際、どの難易度のスコアを更新するかの情報も必要なので、そこでも使う。
    int nowPhase = 0; //現在いくつの合成数を素因数分解し終えたか　これが増えると上に表示される合成数の値が大きくなるなどする。

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
        upNumberqueue.Enqueue(GenerateUpNumber());
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        explainPileUp = GameObject.Find("Canvas").transform.Find("ExplainPileUp").gameObject;
    }

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) explainPileUp.gameObject.SetActive(true); //セーブデータがなければ説明を行う。
    }

    // Update is called once per frame
    void Update()
    {
        UpCompositeNumberSetting();
        CheckAllBlocksOnGround(); 
        CheckPrimeNumberProduct();
        CalculateScore();
        CountGameOverTime();
    }

    public bool GetCompleteNumberFlag()
    {
        return completeCompositeNumberFlag;
    }

    int GenerateUpNumber()
    {
        int randomIndex;
        int randomPrimeNumber;
        int returnUpNumber = 1;

        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                for (int i = 0; i <UnityEngine.Random.Range(2,5); i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.NormalPool.Count);
                    randomPrimeNumber = gameModeManager.NormalPool[randomIndex];
                    if (returnUpNumber * randomPrimeNumber < 10000) returnUpNumber *= randomPrimeNumber;
                }
                break;
                    
            case GameModeManager.DifficultyLevel.Difficult:
                for (int i = 0; i < UnityEngine.Random.Range(3, 6); i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.DifficultPool.Count);
                    randomPrimeNumber = gameModeManager.DifficultPool[randomIndex];
                    if (returnUpNumber * randomPrimeNumber < 10000) returnUpNumber *= randomPrimeNumber;
                }
                break;

            case GameModeManager.DifficultyLevel.Insane:
                for (int i = 0; i < UnityEngine.Random.Range(3, 7); i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.InsanePool.Count);
                    randomPrimeNumber = gameModeManager.InsanePool[randomIndex];
                    if(returnUpNumber * randomPrimeNumber < 100000) returnUpNumber *= randomPrimeNumber;
                }
                break;
        }

        nowPhase++;
        return returnUpNumber;
        return 16 * 27 * 125 * 343;
    }

    //画面上部に表示される合成数や、ネクストの合成数の設定を行う
    void UpCompositeNumberSetting()
    {
        //画面上部の合成数がが空であれば、つまり素因数分解が完了したならば
        if (string.IsNullOrWhiteSpace(nowUpCompositeNumberText.text))
        {
            upNumberqueue.Enqueue(GenerateUpNumber());
            nowUpNumber = upNumberqueue.Dequeue();
            nowUpCompositeNumberText.text = nowUpNumber.ToString();
            nextUpCompositeNumberText.text = upNumberqueue.Peek().ToString();
        }
    }

    //全てのゲームオブジェクトが地面に設置しているかのチェック
    void CheckAllBlocksOnGround()
    {
        isGroundAll_past = isGroundAll;
        isGroundAll = true; //初期はtrueにしておく
        foreach (Transform block in afterField.transform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //一つでも地面に接地してなければ
            {
                isGroundAll = false; //isGroundAllはfalse
            }
        }
        foreach (Transform block in completedField.transform)
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
        nowPrimeNumberProduct = 1; //初期は1にしておく
        foreach (Transform block in afterField.transform) //afterField内の全てのゲームオブジェクトのチェック
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();

            nowPrimeNumberProduct *= blockInfo.GetPrimeNumber();//もしblockの素数が上の合成数の素因数じゃなかったら
            nowUpCompositeNumberText.text = (nowUpNumber / nowPrimeNumberProduct).ToString(); //残りの数字を計算して描画。ただしafterFieldが空になるとこの中の処理が行われなくなるので
                                                                                           //UpNumberの更新のたびに、この値も更新してあげる必要がある。

            if (nowUpNumber % nowPrimeNumberProduct != 0)
            {
                if (isGameOver) break;
                //最後のゲームオーバー理由の出力の際に、元の合成数とその時選択してしまった素数の情報が必要なので、変数に入れておく。
                compositeNumber_GO = nowUpNumber * afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber() / nowPrimeNumberProduct;
                primeNumber_GO = afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetPrimeNumber();
                Debug.Log(compositeNumber_GO);
                Debug.Log(primeNumber_GO);
                GameOver();
            }
        }
        //もしブロックの数値の積が、上部の合成数と一致していたなら
        if (nowPrimeNumberProduct == nowUpNumber) 
        {
            completeCompositeNumberFlag = true;
        }
        //画面上部の合成数達成時の処理
        if (completeCompositeNumberFlag)
        {
            RemoveUpNumber(); //上の数字の消去
            soundManager.PlayAudio(soundManager.SE_DONE); //doneの再生
        }
    }

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

    void RemoveUpNumber()
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
        nowUpCompositeNumberText.text = ""; //テキストの初期化
        nowPrimeNumberProduct = 1; //素数の積の初期化
        completeCompositeNumberFlag = false; //これがtrueの間はblockが生成されないようになっているので、removeの瞬間に直してあげるひつようがある。
    }

    public void GameOver()
    {
        if (isGameOver) return;
        //ソート前に過去の最高スコアの情報を取得しておく(のちにこのゲームで最高スコアを更新したかを確認するため)
        oldMaxScore = scoreManager.PileUpScores[gameModeManager.NowDifficultyLevel][0];

        scoreManager.InsertPileUpScoreAndSort(newScore);
        scoreManager.SaveScoreData();
        isGameOver = true;
        bloomManager.isLightUpStart = true;
        soundManager.FadeOutVolume();
    }

    void CountGameOverTime()
    {
        if (isGameOver)
        {
            gameOverTimer += Time.deltaTime;
            if (gameOverTimer > 1.2f)
            {
                PostGameOver();
                gameOverTimer = float.MinValue;
            }
        }
    }

    //ゲームオーバー処理を行った後の後処理
    void PostGameOver()
    {
        gameOverMenu.SetActive(true);
        soundManager.StopAudio(soundManager.BGM_PLAY);
        SoundManager.LoadSoundData();
    }


}