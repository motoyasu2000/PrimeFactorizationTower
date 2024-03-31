using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//AIの行動をまとめたクラス
public class AIActions : MonoBehaviour
{
    //現在の状況で選ぶべき素数のスコア。ただしゲーム側の制限で全てのキーが生成できるわけではないので、生成できる中で最もスコアの高いものを選択するロジックにする。
    Dictionary<int, float> primeNumberScores = new Dictionary<int, float>();
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
        touchBlock = beforeField.GetComponent<TouchBlock>();
        touchBlock.MoveBlockXAndRelease(x);
    }

    //最も選ぶべきブロックを生成する。
    public void GenerateBlock()
    {
        //AIが求めた最も確率の高いキーを取得
        CalculatePrimeNumberProbabilities();
        int highestProbKey = primeNumberScores
            .Where(kvp => originManager.CurrentOriginNumberDict.ContainsKey(kvp.Key))
            .OrderByDescending(kvp => kvp.Value)
            .First()
            .Key;
        blockGenerator.GenerateBlock_HundleAI(highestProbKey);
    }

    public void CalculatePrimeNumberProbabilities() {
        //primeNumberProbabilitiesをAIが計算するようにする。
    }
}
