using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameModeManager;

//ゲームモードや、難易度を管理するクラス。シングルトンパターンを扱っている。
public class GameModeManager : MonoBehaviour
{
    //シングルトンのインスタンス
    private static GameModeManager instance;
    public static GameModeManager GameModemanagerInstance => instance;

    //ゲームモード関係
    public enum GameMode
    {
        PileUp, //積み上げモード
    }
    GameMode nowGameMode = GameMode.PileUp; //初期値は積み上げモード
    public GameMode NowGameMode => nowGameMode;

    //難易度関係
    public enum DifficultyLevel
    {
        Normal,
        Difficult,
        Insane
    }

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };
    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();
    DifficultyLevel nowDifficultyLevel = DifficultyLevel.Difficult; //難易度型の変数を定義、とりあえずNormalで初期化 適切なタイミングで難易度調整ができるように切り替える必要がある。
    public int[] PrimeNumberPool => primeNumberPool;
    public List<int> NormalPool => normalPool;
    public List<int> DifficultPool => difficultPool;
    public List<int> InsanePool => insanePool;
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

    public int[] GetGameModeMatchDifficultyLevel()
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
        var obj = JsonUtility.FromJson<JsonLoadGameModeManager>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.nowDifficultyLevel = obj.nowDifficultyLevel;
    }
}

//Jsonからインスタンスを生成するためのクラス
class JsonLoadGameModeManager
{
    public DifficultyLevel nowDifficultyLevel;
}
