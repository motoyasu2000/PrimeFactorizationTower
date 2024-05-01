using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//AIの行動をまとめたクラス
public class AIActions : MonoBehaviour
{
    public int GeneratedPrimeNumber;
    GameObject beforeField;
    OriginManager originManager;
    BlockGenerator blockGenerator;
    BlockSpiner blockSpiner;
    TouchBlock touchBlock;

    void Start()
    {
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        blockGenerator = GetComponent<BlockGenerator>();
        blockSpiner = GameObject.Find("PrimeNumberGeneratingPoint").GetComponent<BlockSpiner>();
        beforeField = GameObject.Find("BeforeField");
    }

    //引数で与えられた数だけブロックを45度回転させる
    public void SpinBlock45SeveralTimes(int numberOfTime)
    {
        for (int i = 0; i < numberOfTime; i++)
        {
            blockSpiner.SpinSingleBlock_45();
        }
    }

    //引数で与えられたx座標にブロックを生成する
    public void MoveBlockXAndRelease(float x)
    {
        touchBlock = beforeField.transform.GetChild(0).GetComponent<TouchBlock>();
        touchBlock.Initialize();
        touchBlock.MoveBlockXAndRelease(x);
    }

    //最も選ぶべきブロックを生成する。
    public void GenerateBlock(Dictionary<int, float> primeNumberScores)
    {
        int highestProbKey = primeNumberScores
            .Where(kvp => originManager.CurrentOriginNumberDict.ContainsKey(kvp.Key))
            .OrderByDescending(kvp => kvp.Value)
            .First()
            .Key;
        blockGenerator.GenerateBlock_HundleAI(highestProbKey);
        GeneratedPrimeNumber = highestProbKey;
    }

    public void GenerateBlock(int primeKey)
    {
        blockGenerator.GenerateBlock_HundleAI(primeKey);
    }


}
