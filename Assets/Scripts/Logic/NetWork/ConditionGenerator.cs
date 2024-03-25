using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Common;

//左上に表示される条件を生成するためのクラス
public class ConditionGenerator : MonoBehaviour
{
    GameModeManager gameModeManager;
    ConditionNumberTextManager conditionNumberManager;

    void Awake()
    {
        gameModeManager = GameModeManager.Ins;
        conditionNumberManager = GameObject.Find("ConditonNumber").GetComponent<ConditionNumberTextManager>();
    }

    //条件を生成するメソッド(難易度ごと)
    public Dictionary<int,int> GenerateCondition()
    {
        //キーが素数、バリューがその素数の数の辞書の生成(難易度ごと)
        Dictionary<int,int> conditionNumberDict = new Dictionary<int,int>();
        switch (GameModeManager.Ins.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                conditionNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.NormalPool,int.MaxValue,3,5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                conditionNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, int.MaxValue, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                conditionNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 2, 4);
                break;
        }
        
        //合成数の計算と表示
        int compositeNumber = Helper.CalculateCompsiteNumberForDict(conditionNumberDict);
        conditionNumberManager.PrintConditionNumber(compositeNumber.ToString());

        Debug.Log("Keys : " + string.Join(",", conditionNumberDict.Keys));
        Debug.Log("Values : " + string.Join(",", conditionNumberDict.Values));

        return conditionNumberDict;
    }
}
