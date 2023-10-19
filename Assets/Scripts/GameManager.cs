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
    [SerializeField] TextMeshProUGUI remainingNumberText;

    int nowPhase = 1; //現在のphase
    int compositeNumber = 1;

    int allBlockNumber = 1;

    [SerializeField] GameObject blockField;
    [SerializeField] GameObject beforeField;
    GameObject afterField;
    [SerializeField] GameObject tmpField;
    [SerializeField] GameObject completedField;

    bool existTmpBlocks = false;
    bool isGroundAll = false;
    bool completeNumberFlag = false;


    void Start()
    {
        for(int i=0; i<primeNumberPool.Length; i++)
        {
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 7) normalPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 13) difficultPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 23) insanePool.Add(primeNumberPool[i]);
        }
        afterField = blockField.transform.Find("AfterField").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(upNumberText.text))//文字列が空であれば
        {
            upNumberText.text = GenerateUpNumber().ToString();
        }

        isGroundAll = true; //初期はtrueにしておく(現状使っていない)
        allBlockNumber = 1; //初期は1にしておく(現状使っていない)
        foreach (Transform block in afterField.transform) //afterFieldのチェック
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //一つでも地面に接地してなければ
            {
                isGroundAll = false; //isGroundAllはfalse
            }
            
            //素数の積を更新。
            allBlockNumber *= blockInfo.GetNumber();//もしblockの素数が上の合成数の素因数じゃなかったら
            remainingNumberText.text = (compositeNumber / allBlockNumber).ToString(); //残りの数字を計算して描画。ただしafterFieldが空になるとこの中の処理が行われなくなるので
                                                                                      //UpNumberの更新のたびに、この値も更新してあげる必要がある。
        }
        if (beforeField.transform.childCount != 0) //beforeFieldもチェック
        {
            if (!beforeField.transform.GetChild(0).GetComponent<BlockInfo>().CheckIsGround())
            {
                isGroundAll = false;
            }
            if (beforeField.transform.childCount >= 2)
            {
                Debug.LogError("beforeFieldに二つ以上のブロックが存在します。");
            }
        }
        foreach (Transform block in tmpField.transform) //tmpFieldのチェック afterFieldのぶろっくがそろった瞬間にtmpFieldに転送されるため
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            if (!blockInfo.CheckIsGround()) //一つでも地面に接地してなければ
            {
                isGroundAll = false; //isGroundAllはfalse
            }
        }


        if (allBlockNumber == compositeNumber) //もしブロックの数値の積が、上部の合成数と一致していたなら
        {
            completeNumberFlag = true;
        }

        if(completeNumberFlag)
        {
            SendTempBlocks();
            RemoveUpNumber();
        }

        if(isGroundAll && existTmpBlocks)
        {
            ConnectCompleteBlocks();
        }

        if (compositeNumber % allBlockNumber != 0 && isGroundAll) //数値がリセットされるのが素数ブロックがすべてそろったタイミングで、AfterFieldからCompletedFiledに送られるのが地面に設置したタイミング。この差を埋めるロジックを組む必要がある。→番号がそろった地点でtmpblocksに送信してあげる。そうすればafterblocks内での探索が行われない。
        {
            GameOver();
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
        compositeNumber = 1;

        if (myDifficultyLevel == DifficultyLevel.Normal)
        {
            for (int i=0; i<2+(int)(Random.value*nowPhase/2); i++)
            {
                randomIndex = Random.Range(0, normalPool.Count);
                randomPrimeNumber = normalPool[randomIndex];
                compositeNumber *= randomPrimeNumber;
            }
        }
        remainingNumberText.text = compositeNumber.ToString(); //残りの数値を更新するタイミングで残りナンバーを更新する必要がある。
        nowPhase++;
        return compositeNumber;
    }

    //上部の数字を消す関数
    void RemoveUpNumber()
    {
        upNumberText.text = ""; //テキストの初期化
        allBlockNumber = 1; //素数の積の初期化
        completeNumberFlag = false; //これがtrueの間はblockが生成されないようになっているので、removeの瞬間に直してあげるひつようがある。
    }

    //完成したブロックをいったんtempblockに転送する関数
    void SendTempBlocks()
    {
        //まずは、blockFieldから移動する。
        List<Transform> TmpBlocks = new List<Transform>();
        //すべての子オブジェクトを一時的なリストに追加。Transformをイテレートしながらtransformを変更しないように、一旦リストに追加。
        foreach (Transform block in afterField.transform)
        {
            TmpBlocks.Add(block);
        }
        foreach (Transform block in TmpBlocks)
        {
            block.SetParent(tmpField.transform);
        }
        existTmpBlocks = true;
    }

    //完成したブロックを結合し、親オブジェクトをcompletedFieldに転送する関数
    void ConnectCompleteBlocks()
    {
        //まずは、blockFieldから移動する。
        List<Transform> JointTmpBlocks = new List<Transform>();
        //すべての子オブジェクトを一時的なリストに追加。Transformをイテレートしながらtransformを変更しないように、一旦リストに追加。
        foreach (Transform block in tmpField.transform)
        {
            JointTmpBlocks.Add(block);
        }

        for (int i = 0; i < JointTmpBlocks.Count; i++)
        {
            //最後の要素は0番目の要素と結合
            if (i >= JointTmpBlocks.Count - 1)
            {
                FixedJoint2D fixedJoint = JointTmpBlocks[i].gameObject.AddComponent<FixedJoint2D>();
                fixedJoint.connectedBody = JointTmpBlocks[0].GetComponent<Rigidbody2D>();
            }
            //次の番号を持つオブジェクトと結合
            else
            {
                FixedJoint2D fixedJoint = JointTmpBlocks[i].gameObject.AddComponent<FixedJoint2D>();
                fixedJoint.connectedBody = JointTmpBlocks[i + 1].GetComponent<Rigidbody2D>();
            }
            JointTmpBlocks[i].SetParent(tmpField.transform);
        }

        //一時的なリストを使用して子オブジェクトの親を変更
        foreach (Transform block in JointTmpBlocks)
        {
            block.SetParent(completedField.transform);
        }
        existTmpBlocks = false;
    }
    public static void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
