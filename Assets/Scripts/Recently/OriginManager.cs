using Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

public class OriginManager : MonoBehaviour
{
    bool isFirstGeneration = true;

    //キーが素数、バリューがその素数の数の辞書の生成
    Dictionary<int, int> startOriginNumberDict = new Dictionary<int, int>();
    Dictionary<int, int> currentOriginNumberDict = new Dictionary<int, int>();
    Dictionary<int, int> originNextNumberDict = new Dictionary<int, int>();
    public Dictionary<int, int> StartoriginNumberDict => startOriginNumberDict;
    public Dictionary<int,int> CurrentOriginNumberDict => currentOriginNumberDict;
    public Dictionary<int, int> OriginNextNumberDict => originNextNumberDict;
    public int OriginNumber => Helper.CalculateCompsiteNumberForDict(startOriginNumberDict);
    public int CurrentOriginNumber => Helper.CalculateCompsiteNumberForDict(currentOriginNumberDict);
    public int OriginNextNumber => Helper.CalculateCompsiteNumberForDict(originNextNumberDict);

    GameModeManager gameModeManager;
    UpperUIManager upperUIManager;


    void Awake()
    {
        gameModeManager = GameModeManager.Ins;
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        GenerateOrigin();
    }

    private void Update()
    {
        if(upperUIManager.OriginNumberText.text == "1")
        {
            GenerateOrigin();
        }
    }

    //条件を生成するメソッド(難易度ごとに異なる素数プール、異なる素数の数、異なる値の範囲で提供)
    public void GenerateOrigin()
    {
        //nextOriginをoriginに入れて
        startOriginNumberDict = new Dictionary<int, int>(originNextNumberDict);
        currentOriginNumberDict = new Dictionary<int, int>(originNextNumberDict);

        //nextOriginの更新
        switch (GameModeManager.Ins.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.NormalPool, 5000, 3, 5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, 20000, 3, 5);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 10, 10);
                break;
        }

        //合成数の計算と表示
        upperUIManager.ChangeDisplayText(UpperUIManager.KindOfUI.Origin , OriginNumber.ToString());
        upperUIManager.ChangeDisplayText(UpperUIManager.KindOfUI.NextOrigin, OriginNextNumber.ToString());

        //Debug.Log("Keys : " + string.Join(",", startOriginNumberDict.Keys));
        //Debug.Log("Values : " + string.Join(",", startOriginNumberDict.Values));

        //最初だけ2回実行することで、originもnextも両方初期化する
        if (isFirstGeneration)
        {
            isFirstGeneration = false;
            GenerateOrigin();
        }
    }

    public void RemovePrimeCurrentOriginNumberDict(int primeNumber)
    {
        if (currentOriginNumberDict.ContainsKey(primeNumber))
        {
            currentOriginNumberDict[primeNumber]--;
            if (currentOriginNumberDict[primeNumber] == 0)
            {
                currentOriginNumberDict.Remove(primeNumber);
            }
        }
        else
        {
            Debug.LogError("存在しないキーを選択しています。");
        }
    }
}
