using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionGenerator : MonoBehaviour
{
    GameModeManager gameModeManager;
    [SerializeField] ConditionNumberManager conditionNumberManager;

    void Start()
    {
        gameModeManager = GameModeManager.GameModemanagerInstance;
    }

    //�����𐶐����郁�\�b�h(��Փx����)
    public Dictionary<int,int> GenerateCondition()
    {
        //�L�[���f���A�o�����[�����̑f���̐��̎����̐���(��Փx����)
        Dictionary<int,int> returnDict = new Dictionary<int,int>();
        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                returnDict = GenerateConditionForDifficultyLevel(gameModeManager.NormalPool,3,5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                returnDict = GenerateConditionForDifficultyLevel(gameModeManager.DifficultPool, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                returnDict = GenerateConditionForDifficultyLevel(gameModeManager.NormalPool, 2, 4);
                break;
        }
        
        //�������̌v�Z�ƕ\��
        int compositeNumber = DictToCompositeNumber(returnDict);
        conditionNumberManager.PrintConditionNumber(compositeNumber.ToString());

        //�f�o�b�O
        Debug.Log("Keys : " + string.Join(",", returnDict.Keys));
        Debug.Log("Values : " + string.Join(",", returnDict.Values));

        return returnDict;
    }

    //�f���v�[���A�����̍ŏ��l�ő�l���w�肵�āA�L�[���f���A�o�����[�����̑f���̐��̎�������郁�\�b�h
    Dictionary<int, int> GenerateConditionForDifficultyLevel(List<int> primePool, int minRand, int maxRand)
    {
        Dictionary<int, int> returnDict = new Dictionary<int, int>();
        int randomIndex;
        int randomPrimeNumber;
        int rand = Random.Range(minRand, maxRand);

        for (int i = 0; i < rand; i++)
        {
            randomIndex = Random.Range(0, primePool.Count);
            randomPrimeNumber = primePool[randomIndex];
            if (!returnDict.TryAdd(randomPrimeNumber, 1)) returnDict[randomPrimeNumber] += 1;
        }
        return returnDict;
    }

    //�L�[���f���A�o�����[�����̑f���̐��̎������獇�������v�Z���郁�\�b�h
    int DictToCompositeNumber(Dictionary<int, int> returnDict)
    {
        int compositeNumber = 1;
        foreach(var pair in returnDict){
            compositeNumber *= pair.Value*pair.Key;
        }
        return compositeNumber;
    }
}
