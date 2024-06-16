using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 画面上部に表示される数字、Originを管理するクラス
/// </summary>
public class OriginManager : MonoBehaviour
{
    //初めての生成かどうか(初めての生成の場合はNextも生成するために、2回生成する必要がある。)
    bool isFirstGeneration = true;

    //キーが素数、バリューがその素数の数の辞書
    Dictionary<int, int> startOriginNumberDict = new Dictionary<int, int>(); //生成されたときの初めのもの
    Dictionary<int, int> currentOriginNumberDict = new Dictionary<int, int>(); //ブロックを生成してその分減少したもの

    Dictionary<int, int> originNextNumberDict = new Dictionary<int, int>(); //ネクストのもの
    public Dictionary<int, int> StartoriginNumberDict => startOriginNumberDict;
    public Dictionary<int,int> CurrentOriginNumberDict => currentOriginNumberDict;

    public Dictionary<int, int> OriginNextNumberDict => originNextNumberDict;

    //上の辞書を数値に変換したもの
    public int OriginNumber => Helper.CalculateCompsiteNumberForDict(startOriginNumberDict);
    public int CurrentOriginNumber => Helper.CalculateCompsiteNumberForDict(currentOriginNumberDict);

    public int OriginNextNumber => Helper.CalculateCompsiteNumberForDict(originNextNumberDict);

    GameModeManager gameModeManager;
    UpperUIManager upperUIManager;


    void Awake()
    {
        gameModeManager = GameModeManager.Ins;
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        UpdateOriginAndNextOrigin();
    }

    private void Update()
    {
        //今の数値が1なら、つまり素因数分解し終えたなら
        if(CurrentOriginNumber == 1)
        {
            UpdateOriginAndNextOrigin();
        }
    }

    /// <summary>
    /// 難易度に応じてOriginを更新する。NextOriginをOriginに送り、NextOriginを生成する
    /// </summary>
    public void UpdateOriginAndNextOrigin()
    {
        MoveNextOriginToOrigin();
        GenerateNextOrigin();

        //更新したOriginとNextOriginの表示
        upperUIManager.ChangeDisplayText(UpperUIManager.KindOfUI.Origin , OriginNumber.ToString());
        upperUIManager.ChangeDisplayText(UpperUIManager.KindOfUI.NextOrigin, OriginNextNumber.ToString());

        //最初だけ2回実行することで、OriginもNextOriginも両方初期化する
        if (isFirstGeneration)
        {
            isFirstGeneration = false;
            UpdateOriginAndNextOrigin();
        }
    }

    //NextOriginをOriginに入れる
    void MoveNextOriginToOrigin()
    {
        startOriginNumberDict = new Dictionary<int, int>(originNextNumberDict);
        currentOriginNumberDict = new Dictionary<int, int>(originNextNumberDict);
    }

    void GenerateNextOrigin()
    {
        if (GameInfo.AILearning) originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 4, 13);
        else
        {
            switch (GameModeManager.Ins.NowDifficultyLevel)
            {
                case GameModeManager.DifficultyLevel.Normal:
                    originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.NormalPool, 5000, 3, 3);
                    break;

                case GameModeManager.DifficultyLevel.Difficult:
                    originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, 10000, 3, 5);
                    break;

                case GameModeManager.DifficultyLevel.Insane:
                    originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 3, 5);
                    break;
            }
        }
    }

    //現在のOriginを扱う辞書をから引数で指定したprimeNumberを一つ減らす
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

    //Originに存在する素数を集合として返す
    public HashSet<int> GetCurrentOriginSet()
    {
        //Debug.Log(string.Join(",", CurrentOriginNumberDict.Keys));
        return new HashSet<int>( CurrentOriginNumberDict.Keys.Select(key => GameModeManager.Ins.GetPrimeNumberPoolIndex(key)));
    }
}
