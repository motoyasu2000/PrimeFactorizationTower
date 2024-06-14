using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//画面上部に表示される数字、Originを管理するクラス
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
        GenerateOrigin();
    }

    private void Update()
    {
        //今の数値が1なら、つまり素因数分解し終えたなら
        if(CurrentOriginNumber == 1)
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
        if (GameInfo.AILearning) originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 7, 13);
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

    public HashSet<int> GetCurrentOriginSet()
    {
        //Debug.Log(string.Join(",", CurrentOriginNumberDict.Keys));
        return new HashSet<int>( CurrentOriginNumberDict.Keys.Select(key => GameModeManager.Ins.GetPrimeNumberPoolIndex(key)));
    }
}
