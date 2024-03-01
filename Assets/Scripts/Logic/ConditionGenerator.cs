using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

//左上に表示される条件を生成するためのクラス
public class ConditionGenerator : MonoBehaviour
{
    GameModeManager gameModeManager;
    ConditionNumberTextManager conditionNumberManager;

    void Awake()
    {
        gameModeManager = GameModeManager.GameModemanagerInstance;
        conditionNumberManager = GameObject.Find("ConditonNumber").GetComponent<ConditionNumberTextManager>();
    }

    //条件を生成するメソッド(難易度ごと)
    public Dictionary<int,int> GenerateCondition()
    {
        //キーが素数、バリューがその素数の数の辞書の生成(難易度ごと)
        Dictionary<int,int> returnDict = new Dictionary<int,int>();
        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                returnDict = GenerateConditionForDifficultyLevel(gameModeManager.NormalPool,3,5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                returnDict = GenerateConditionForDifficultyLevel(gameModeManager.DifficultPool, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                returnDict = GenerateConditionForDifficultyLevel(gameModeManager.InsanePool, 2, 4);
                break;
        }
        
        //合成数の計算と表示
        int compositeNumber = DictToCompositeNumber(returnDict);
        conditionNumberManager.PrintConditionNumber(compositeNumber.ToString());

        //デバッグ
        Debug.Log("Keys : " + string.Join(",", returnDict.Keys));
        Debug.Log("Values : " + string.Join(",", returnDict.Values));

        return returnDict;
    }

    //素数プール、乱数の最小値最大値を指定して、キーが素数、バリューがその素数の数の辞書を作るメソッド
    Dictionary<int, int> GenerateConditionForDifficultyLevel(List<int> primeNumberPool, int minRand, int maxRand)
    {
        Dictionary<int, int> returnDict = new Dictionary<int, int>();
        int randomIndex;
        int randomPrimeNumber;
        int numberOfPrimeNumber = Random.Range(minRand, maxRand);

        for (int i = 0; i < numberOfPrimeNumber; i++)
        {
            randomIndex = Random.Range(0, primeNumberPool.Count);
            randomPrimeNumber = primeNumberPool[randomIndex];
            if (!returnDict.TryAdd(randomPrimeNumber, 1)) returnDict[randomPrimeNumber] += 1;
        }
        return returnDict;
    }

    //キーが素数、バリューがその素数の数の辞書から合成数を計算するメソッド
    int DictToCompositeNumber(Dictionary<int, int> returnDict)
    {
        int compositeNumber = 1;
        foreach(var pair in returnDict){
            compositeNumber *= (int)Math.Pow(pair.Key, pair.Value);
        }
        return compositeNumber;
    }
}
