using Common;
using MaterialLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Rendering;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MaterialDatabaseManager : MonoBehaviour
{
    static readonly float initColorValue = 0.8f;
    static readonly float initFloatValue = 10f;
    MaterialDatabase materialDatabase;
    public MaterialDatabase MaterialDatabase => materialDatabase;

    private void Start()
    {
        LoadMaterialDatabase();
    }

    //引数で受け取った情報に合わせて、materialDatabaseの更新処理
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

    //materialDatabaseの初期化　初回起動時に呼ばれる
    void InitializeMaterialDatabase()
    {
        foreach(var prime in GameModeManager.Ins.PrimeNumberPool)
        {
            DefaultMaterialEnumBinder defaultBinder = new DefaultMaterialEnumBinder();
            string parameterName = defaultBinder.MaterialPathAndName;
            BlockMaterialData materialData = new BlockMaterialData() {blockNumber = prime};
            for(int i=0; i<Enum.GetValues(defaultBinder.EnumType).Length; i++)
            {
                materialData.AddParameter(GenerateParameterData(parameterName));
            }
            materialDatabase.AddBlockMaterial(materialData);
        }
    }

    //MaterialDatabase上で、引数で指定したブロックに引数に指定したバインダーを割り当てる マテリアルのボタンのタップ時に呼び出され、そのマテリアルで初期化されるようにする。
    public void SetBinderToBlock(IEnumParametersBinder ibinder, int blockNumber)
    {
        string parameterName = ibinder.MaterialPathAndName;
        BlockMaterialData materialData = new BlockMaterialData() { blockNumber = blockNumber };
        for (int i=0; i<Enum.GetValues(ibinder.EnumType).Length; i++)
        {
            materialData.AddParameter(GenerateParameterData(parameterName));
        }
    }

    //パラメーターの文字列に対応するParameterDataを返す
    ParameterData GenerateParameterData(string parameterName)
    {
        ParameterData parameterData = new ParameterData();
        if (parameterName.Contains("Color") || parameterName.Contains("color"))
        {
            parameterData.redValue = initColorValue;
            parameterData.greenValue = initColorValue;
            parameterData.blueValue = initColorValue;
        }
        //color以外(今のところfloatのみ)は全て1つだけ生成
        else
        {
            parameterData.floatValue = initFloatValue;
        }
        return parameterData;
    }
}
