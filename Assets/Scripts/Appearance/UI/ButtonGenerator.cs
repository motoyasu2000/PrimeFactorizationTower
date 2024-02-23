using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ButtonGenerator : MonoBehaviour
    {
        BlockGenerator[] allBlockGenerators = null; //全てのボタンのBlockGeneratorsインスタンスが入る配列
        void Awake()
        {
            allBlockGenerators = transform.GetComponentsInChildren<BlockGenerator>();
            //すべてのBlockGeneratorインスタンスに対して、素数プールから素数を設定。
            for (int i = 0; i < allBlockGenerators.Length; i++)
            {
                allBlockGenerators[i].SetPrimeNumber(GameModeManager.GameModemanagerInstance.PrimeNumberPool[i]);
            }
        }
    }
}
