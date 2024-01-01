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

        //�����̃Q�[���I�u�W�F�N�g�̓V�[���ڍs���ɔj������Ȃ��̂Ŏ��s����Ȃ��I
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

    //�S�Q�[���I�u�W�F�N�g�̒��_����ł��������_��y���W��Ԃ����\�b�h
    public float CalculateAllVerticesHeight()
    {
        List<Vector3> allVertices = new List<Vector3>();
        if (afterField != null)
        {
            foreach (Transform block in afterField.transform)
            {
                maxHeight = Mathf.Max(maxHeight, CaluculateGameObjectHeight(block.gameObject)); //���݌��Ă���Q�[���I�u�W�F�N�g�̍ł��������_��max�̔�r
                                                                                                //Debug.Log(block.gameObject.name);
            }
        }
        if (completedField != null)
        {
            foreach (Transform block in completedField.transform)
            {
                maxHeight = Mathf.Max(maxHeight, CaluculateGameObjectHeight(block.gameObject)); //���݌��Ă���Q�[���I�u�W�F�N�g�̍ł��������_��max�̔�r
                                                                                                //Debug.Log(block.gameObject.name);
            }
        }
        return maxHeight;
    }

    //�^����ꂽ�Q�[���I�u�W�F�N�g�̑S���_�̓��A�ł��������_��Ԃ����\�b�h
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
        return max-0.5f; //���̍�������500������
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
        var obj = JsonUtility.FromJson<SerializableScore>(datastr); //Monobehavior���p�������N���X�ł�Json�t�@�C����ǂݍ��ނ��Ƃ��ł��Ȃ����߁A���̃N���X�𐶐����ǂݍ���
        instance.pileUpScores = obj.GetScore();
    }

    //�������z��⎫���̓V���A���C�Y�ł��Ȃ��̂ŁA���G�ȍ\���ł��V���A���C�Y���s�����߂ɁA�V���A���C�Y�\�Ȕz����������N���X��p�ӂ��Ă����B
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
        //�V���A���C�Y�\�ȃ��X�g���g�p ���������z��⎫���̓V���A���C�Y�ł��Ȃ�
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
