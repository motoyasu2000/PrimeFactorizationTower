using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //難易度を表す列挙型の定義
    public enum DifficultyLevel
    {
        Normal,
        difficult,
        Insane
    }

    DifficultyLevel myDifficultyLevel = DifficultyLevel.Normal; //難易度型の変数を定義、とりあえずNormalで初期化 適切なタイミングで難易度調整ができるように切り替える必要がある。

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };

    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();

    [SerializeField] TextMeshProUGUI text; //画面上部の合成数のテキスト

    int nowPhase = 1; //現在のphase

    void Start()
    {
        for(int i=0; i<primeNumberPool.Length; i++)
        {
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 7) normalPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 13) difficultPool.Add(primeNumberPool[i]);
            if (primeNumberPool[i] >= 2 && primeNumberPool[i] <= 23) insanePool.Add(primeNumberPool[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrWhiteSpace(text.text))//文字列が空であれば
        {
            text.text = GenerateNumber().ToString();
        }
    }

    void ChangeDifficultyLevel(DifficultyLevel newDifficultyLevel)
    {
        myDifficultyLevel = newDifficultyLevel;
    }

    int GenerateNumber()
    {
        int randomIndex;
        int randomPrimeNumber;
        int compositeNumber = 1;
        if (myDifficultyLevel == DifficultyLevel.Normal)
        {
            for (int i=0; i<2+(int)(Random.value*nowPhase/2); i++)
            {
                randomIndex = Random.Range(0, normalPool.Count);
                randomPrimeNumber = normalPool[randomIndex];
                compositeNumber *= randomPrimeNumber;
            }
        }
        nowPhase++;
        return compositeNumber;
    }
}
