using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameModeManager;

/// <summary>
/// スコア管理を担当するクラス。
/// ゲーム中のスコア計算、保存・読み込み、シーン間でのスコアデータの保持を行う。
/// シングルトンパターンを使用している
/// </summary>

public class ScoreManager : MonoBehaviour
{
    MaxHeightCalculator maxHeightCalculator;

    //各レベルのスコアは要素数11の配列として保存される。0~9番目が過去のランキング、10番目に最新のスコアが入り、ソートされるという仕組み
    [SerializeField] Dictionary<GameMode, Dictionary<DifficultyLevel, int[]>> allScores = new Dictionary<GameMode, Dictionary<DifficultyLevel, int[]>>();　//Json形式で保存するために、シリアライズ可能にしておく
    public Dictionary<GameMode, Dictionary<DifficultyLevel, int[]>> AllScores => allScores;

    //インスタンス
    private static ScoreManager instance;
    public static ScoreManager Ins => instance;

    //シングルトンパターンを使用して、ScoreManagerのインスタンスを管理。
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        LoadScoreData();
    }

    //シーンロードのコールバックを設定し、初期化処理を行う。
    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoadProcess;
        InitializeFields();
    }

    //現在の高さからスコアを計算する
    public int CalculatePileUpScore()
    {
        const int scoreMultiplier = 1000; //高さにかかるスコア倍率
        float height = maxHeightCalculator.NowHeight;
        return (int)(height * scoreMultiplier);
    }



    //シーンのロード時に実行されるメソッド
    void SceneLoadProcess(Scene scene, LoadSceneMode mode)
    {
        InitializeFields();
    }

    /// <summary>
    /// シーンロード時に呼ばれる初期化メソッド
    /// 列挙型GameModeで定義されているのに、辞書のキー内に存在しないゲームモードがあったら、そのモードのキーを追加する。
    /// 列挙型DifficultyLevelで定義されているのに、辞書のキー内に存在しない難易度があったら、その難易度のキーを追加する。
    /// </summary>

    void InitializeFields()
    {

        foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
        {
            if (!instance.allScores.ContainsKey(gameMode))
            {
                instance.allScores[gameMode] = new Dictionary<DifficultyLevel, int[]>();
            }
            foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
            {
                if (!instance.allScores[gameMode].ContainsKey(level))
                {
                    instance.allScores[gameMode][level] = new int[GameInfo.RankDisplayLimit + 1];
                }
            }
        }

        //ゲームシーンにあるゲームオブジェクト名を使って変数を作っているので 現在PlayScene以外ならこの後の処理を行わない。
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("PlayScene")) return;
        instance.maxHeightCalculator = GameObject.Find("MaxHeightCalculator").GetComponent<MaxHeightCalculator>();
    }

    //新しいスコアをスコアを管理する辞書に追加し、ソートを行う。
    public void AddAndSortScore(int newScore)
    {
        DifficultyLevel nowLevel = GameModeManager.Ins.NowDifficultyLevel;
        GameMode nowGameMode = GameModeManager.Ins.NowGameMode;
        Ins.allScores[nowGameMode][nowLevel][GameInfo.RankDisplayLimit] = newScore;
        Array.Sort(Ins.allScores[nowGameMode][nowLevel]);
        Array.Reverse(Ins.allScores[nowGameMode][nowLevel]);
    }

    //現在のスコアをjson形式で保存
    public void SaveScoreData()
    {
        GameModeScores_EachDiffLevel eachLevelScores = new GameModeScores_EachDiffLevel();
        eachLevelScores.SetScoresEachLevel(instance.allScores[GameModeManager.Ins.NowGameMode]);
        string jsonstr = JsonUtility.ToJson(eachLevelScores);
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + $"/{GameModeManager.Ins.NowGameMode}.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    //json形式で保存されたデータを読み込む
    public void LoadScoreData()
    {
        foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
        {
            if (!File.Exists(Application.persistentDataPath + $"/{gameMode}.json")) { return; }
            StreamReader reader = new StreamReader(Application.persistentDataPath + $"/{gameMode}.json");
            string datastr = reader.ReadToEnd();
            reader.Close();
            var obj = JsonUtility.FromJson<GameModeScores_EachDiffLevel>(datastr);
            instance.allScores[gameMode]=obj.GetScoresEachLevel();
        }
    }



    //////////////////////////////////////////
    //以下、シリアライズを行うための一時的なクラス//
    //////////////////////////////////////////

    /// <summary>
    /// シリアライズ可能な配列を持ったシリアライズ可能なクラス
    /// 他次元配列や辞書はシリアライズできないため必要
    /// </summary>
    [Serializable]
    public class Top10Score
    {
        [SerializeField] int[] scores;

        public Top10Score()
        {
            scores = new int[GameInfo.RankDisplayLimit + 1];
        }

        public void SetTop10Score(int[] newScores)
        {
            scores = newScores;
        }

        public int[] GetTop10Score()
        {
            return scores;
        }
    }

    /// <summary>
    /// シリアライズ可能なクラスのリストを持ったシリアライズ可能なクラス
    /// 難易度ごとのトップ10のスコア
    /// </summary>
    [Serializable]
    class GameModeScores_EachDiffLevel
    {
        //シリアライズ可能なクラスのリストを使用 難易度ごとのトップ10のスコア
        [SerializeField] private List<Top10Score> scoresEachDiffLevel = new List<Top10Score>();

        public GameModeScores_EachDiffLevel()
        {
            foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
            {
                scoresEachDiffLevel.Add(new Top10Score());
            }
        }

        /// <summary>
        /// Dictionary&lt;DifficultyLevel, int[]&gt;型で受け取り、List&lt;Top10Score&gt;型にして保存。
        /// 最終的にはjson形式で保存される。
        /// </summary>
        /// <param name="newScores">保存したい難易度ごとのトップ10スコア</param>
        public void SetScoresEachLevel(Dictionary<DifficultyLevel, int[]> newScores)
        {
            foreach (var scorePair in newScores)
            {
                scoresEachDiffLevel[(int)scorePair.Key].SetTop10Score(scorePair.Value);
            }
        }

        /// <summary>
        /// 内部でList&lt;Top10Score&gt;型であつかっているものをDictionary&lt;DifficultyLevel, int[]&gt;型に変換して返す
        /// </summary>
        public Dictionary<DifficultyLevel, int[]> GetScoresEachLevel()
        {
            var result = new Dictionary<DifficultyLevel, int[]>();
            foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
            {
                result[level] = scoresEachDiffLevel[(int)level].GetTop10Score();
            }
            return result;
        }
    }

    //[Serializable]
    //class AllScoreData
    //{
    //    //シリアライズ可能なクラスのリストを使用 ※他次元配列や辞書はシリアライズできない
    //    [SerializeField] private List<ScoreData_EachDiffLevel> scoresEachGameMode = new List<ScoreData_EachDiffLevel>();

    //    public AllScoreData()
    //    {
    //        foreach (GameMode level in Enum.GetValues(typeof(GameMode)))
    //        {
    //            scoresEachGameMode.Add(new ScoreData_EachDiffLevel());
    //        }
    //    }

    //    public void SetAllScores(Dictionary<GameMode, Dictionary<DifficultyLevel, int[]>> scoresEachMode)
    //    {
    //        foreach (var mode__EachLevelScores in scoresEachMode)
    //        {
    //            scoresEachGameMode[(int)mode__EachLevelScores.Key].SetEachLevelScores(mode__EachLevelScores.Value);
    //        }
    //    }

    //    public Dictionary<GameMode, Dictionary<DifficultyLevel, int[]>> GetAllScore()
    //    {
    //        var result = new Dictionary<GameMode, Dictionary<DifficultyLevel, int[]>>();
    //        foreach(GameMode gameMode in Enum.GetValues(typeof(GameMode)))
    //        {
    //            Dictionary<DifficultyLevel, int[]> eachLevelScores = scoresEachGameMode[(int)gameMode].GetEachLevelScores();
    //            foreach (DifficultyLevel level in Enum.GetValues(typeof(DifficultyLevel)))
    //            {
    //                result[gameMode][level] = eachLevelScores[level];
    //            }
    //        }
    //        return result;
    //    }

    //}
}
