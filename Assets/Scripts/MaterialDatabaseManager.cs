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
            Debug.LogError("指定されたインデックスが見つかりませんでした。");
            blockData = new BlockMaterialData() { blockNumber = blockNum };
        }
        blockData.materialPath = materialPath;
        blockData.AddParameter(parameter);
        materialDatabase.AddBlockMaterial(blockData);
    }

    void LoadMaterialDatabase()
    {
        //実際にはjsonからよみとるようなしくみにする
        materialDatabase = new MaterialDatabase();
    }
}
