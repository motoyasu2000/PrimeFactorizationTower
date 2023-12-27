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

        //�����̃Q�[���I�u�W�F�N�g�̓V�[���ڍs���ɔj������Ȃ��̂Ŏ��s����Ȃ��I

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
        var obj = JsonUtility.FromJson<JsonLoadSoundManager>(datastr); //Monobehavior���p�������N���X�ł�Json�t�@�C����ǂݍ��ނ��Ƃ��ł��Ȃ����߁A���̃N���X�𐶐����ǂݍ���
        instance.pileUpScores = obj.pileUpScores;
    }

    class JsonLoadSoundManager
    {
        public int[] pileUpScores;
    }
}
