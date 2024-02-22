using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGenerator : MonoBehaviour
{
    BlockGenerator[] allBlockGenerators = null; //�S�Ẵ{�^����BlockGenerators�C���X�^���X������z��
    void Awake()
    {
        allBlockGenerators = transform.GetComponentsInChildren<BlockGenerator>();
        //���ׂĂ�BlockGenerators�C���X�^���X�ɑ΂��āA�f���v�[������f����ݒ�B
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
