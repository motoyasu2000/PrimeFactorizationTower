using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    //シングルトンのインスタンス
    private static GameModeManager instance;

    public static GameModeManager GameModemanagerInstance => instance;



    //難易度を表す列挙型の定義
    public enum DifficultyLevel
    {
        Normal,
        difficult,
        Insane
    }
    public enum GameMode
    {
        PileUp, //積み上げモード
    }

    DifficultyLevel myDifficultyLevel = DifficultyLevel.difficult; //難易度型の変数を定義、とりあえずNormalで初期化 適切なタイミングで難易度調整ができるように切り替える必要がある。
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
        instance = this; //単一のstaticインスタンスの生成。
    }

    public static void SetGameMode(GameMode newGameMode)
    {
        instance.nowGameMode = newGameMode;
        Debug.Log($"ModeSet : {newGameMode}");
    }

    public static void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        instance.myDifficultyLevel = newDifficultyLevel;
    }
}
