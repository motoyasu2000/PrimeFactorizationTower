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
        //キーが素数、バリューがその素数の数の辞書の生成
        Dictionary<int,int> returnDict = new Dictionary<int,int>();

        int resultCompositNumber = 1;
        int randomIndex;
        int randomPrimeNumber;

        int rand;

        switch (GameModeManager.GameModemanagerInstance.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:

                rand = Random.Range(3, 5);
                for (int i = 0; i < rand; i++)
                {
                    randomIndex = Random.Range(0, GameModeManager.GameModemanagerInstance.NormalPool.Count);
                    randomPrimeNumber = GameModeManager.GameModemanagerInstance.NormalPool[randomIndex];
                    resultCompositNumber *= randomPrimeNumber;
                    if (!returnDict.TryAdd(randomPrimeNumber, 1)) returnDict[randomPrimeNumber] += 1;
                }
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                rand = Random.Range(2, 4);
                for (int i = 0; i < rand; i++)
                {
                    randomIndex = Random.Range(0, GameModeManager.GameModemanagerInstance.DifficultPool.Count);
                    randomPrimeNumber = GameModeManager.GameModemanagerInstance.DifficultPool[randomIndex];
                    resultCompositNumber *= randomPrimeNumber;
                    if (!returnDict.TryAdd(randomPrimeNumber, 1)) returnDict[randomPrimeNumber] += 1;
                }
                break;

            case GameModeManager.DifficultyLevel.Insane:
                rand = 2;
                for (int i = 0; i < rand; i++)
                {
                    randomIndex = Random.Range(0, GameModeManager.GameModemanagerInstance.InsanePool.Count);
                    randomPrimeNumber = GameModeManager.GameModemanagerInstance.InsanePool[randomIndex];
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
