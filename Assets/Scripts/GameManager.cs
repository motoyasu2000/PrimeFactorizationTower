using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upNumberText; //画面上部の合成数のテキスト
    [SerializeField] TextMeshProUGUI nextUpNumberText;
    [SerializeField] TextMeshProUGUI remainingNumberText;
    [SerializeField] TextMeshProUGUI MainText;
    [SerializeField] TextMeshProUGUI scoreText;

    int nowPhase = 1; //現在のphase
    int nowUpNumber = 1;

    int allBlockNumber = 1;

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

    private void Awake()
    {
        afterField = blockField.transform.Find("AfterField").gameObject;
        soundManager = SoundManager.SoundManagerInstance;
        scoreManager = ScoreManager.ScoreManagerInstance;
        gameModeManager = GameModeManager.GameModemanagerInstance;
        upNumberqueue.Enqueue(GenerateUpNumber());
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
            remainingNumberText.text = nowUpNumber.ToString(); //残りの数値を更新するタイミングで残りナンバーを更新する必要がある。
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
            remainingNumberText.text = (nowUpNumber / allBlockNumber).ToString(); //残りの数字を計算して描画。ただしafterFieldが空になるとこの中の処理が行われなくなるので
                                                                                  //UpNumberの更新のたびに、この値も更新してあげる必要がある。

            if (nowUpNumber % allBlockNumber != 0)
            {
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
                scoreText.text = ((int)(scoreManager.CalculateAllVerticesHeight()*1000)).ToString();
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

        switch (GameModeManager.GameModemanagerInstance.MyDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                for (int i = 0; i < 2 + (int)(UnityEngine.Random.value * nowPhase / 2) && i <= 5; i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.NormalPool.Count);
                    randomPrimeNumber = gameModeManager.NormalPool[randomIndex];
                    returnUpNumber *= randomPrimeNumber;
                }
                break;
                    
            case GameModeManager.DifficultyLevel.Difficult:
                for (int i = 0; i < 2 + (int)(UnityEngine.Random.value * nowPhase / 2) && i <= 5; i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.DifficultPool.Count);
                    randomPrimeNumber = gameModeManager.DifficultPool[randomIndex];
                    returnUpNumber *= randomPrimeNumber;
                }
                break;

            case GameModeManager.DifficultyLevel.Insane:
                for (int i = 0; i < 2 + (int)(UnityEngine.Random.value * nowPhase / 2) && i <= 5; i++)
                {
                    randomIndex = UnityEngine.Random.Range(0, gameModeManager.InsanePool.Count);
                    randomPrimeNumber = gameModeManager.InsanePool[randomIndex];
                    returnUpNumber *= randomPrimeNumber;
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

    public static void GameOver()
    {
        ScoreManager.ScoreManagerInstance.pileUpScores[10] = (int)(ScoreManager.ScoreManagerInstance.CalculateAllVerticesHeight() * 1000);
        Array.Sort(ScoreManager.ScoreManagerInstance.pileUpScores);
        Array.Reverse(ScoreManager.ScoreManagerInstance.pileUpScores);
        ScoreManager.ScoreManagerInstance.SaveScoreData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



}