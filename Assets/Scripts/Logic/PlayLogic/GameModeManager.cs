using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameModeManager;

/// <summary>
/// ゲームモードや、難易度を管理するクラス。
/// シングルトンパターンを扱っている。
/// </summary>
public class GameModeManager : MonoBehaviour
{
    //シングルトンのインスタンス
    private static GameModeManager instance;
    public static GameModeManager Ins => instance;

    //ゲームモード関係
    public enum GameMode
    {
        PileUp, //積み上げモード
        PileUp_60s, //時間制限のある積み上げモード
        Battle, //対戦モード(AI含む)
    }
    GameMode nowGameMode = GameMode.PileUp; //初期値は積み上げモード

    //難易度関係
    public enum DifficultyLevel
    {
        Normal,
        Difficult,
        Insane,
    }

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };
    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();
    [SerializeField] DifficultyLevel nowDifficultyLevel = DifficultyLevel.Difficult; //Json形式で保存するために、シリアライズ可能にしておく

    //プロパティ
    public string NowModeAndLevel => $"{nowGameMode}_{nowDifficultyLevel}";
    public int[] PrimeNumberPool => primeNumberPool;
    public List<int> NormalPool => normalPool;
    public List<int> DifficultPool => difficultPool;
    public List<int> InsanePool => insanePool;
    public GameMode NowGameMode => nowGameMode;
    public DifficultyLevel NowDifficultyLevel => nowDifficultyLevel;

    void Awake()
    {
        for (int i = 0; i < primeNumberPool.Length; i++)
        {
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 7) normalPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 13) difficultPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 23) insanePool.Add(primeNumberPool[i]);
        }
        if (instance == null)
        {
            instance = this; //単一のstaticインスタンスの生成。
            DontDestroyOnLoad(this.gameObject); //シーンの切り替え時に破棄されないようにする
        }
        LoadDifficultyLevelData();
    }

    public void SetGameMode(GameMode newGameMode)
    {
        instance.nowGameMode = newGameMode;
        Debug.Log($"ModeSet : {newGameMode}");
    }

    public int[] GetPrimeWithDifficultyLevel()
    {
        switch (instance.nowDifficultyLevel)
        {
            case DifficultyLevel.Normal:
                return normalPool.ToArray();
            case DifficultyLevel.Difficult:
                return difficultPool.ToArray();
            case DifficultyLevel.Insane:
                return insanePool.ToArray();
        }
        Debug.LogError("定義外の難易度の可能性があります。");
        return null;
    }

    //引数で与えられた素数から、その素数のインデックスを返す　見つからなければ-1
    public int GetPrimeNumberPoolIndex(int primeNumber)
    {
        int primeNumberPoolIndex = 0;
        foreach(var nowPrimeNumber in primeNumberPool)
        {
            if(nowPrimeNumber == primeNumber) return primeNumberPoolIndex;
            primeNumberPoolIndex++;
        }

        return -1;
    }

    public void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        instance.nowDifficultyLevel = newDifficultyLevel;
        SaveDifficultyLevelData();
        Debug.Log($"現在変更された難易度 : {newDifficultyLevel}");
    }

    public void SaveDifficultyLevelData()
    {
        GameModeManager dGameModeManagerInstance = instance;
        string jsonstr = JsonUtility.ToJson(dGameModeManagerInstance);
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/DifficultyLevel.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }
    public void LoadDifficultyLevelData()
    {
        if (!File.Exists(Application.persistentDataPath + "/DifficultyLevel.json")) { return; }
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/DifficultyLevel.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<GameModeData>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.nowDifficultyLevel = obj.nowDifficultyLevel;
    }
}

//Jsonからインスタンスを生成するためのクラス
class GameModeData
{
    public DifficultyLevel nowDifficultyLevel;
}
