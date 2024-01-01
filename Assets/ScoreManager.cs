using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    GameObject blockField;
    GameObject afterField;
    GameObject completedField;
    TextMeshProUGUI maxScore;
    float maxHeight = 0;
    Dictionary<GameModeManager.DifficultyLevel, int[]> pileUpScores = new Dictionary<GameModeManager.DifficultyLevel,int[]>();
    public Dictionary<GameModeManager.DifficultyLevel, int[]> PileUpScores => pileUpScores;

    private static ScoreManager instance;
    public static ScoreManager ScoreManagerInstance => instance;
    public float MaxHeight => maxHeight;
    void Awake()
    {
        //blockField = GameObject.Find("BlockField");
        //afterField = blockField.transform.Find("AfterField").gameObject;
        //completedField = blockField.transform.Find("CompletedField").gameObject;
        //maxHeight = 0;

        //↑このゲームオブジェクトはシーン移行時に破棄されないので実行されない！
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        LoadScoreData();
        
    }
    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoadProcess;
        InitializeFields();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //全ゲームオブジェクトの頂点から最も高い頂点のy座標を返すメソッド
    public float CalculateAllVerticesHeight()
    {
        List<Vector3> allVertices = new List<Vector3>();
        if (afterField != null)
        {
            foreach (Transform block in afterField.transform)
            {
                maxHeight = Mathf.Max(maxHeight, CaluculateGameObjectHeight(block.gameObject)); //現在見ているゲームオブジェクトの最も高い頂点とmaxの比較
                                                                                                //Debug.Log(block.gameObject.name);
            }
        }
        if (completedField != null)
        {
            foreach (Transform block in completedField.transform)
            {
                maxHeight = Mathf.Max(maxHeight, CaluculateGameObjectHeight(block.gameObject)); //現在見ているゲームオブジェクトの最も高い頂点とmaxの比較
                                                                                                //Debug.Log(block.gameObject.name);
            }
        }
        return maxHeight;
    }

    //与えられたゲームオブジェクトの全頂点の内、最も高い頂点を返すメソッド
    float CaluculateGameObjectHeight(GameObject block)
    {
        Vector2[] vertices = block.GetComponent<SpriteRenderer>().sprite.vertices;
        float max = 0;
        foreach(Vector2 vartex in vertices)
        {
            Vector2 worldPoint = block.transform.TransformPoint(vartex);
            max = Mathf.Max(max, worldPoint.y);
            //Debug.Log(worldPoint.y);
        }
        return max-0.5f; //元の高さ分の500を引く
    }

    void SceneLoadProcess(Scene scene, LoadSceneMode mode)
    {
        InitializeFields();
    }

    void InitializeFields()
    {
        foreach (GameModeManager.DifficultyLevel level in Enum.GetValues(typeof(GameModeManager.DifficultyLevel)))
        {
            if (!instance.pileUpScores.ContainsKey(level))
            {
                instance.pileUpScores[level] = new int[11];
            }
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("TitleScene")) return;
        instance.blockField = GameObject.Find("BlockField");
        instance.afterField = blockField.transform.Find("AfterField").gameObject;
        instance.completedField = blockField.transform.Find("CompletedField").gameObject;
        instance.maxScore = GameObject.Find("MaxScore").GetComponent<TextMeshProUGUI>();
        instance.maxHeight = 0;

        instance.maxScore.text = instance.pileUpScores[GameModeManager.GameModemanagerInstance.NowDifficultyLevel][0].ToString();


    }

    public void InsertPileUpScoreAndSort(int newScore)
    {
        GameModeManager.DifficultyLevel nowLevel = GameModeManager.GameModemanagerInstance.NowDifficultyLevel;
        ScoreManagerInstance.pileUpScores[nowLevel][10] = newScore;
        Array.Sort(ScoreManagerInstance.pileUpScores[nowLevel]);
        Array.Reverse(ScoreManagerInstance.pileUpScores[nowLevel]);
    }
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

    public static void LoadScoreData()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) { return; }
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/PileUp.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<SerializableScore>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.pileUpScores = obj.GetScore();
    }

    //他次元配列や辞書はシリアライズできないので、複雑な構造でもシリアライズを行うために、シリアライズ可能な配列を持ったクラスを用意しておく。
    [Serializable]
    public class Top10Score
    {
        public int[] scores;

        public Top10Score()
        {
            scores = new int[11];
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
