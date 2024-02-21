using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//�X�R�A�Ǘ���S������N���X�B
//�Q�[�����̃X�R�A�v�Z�A�ۑ��E�ǂݍ��݁A�V�[���Ԃł̃X�R�A�f�[�^�̕ێ����s���B

public class ScoreManager : MonoBehaviour
{
    const float groundHeight = 0.5f; //���̒n�ʂ̍���
    float nowScore = 0; //���ݐi�s���̃Q�[���̃X�R�A������ϐ�
    public float NowScore => nowScore;
    GameObject blockField;
    GameObject afterField;
    GameObject completedField;
    TextMeshProUGUI maxScore;
    Dictionary<GameModeManager.DifficultyLevel, int[]> pileUpScores = new Dictionary<GameModeManager.DifficultyLevel,int[]>();
    public Dictionary<GameModeManager.DifficultyLevel, int[]> PileUpScores => pileUpScores;
    private static ScoreManager instance;
    public static ScoreManager ScoreManagerInstance => instance;

    //�V���O���g���p�^�[�����g�p���āAScoreManager�̃C���X�^���X���Ǘ��B
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        LoadScoreData();
    }

    //�V�[�����[�h�̃R�[���o�b�N��ݒ肵�A�������������s���B
    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoadProcess;
        InitializeFields();
    }

    //�S�Q�[���I�u�W�F�N�g�̒��_����ł��������_��y���W��Ԃ����\�b�h(GameObject��pivot�ł͂Ȃ��A���_���x���ō������v�Z����B)
    public float CalculateAllGameObjectsMaxHeight()
    {
        List<Vector3> allVertices = new List<Vector3>();
        if (afterField != null)
        {
            foreach (Transform block in afterField.transform)
            {
                nowScore = Mathf.Max(nowScore, CalculateGameObjectMaxHeight(block.gameObject)); //���݌��Ă���Q�[���I�u�W�F�N�g�̍ł��������_��max�̔�r
                                                                                                //Debug.Log(block.gameObject.name);
            }
        }
        if (completedField != null)
        {
            foreach (Transform block in completedField.transform)
            {
                nowScore = Mathf.Max(nowScore, CalculateGameObjectMaxHeight(block.gameObject)); //���݌��Ă���Q�[���I�u�W�F�N�g�̍ł��������_��max�̔�r
                                                                                                //Debug.Log(block.gameObject.name);
            }
        }
        return nowScore;
    }

    //�w�肳�ꂽ�Q�[���I�u�W�F�N�g�̑S���_�̒��ŁA�ł��������_��y���W���v�Z���A�Ԃ�
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
        return max-groundHeight; //���߂̍�����0�ɂ��邽�߂Ɍ��̍���������
    }

    //�V�[���̃��[�h���Ɏ��s����郁�\�b�h
    void SceneLoadProcess(Scene scene, LoadSceneMode mode)
    {
        InitializeFields();
    }

    //�V�[�����[�h���ɌĂ΂�鏉�������\�b�h
    void InitializeFields()
    {
        //�񋓌^DifficultyLevel�Œ�`����Ă���̂ɁA�����̃L�[���ɑ��݂��Ȃ���Փx����������A���̓�Փx�̃L�[��ǉ�����B
        foreach (GameModeManager.DifficultyLevel level in Enum.GetValues(typeof(GameModeManager.DifficultyLevel)))
        {
            if (!instance.pileUpScores.ContainsKey(level))
            {
                instance.pileUpScores[level] = new int[11];
            }
        }

        //�Q�[���V�[���ɂ���Q�[���I�u�W�F�N�g�����g���ĕϐ�������Ă���̂� ����GameScene�ȊO�Ȃ炱�̌�̏������s��Ȃ��B
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("GameScene")) return;
        instance.blockField = GameObject.Find("BlockField");
        instance.afterField = blockField.transform.Find("AfterField").gameObject;
        instance.completedField = blockField.transform.Find("CompletedField").gameObject;
        instance.maxScore = GameObject.Find("MaxScore").GetComponent<TextMeshProUGUI>();
        instance.nowScore = 0;
        //�\������ō��X�R�A�̍X�V
        instance.maxScore.text = instance.pileUpScores[GameModeManager.GameModemanagerInstance.NowDifficultyLevel][0].ToString();
    }

    //�V�����X�R�A���X�R�A���Ǘ����鎫���ɒǉ����A�\�[�g���s���B
    public void InsertPileUpScoreAndSort(int newScore)
    {
        GameModeManager.DifficultyLevel nowLevel = GameModeManager.GameModemanagerInstance.NowDifficultyLevel;
        ScoreManagerInstance.pileUpScores[nowLevel][10] = newScore;
        Array.Sort(ScoreManagerInstance.pileUpScores[nowLevel]);
        Array.Reverse(ScoreManagerInstance.pileUpScores[nowLevel]);
    }

    //���݂̃X�R�A��json�`���ŕۑ�
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

    //json�`���ŕۑ����ꂽ�f�[�^��ǂݍ���
    public static void LoadScoreData()
    {
        if (!File.Exists(Application.persistentDataPath + "/PileUp.json")) { return; }
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/PileUp.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<SerializableScore>(datastr); //Monobehavior���p�������N���X�ł�Json�t�@�C����ǂݍ��ނ��Ƃ��ł��Ȃ����߁A���̃N���X�𐶐����ǂݍ���
        instance.pileUpScores = obj.GetScore();
    }


    
    //////////////////////////////////////////
    //�ȉ��A�V���A���C�Y���s�����߂̈ꎞ�I�ȃN���X//
    //////////////////////////////////////////

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
