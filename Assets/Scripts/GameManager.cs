using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //難易度を表す列挙型の定義
    public enum DifficultyLevel
    {
        Normal,
        difficult,
        Insane
    }

    DifficultyLevel myDifficultyLevel = DifficultyLevel.Normal; //難易度型の変数を定義、とりあえずNormalで初期化 適切なタイミングで難易度調整ができるように切り替える必要がある。

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };

    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();

    [SerializeField] TextMeshProUGUI upNumberText; //画面上部の合成数のテキスト
    [SerializeField] TextMeshProUGUI nextUpNumberText;
    [SerializeField] TextMeshProUGUI remainingNumberText;

    int nowPhase = 1; //現在のphase
    int nowUpNumber = 1;

    int allBlockNumber = 1;

    [SerializeField] GameObject blockField;
    GameObject afterField;
    [SerializeField] GameObject completedField;

    bool isGroundAll = false;
    bool completeNumberFlag = false;

    [SerializeField]Queue<int> upNumberqueue = new Queue<int>();


    void Start()
    {
        for(int i=0; i<primeNumberPool.Length; i++)
        {
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 7) normalPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 13) difficultPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 23) insanePool.Add(primeNumberPool[i]);
        }
        afterField = blockField.transform.Find("AfterField").gameObject;
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

        if(allBlockNumber == nowUpNumber) //もしブロックの数値の積が、上部の合成数と一致していたなら
        {
            completeNumberFlag = true;
        }

        if(completeNumberFlag)
        {
            RemoveUpNumber();
        }
    }

    public bool GetCompleteNumberFlag()
    {
        return completeNumberFlag;
    }

    void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        myDifficultyLevel = newDifficultyLevel;
    }

    int GenerateUpNumber()
    {
        int randomIndex;
        int randomPrimeNumber;
        int returnUpNumber = 1;

        if (myDifficultyLevel == DifficultyLevel.Normal)
        {
            for (int i = 0; i < 2 + (int)(Random.value * nowPhase / 2); i++)
            {
                randomIndex = Random.Range(0, normalPool.Count);
                randomPrimeNumber = normalPool[randomIndex];
                returnUpNumber *= randomPrimeNumber;
            }
        }
        
        nowPhase++;
        return returnUpNumber;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
