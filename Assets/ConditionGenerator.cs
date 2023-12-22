using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ConditionGenerator : MonoBehaviour
{
    GameManager gameManager;
    DifficultyLevel MyDifficultyLevel => gameManager.MyDifficultyLevel;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<int,int> GenerateCondition()
    {
        Dictionary<int,int> returnDict = new Dictionary<int,int>();
        int randomIndex;
        int randomPrimeNumber;

        int rand = Random.Range(3, 4);

        if (MyDifficultyLevel == DifficultyLevel.Normal)
        {
            for (int i = 0; i<rand; i++)
            {
                randomIndex = Random.Range(0, gameManager.NormalPool.Count);
                randomPrimeNumber = gameManager.NormalPool[randomIndex];
                if (!returnDict.TryAdd(randomPrimeNumber,1)) returnDict[randomPrimeNumber] += 1;
            }
        }
        Debug.Log("Keys : " + string.Join(",", returnDict.Keys));
        Debug.Log("Values : " + string.Join(",", returnDict.Values));
        return returnDict;
    }
}
