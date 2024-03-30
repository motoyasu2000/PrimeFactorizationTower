using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActions : MonoBehaviour
{
    BlockSpiner blockSpiner;
    GameObject beforeField;
    TouchBlock touchBlock;
    void Start()
    {
        blockSpiner = GameObject.Find("PrimeNumberGeneratingPoint").GetComponent<BlockSpiner>();
        beforeField = GameObject.Find("BeforeField");
    }

    public void MoveBlockXAndRelease(float x)
    {
        touchBlock = beforeField.GetComponent<TouchBlock>();
        touchBlock.MoveBlockXAndRelease(x);
    }

    public void SpinBlock45SeveralTimes(int numberOfTime)
    {
        for (int i = 0; i < numberOfTime; i++)
        {
            blockSpiner.SpinSingleBlock_45();
        }
    }
}
