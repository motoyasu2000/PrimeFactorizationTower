using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGenerator : MonoBehaviour
{
    BlockGenerator[] allBlockGenerators = null;
    void Awake()
    {
        allBlockGenerators = transform.GetComponentsInChildren<BlockGenerator>();
        for (int i = 0; i < allBlockGenerators.Length; i++)
        {
            allBlockGenerators[i].SetPrimeNumber(GameModeManager.GameModemanagerInstance.PrimeNumberPool[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
