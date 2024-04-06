using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialDatabaseManager : MonoBehaviour
{
    MaterialDatabase materialDatabase;
    public MaterialDatabase MaterialDatabase => materialDatabase;

    private void Start()
    {
        LoadMaterialDatabase();
    }

    public void SetShaderParameter(int blockNum, string materialPath, ParameterData parameter)
    {
        BlockMaterialData blockData;
        if (materialDatabase.blockMaterials.Any(b =>  b.blockNumber == blockNum))
        {
            blockData = materialDatabase.GetBlockMaterialData(blockNum);
        }
        else
        {
            Debug.LogError("�w�肳�ꂽ�C���f�b�N�X��������܂���ł����B");
            blockData = new BlockMaterialData() { blockNumber = blockNum };
        }
        blockData.materialPath = materialPath;
        blockData.AddParameter(parameter);
        materialDatabase.AddBlockMaterial(blockData);
    }

    void LoadMaterialDatabase()
    {
        //���ۂɂ�json�����݂Ƃ�悤�Ȃ����݂ɂ���
        materialDatabase = new MaterialDatabase();
    }
}
