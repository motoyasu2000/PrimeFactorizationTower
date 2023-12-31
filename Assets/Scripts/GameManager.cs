using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upNumberText; //画面上部の合成数のテキスト
    [SerializeField] TextMeshProUGUI nextUpNumberText;
    [SerializeField] TextMeshProUGUI MainText;
    [SerializeField] TextMeshProUGUI scoreText;
    GameObject gameOverMenu;
    GameObject explainPileUp;

    int nowPhase = 1; //現在のphase
    int nowUpNumber = 1;
    int allBlockNumber = 1;
    int compositeNumber_GO; //ゲームオーバー時の合成数6y
    int primeNumber_GO; //ゲームオーバー時の素数
    public int CompositeNumber_GO => compositeNumber_GO;
    public int PrimeNumber_GO => primeNumber_GO;
    int oldMaxScore = -1;
    int newScore = -1;
    public int OldMaxScore => oldMaxScore;
    public int NewScore => newScore;

    [SerializeField] GameObject blockField;
    GameObject afterField;
    [SerializeField] GameObject completedField;

    bool isGroundAll = false;
    bool isGroundAll_past = false;
    public bool IsGroundAll => isGroundAll;
    public bool IsGroundAll_past => isGroundAll_past;
    bool completeNumberFlag = false;

    [SerializeField] Queue<int> upNumberqueue = new Queue<int>();

    SoundManager soundManager;
    ScoreManager scoreManager;
    GameModeManager gameModeManager;
    BloomManager bloomManager;

    bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    float gameOverTimer = 0;

    private void Awake()
    {
        afterField = blockField.transform.Find("AfterField").gameObject;
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
        if (!File.Exists(Application.dataPath + "/Savedata/Score/PileUp.json")) explainPileUp.gameObject.SetActive(true); //セーブデータがなければ説明を行う。
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(upNumberText.text))//文字列が空であれば
        {
            upNumberqueue.Enqueue(GenerateUpNumber());
            nowUpNumber = upNumberqueue.Dequeue();
            upNumberText.text = nowUpNumber.ToString();
            nextUpNumberText.text = upNumberqueue.Peek().ToString();
        }

        isGroundAll_past = isGroundAll;

        isGroundAll = true; //初期はtrueにしておく(現状使っていない)
        allBlockNumber = 1; //初期は1にしておく(現状使っていない)
        foreach (Transform block in afterField.transform) //すべてのゲームオブジェクトのチェック
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //一つでも地面に接地してなければ
            {
                isGroundAll = false; //isGroundAllはfalse
            }

            allBlockNumber *= blockInfo.GetNumber();//もしblockの素数が上の合成数の素因数じゃなかったら
            upNumberText.text = (nowUpNumber / allBlockNumber).ToString(); //残りの数字を計算して描画。ただしafterFieldが空になるとこの中の処理が行われなくなるので
                                                                                  //UpNumberの更新のたびに、この値も更新してあげる必要がある。

            if (nowUpNumber % allBlockNumber != 0)
            {
                if (isGameOver) break;
                //最後のゲームオーバー理由の出力の際に、元の合成数とその時選択してしまった素数の情報が必要なので、変数に入れておく。
                compositeNumber_GO = nowUpNumber * afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetNumber() / allBlockNumber;
                primeNumber_GO = afterField.transform.GetChild(afterField.transform.childCount - 1).GetComponent<BlockInfo>().GetNumber();
                Debug.Log(compositeNumber_GO);
                Debug.Log(primeNumber_GO);
                GameOver();
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

        //もし積み上げモードで、地面に設置しているなら高さを計算する。
        if (gameModeManager.NowGameMode == GameModeManager.GameMode.PileUp)
        {
            if (isGroundAll)
            {
                newScore = (int)(scoreManager.CalculateAllVerticesHeight() * 1000);
                scoreText.text = newScore.ToString();
            }
        }

        if (allBlockNumber == nowUpNumber) //もしブロックの数値の積が、上部の合成数と一致していたなら
        {
            completeNumberFlag = true;
        }

        //合成数達成時の処理
        if (completeNumberFlag)
        {
            RemoveUpNumber(); //上の数字の消去
            soundManager.PlayAudio(soundManager.SE_DONE); //doneの再生
        }

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

    public bool GetCompleteNumberFlag()
    {
        return completeNumberFlag;
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
        upNumberText.text = ""; //テキストの初期化
        allBlockNumber = 1; //素数の積の初期化
        completeNumberFlag = false; //これがtrueの間はblockが生成されないようになっているので、removeの瞬間に直してあげるひつようがある。
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

    public void PostGameOver()
    {
        gameOverMenu.SetActive(true);
        soundManager.StopAudio(soundManager.BGM_PLAY);
        SoundManager.LoadSoundData();
    }


}