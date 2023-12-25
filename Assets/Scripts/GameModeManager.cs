using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameModeManager;

public class GameModeManager : MonoBehaviour
{
    //�V���O���g���̃C���X�^���X
    private static GameModeManager instance;

    public static GameModeManager GameModemanagerInstance => instance;



    //��Փx��\���񋓌^�̒�`
    public enum DifficultyLevel
    {
        Normal,
        Difficult,
        Insane
    }
    public enum GameMode
    {
        PileUp, //�ςݏグ���[�h
    }

    public DifficultyLevel myDifficultyLevel = DifficultyLevel.Difficult; //��Փx�^�̕ϐ����`�A�Ƃ肠����Normal�ŏ����� �K�؂ȃ^�C�~���O�œ�Փx�������ł���悤�ɐ؂�ւ���K�v������B
    public DifficultyLevel MyDifficultyLevel => myDifficultyLevel;
    GameMode nowGameMode = GameMode.PileUp; //�����l�͐ςݏグ���[�h
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
            instance = this; //�P���static�C���X�^���X�̐����B
            DontDestroyOnLoad(this.gameObject); //�V�[���̐؂�ւ����ɔj������Ȃ��悤�ɂ���
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
        Debug.Log($"���ݕύX���ꂽ��Փx : {newDifficultyLevel}");
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
        var obj = JsonUtility.FromJson<JsonLoadGameModeManager>(datastr); //Monobehavior���p�������N���X�ł�Json�t�@�C����ǂݍ��ނ��Ƃ��ł��Ȃ����߁A���̃N���X�𐶐����ǂݍ���
        instance.myDifficultyLevel = obj.myDifficultyLevel;
    }
}

//Json����C���X�^���X�𐶐����邽�߂̃N���X
class JsonLoadGameModeManager
{
    public DifficultyLevel myDifficultyLevel;
}
