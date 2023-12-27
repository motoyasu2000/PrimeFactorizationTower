using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] GameObject blockField;
    GameObject afterField;
    GameObject completedField;
    TextMeshProUGUI maxScore;
    float maxHeight = 0;
    public int[] pileUpScores = new int[11];

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
        SceneManager.sceneLoaded += InitializeFields;
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

    void InitializeFields(Scene scene, LoadSceneMode mode)
    {
        instance.blockField = GameObject.Find("BlockField");
        instance.afterField = blockField.transform.Find("AfterField").gameObject;
        instance.completedField = blockField.transform.Find("CompletedField").gameObject;
        instance.maxScore = GameObject.Find("MaxScore").GetComponent<TextMeshProUGUI>();
        instance.maxHeight = 0;
        instance.maxScore.text = instance.pileUpScores[0].ToString();
    }

    public void SaveScoreData()
    {
        ScoreManager dScoreManagerInstance = instance;
        string jsonstr = JsonUtility.ToJson(dScoreManagerInstance);
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Savedata/Score/PileUp.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public void LoadScoreData()
    {
        if (!File.Exists(Application.dataPath + "/Savedata/Score/PileUp.json")) { return; }
        StreamReader reader = new StreamReader(Application.dataPath + "/Savedata/Score/PileUp.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<JsonLoadSoundManager>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.pileUpScores = obj.pileUpScores;
    }

    class JsonLoadSoundManager
    {
        public int[] pileUpScores;
    }
}
