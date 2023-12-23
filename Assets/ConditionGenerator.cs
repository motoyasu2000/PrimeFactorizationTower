using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionGenerator : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] ConditionNumberManager conditionNumberManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public Dictionary<int,int> GenerateCondition()
    {
        Dictionary<int,int> returnDict = new Dictionary<int,int>();

        int resultCompositNumber = 1;
        int randomIndex;
        int randomPrimeNumber;

        int rand = Random.Range(3, 4);

        switch (GameModeManager.GameModemanagerInstance.MyDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                for (int i = 0; i < rand; i++)
                {
                    randomIndex = Random.Range(0, GameModeManager.GameModemanagerInstance.NormalPool.Count);
                    randomPrimeNumber = GameModeManager.GameModemanagerInstance.NormalPool[randomIndex];
                    resultCompositNumber *= randomPrimeNumber;
                    if (!returnDict.TryAdd(randomPrimeNumber, 1)) returnDict[randomPrimeNumber] += 1;
                }
                break;

            case GameModeManager.DifficultyLevel.difficult:
                for (int i = 0; i < rand; i++)
                {
                    randomIndex = Random.Range(0, GameModeManager.GameModemanagerInstance.DifficultPool.Count);
                    randomPrimeNumber = GameModeManager.GameModemanagerInstance.DifficultPool[randomIndex];
                    resultCompositNumber *= randomPrimeNumber;
                    if (!returnDict.TryAdd(randomPrimeNumber, 1)) returnDict[randomPrimeNumber] += 1;
                }
                break;
        }
        conditionNumberManager.PrintConditionNumber(resultCompositNumber.ToString());
        Debug.Log("Keys : " + string.Join(",", returnDict.Keys));
        Debug.Log("Values : " + string.Join(",", returnDict.Values));
        return returnDict;
    }
}
