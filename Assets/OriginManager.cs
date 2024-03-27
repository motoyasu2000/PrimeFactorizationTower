using Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OriginManager : MonoBehaviour
{
    bool isFirstGeneration = true;

    //キーが素数、バリューがその素数の数の辞書の生成
    Dictionary<int, int> originNumberDict = new Dictionary<int, int>();
    Dictionary<int, int> originNextNumberDict = new Dictionary<int, int>();
    public int OriginNumber => Helper.CalculateCompsiteNumberForDict(originNumberDict);
    public int OriginNextNumber => Helper.CalculateCompsiteNumberForDict(originNumberDict);

    GameModeManager gameModeManager;
    UpperUIManager upperUIManager;

    public Dictionary<int, int> OriginNumberDict => originNumberDict;

    void Awake()
    {
        gameModeManager = GameModeManager.Ins;
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        GenerateOrigin();
    }

    private void Update()
    {
        if(string.IsNullOrWhiteSpace(upperUIManager.OriginNumberText.text))
        {
            GenerateOrigin();
            Debug.Log("a");
        }
    }

    //条件を生成するメソッド(難易度ごとに異なる素数プール、異なる素数の数、異なる値の範囲で提供)
    public void GenerateOrigin()
    {
        //nextOriginをoriginに入れて
        originNumberDict = new Dictionary<int, int>(originNextNumberDict);

        //nextOriginの更新
        switch (GameModeManager.Ins.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.NormalPool, int.MaxValue, 3, 5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, int.MaxValue, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 2, 4);
                break;
        }

        //合成数の計算と表示
        int compositeNumberOrigin = Helper.CalculateCompsiteNumberForDict(originNumberDict);
        int compositeNumberNext = Helper.CalculateCompsiteNumberForDict(originNumberDict);
        upperUIManager.DisplayNumber(UpperUIManager.KindOfUI.Origin , compositeNumberOrigin.ToString());
        upperUIManager.DisplayNumber(UpperUIManager.KindOfUI.NextOrigin, compositeNumberNext.ToString());

        Debug.Log("Keys : " + string.Join(",", originNumberDict.Keys));
        Debug.Log("Values : " + string.Join(",", originNumberDict.Values));

        //最初だけ2回実行することで、originもnextも両方初期化する
        if (isFirstGeneration)
        {
            isFirstGeneration = false;
            GenerateOrigin();
        }
    }
}
