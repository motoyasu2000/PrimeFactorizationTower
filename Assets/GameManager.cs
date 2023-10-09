using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //��Փx��\���񋓌^�̒�`
    public enum DifficultyLevel
    {
        Normal,
        difficult,
        Insane
    }

    DifficultyLevel myDifficultyLevel = DifficultyLevel.Normal; //��Փx�^�̕ϐ����`�A�Ƃ肠����Normal�ŏ����� �K�؂ȃ^�C�~���O�œ�Փx�������ł���悤�ɐ؂�ւ���K�v������B

    int[] primeNumberPool = new int[9]
    {
        2,3,5,7,11,13,17,19,23
    };

    List<int> normalPool = new List<int>();
    List<int> difficultPool = new List<int>();
    List<int> insanePool = new List<int>();

    [SerializeField] TextMeshProUGUI text; //��ʏ㕔�̍������̃e�L�X�g

    int nowPhase = 1; //���݂�phase

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
        if (string.IsNullOrWhiteSpace(text.text))//�����񂪋�ł����
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
