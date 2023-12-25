using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameModeManager;

public class GameModeManager : MonoBehaviour
{
    //シングルトンのインスタンス
    private static GameModeManager instance;

    public static GameModeManager GameModemanagerInstance => instance;



    //難易度を表す列挙型の定義
    public enum DifficultyLevel
    {
        Normal,
        Difficult,
        Insane
    }
    public enum GameMode
    {
        PileUp, //積み上げモード
    }

    public DifficultyLevel myDifficultyLevel = DifficultyLevel.Difficult; //難易度型の変数を定義、とりあえずNormalで初期化 適切なタイミングで難易度調整ができるように切り替える必要がある。
    public DifficultyLevel MyDifficultyLevel => myDifficultyLevel;
    GameMode nowGameMode = GameMode.PileUp; //初期値は積み上げモード
    public GameMode NowGameMode => nowGameMode;

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };

    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();
    public List<int> NormalPool => normalPool;
    public List<int> DifficultPool => difficultPool;
    public List<int> InsanePool => insanePool;

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

    public void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        instance.myDifficultyLevel = newDifficultyLevel;
        SaveDifficultyLevelData();
        Debug.Log($"現在変更された難易度 : {newDifficultyLevel}");
    }

    public void SaveDifficultyLevelData()
    {
        GameModeManager dGameModeManagerInstance = instance;
        string jsonstr = JsonUtility.ToJson(dGameModeManagerInstance);
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Savedata/System/DifficultyLevel.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }
    public void LoadDifficultyLevelData()
    {
        StreamReader reader = new StreamReader(Application.dataPath + "/Savedata/System/SoundSetting.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<JsonLoadGameModeManager>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.myDifficultyLevel = obj.myDifficultyLevel;
    }
}

//Jsonからインスタンスを生成するためのクラス
class JsonLoadGameModeManager
{
    public DifficultyLevel myDifficultyLevel;
}
