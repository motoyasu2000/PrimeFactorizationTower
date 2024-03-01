using Common;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//スコア管理を担当するクラス。
//ゲーム中のスコア計算、保存・読み込み、シーン間でのスコアデータの保持を行う。

public class ScoreManager : MonoBehaviour
{
    float nowHeight = 0; //現在のゲームオブジェクトのブロックの高さが入る変数
    GameObject blockField;
    GameObject afterField;
    GameObject completedField;
    TextMeshProUGUI maxScore;

    //各レベルのスコアは要素数11の配列として保存される。0~9番目が過去のランキング、10番目に最新のスコアが入り、ソートされるという仕組み
    [SerializeField] Dictionary<GameModeManager.DifficultyLevel, int[]> pileUpScores = new Dictionary<GameModeManager.DifficultyLevel,int[]>();　//Json形式で保存するために、シリアライズ可能にしておく
    public float NowHeight => nowHeight;
    public Dictionary<GameModeManager.DifficultyLevel, int[]> PileUpScores => pileUpScores;

    //インスタンス
    private static ScoreManager instance;
    public static ScoreManager ScoreManagerInstance => instance;

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
        float height = CalculateAllGameObjectsMaxHeight();
        return (int)(height * 1000);
    }

    //全ゲームオブジェクトの頂点から最も高い頂点のy座標を返すメソッド(GameObjectのpivotではなく、頂点レベルで高さを計算する。)
    float CalculateAllGameObjectsMaxHeight()
    {
        List<Vector3> allVertices = new List<Vector3>();
        if (afterField != null)
        {
            foreach (Transform block in afterField.transform)
            {
                nowHeight = Mathf.Max(nowHeight, CalculateGameObjectMaxHeight(block.gameObject)); //現在見ているゲームオブジェクトの最も高い頂点とmaxの比較                                                                          //Debug.Log(block.gameObject.name);
            }
        }
        if (completedField != null)
        {
            foreach (Transform block in completedField.transform)
            {
                nowHeight = Mathf.Max(nowHeight, CalculateGameObjectMaxHeight(block.gameObject)); //現在見ているゲームオブジェクトの最も高い頂点とmaxの比較
            }
        }
        return nowHeight;
    }

    //指定されたゲームオブジェクトの全頂点の中で、最も高い頂点のy座標を計算し、返す
    float CalculateGameObjectMaxHeight(GameObject block)
    {
        Vector2[] vertices = block.GetComponent<SpriteRenderer>().sprite.vertices;
        float max = 0;
        foreach(Vector2 vartex in vertices)
        {
            Vector2 worldPoint = block.transform.TransformPoint(vartex);
            max = Mathf.Max(max, worldPoint.y);
            //Debug.Log(worldPoint.y);
        }
        return max-Info.groundHeight; //初めの高さを0にするために元の高さ分引く
    }

    //シーンのロード時に実行されるメソッド
    void SceneLoadProcess(Scene scene, LoadSceneMode mode)
    {
        InitializeFields();
    }

    //シーンロード時に呼ばれる初期化メソッド
    void InitializeFields()
    {
        nowHeight = 0;
        //列挙型DifficultyLevelで定義されているのに、辞書のキー内に存在しない難易度があったら、その難易度のキーを追加する。
        foreach (GameModeManager.DifficultyLevel level in Enum.GetValues(typeof(GameModeManager.DifficultyLevel)))
        {
            if (!instance.pileUpScores.ContainsKey(level))
            {
                instance.pileUpScores[level] = new int[Info.rankDisplayLimit + 1];
            }
        }

        //ゲームシーンにあるゲームオブジェクト名を使って変数を作っているので 現在PlayScene以外ならこの後の処理を行わない。
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("PlayScene")) return;
        instance.blockField = GameObject.Find("BlockField");
        instance.afterField = blockField.transform.Find("AfterField").gameObject;
        instance.completedField = blockField.transform.Find("CompletedField").gameObject;
        instance.maxScore = GameObject.Find("MaxScoreText").GetComponent<TextMeshProUGUI>();
        instance.nowHeight = 0;
        //表示する最高スコアの更新
        instance.maxScore.text = instance.pileUpScores[GameModeManager.GameModemanagerInstance.NowDifficultyLevel][0].ToString();
        //Debug.Log(instance.pileUpScores[GameModeManager.GameModemanagerInstance.NowDifficultyLevel][0]);
    }

    //新しいスコアをスコアを管理する辞書に追加し、ソートを行う。
    public void InsertPileUpScoreAndSort(int newScore)
    {
        GameModeManager.DifficultyLevel nowLevel = GameModeManager.GameModemanagerInstance.NowDifficultyLevel;
        ScoreManagerInstance.pileUpScores[nowLevel][Info.rankDisplayLimit] = newScore;
        Array.Sort(ScoreManagerInstance.pileUpScores[nowLevel]);
        Array.Reverse(ScoreManagerInstance.pileUpScores[nowLevel]);
    }

    //現在のスコアをjson形式で保存
    public void SaveScoreData()
    {
        SerializableScore score = new SerializableScore();
        score.SetScore(instance.pileUpScores);
        string jsonstr = JsonUtility.ToJson(score);
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/PileUp.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    //json形式で保存されたデータを読み込む
    public static void LoadScoreData()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) { return; }
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/PileUp.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<SerializableScore>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.pileUpScores = obj.GetScore();
    }


    
    //////////////////////////////////////////
    //以下、シリアライズを行うための一時的なクラス//
    //////////////////////////////////////////

    //他次元配列や辞書はシリアライズできないので、複雑な構造でもシリアライズを行うために、シリアライズ可能な配列を持ったクラスを用意しておく。
    [Serializable]
    public class Top10Score
    {
        public int[] scores;

        public Top10Score()
        {
            scores = new int[Info.rankDisplayLimit + 1];
        }
    }

    [Serializable]
    class SerializableScore
    {
        //シリアライズ可能なリストを使用 ※他次元配列や辞書はシリアライズできない
        [SerializeField] private List<Top10Score> pileUpScores_Serializable = new List<Top10Score>();

        public SerializableScore()
        {
            foreach (GameModeManager.DifficultyLevel level in Enum.GetValues(typeof(GameModeManager.DifficultyLevel)))
            {
                pileUpScores_Serializable.Add(new Top10Score());
            }
        }

        public void SetScore(Dictionary<GameModeManager.DifficultyLevel, int[]> pileUpScores)
        {
            foreach (var scorePair in pileUpScores)
            {
                pileUpScores_Serializable[(int)scorePair.Key].scores = scorePair.Value;
            }
        }

        public Dictionary<GameModeManager.DifficultyLevel, int[]> GetScore()
        {
            var result = new Dictionary<GameModeManager.DifficultyLevel, int[]>();
            foreach (GameModeManager.DifficultyLevel level in Enum.GetValues(typeof(GameModeManager.DifficultyLevel)))
            {
                result[level] = pileUpScores_Serializable[(int)level].scores;
            }
            return result;
        }
    }
}
