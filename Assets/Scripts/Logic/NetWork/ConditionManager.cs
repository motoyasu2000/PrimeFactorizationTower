using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Common;

//左上に表示される条件を生成するためのクラス
public class ConditionManager : MonoBehaviour
{
    //キーが素数、バリューがその素数の数の辞書の生成
    Dictionary<int, int> conditionNumberDict = new Dictionary<int, int>();
    GameModeManager gameModeManager;
    UpperUIManager upperUIManager;

    public Dictionary<int, int> ConditionNumberDict => conditionNumberDict;

    void Awake()
    {
        gameModeManager = GameModeManager.Ins;
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        GenerateCondition();
    }

    //条件を生成するメソッド(難易度ごとに異なる素数プール、異なる素数の数、異なる値の範囲で提供)
    public void GenerateCondition()
    {
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
        upperUIManager.ChangeDisplayText(UpperUIManager.KindOfUI.Condition, compositeNumber.ToString());

        Debug.Log("Keys : " + string.Join(",", conditionNumberDict.Keys));
        Debug.Log("Values : " + string.Join(",", conditionNumberDict.Values));
    }
}
