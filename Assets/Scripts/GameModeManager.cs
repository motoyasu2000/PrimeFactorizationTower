using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    //��Փx��\���񋓌^�̒�`
    public enum DifficultyLevel
    {
        Normal,
        difficult,
        Insane
    }
    public enum GameMode
    {
        PileUp, //�ςݏグ���[�h
    }

    DifficultyLevel myDifficultyLevel = DifficultyLevel.Normal; //��Փx�^�̕ϐ����`�A�Ƃ肠����Normal�ŏ����� �K�؂ȃ^�C�~���O�œ�Փx�������ł���悤�ɐ؂�ւ���K�v������B
    public DifficultyLevel MyDifficultyLevel => myDifficultyLevel;
    GameMode nowGameMode;
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
        SetGameMode(GameMode.PileUp); //��U���s���ɐςݏグ���[�h�ɂ��Ă����B
    }

    public void SetGameMode(GameMode newGameMode)
    {
        nowGameMode = newGameMode;
    }

    void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        myDifficultyLevel = newDifficultyLevel;
    }
}
