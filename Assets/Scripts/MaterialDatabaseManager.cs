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
    static readonly float initColorValue = 0f;
    static readonly float initFloatValue = 10f;

    //jsonに保存するまえのマテリアル情報を保存するdatabase
    MaterialDatabase tmpMaterialDatabase = new MaterialDatabase();
    public MaterialDatabase TmpMaterialDatabase => tmpMaterialDatabase;

    private void Start()
    {
        LoadMaterialDatabase();
    }

    //引数で受け取った情報に合わせて、materialDatabaseの更新処理
    public void SetShaderParameter(int blockNum, string materialPath, IEnumParametersBinder ibinder ,ParameterData parameter)
    {
        BlockMaterialData blockData;
        if (tmpMaterialDatabase.blockMaterials.Any(b =>  b.blockNumber == blockNum))
        {
            blockData = tmpMaterialDatabase.GetBlockMaterialData(blockNum);
        }
        else
        {
            Debug.LogError("指定されたインデックスが見つかりませんでした。");
            blockData = new BlockMaterialData() { blockNumber = blockNum };
        }
        blockData.materialPath = materialPath;
        blockData.binderIndex = EnumParameterBinderManager.GetBindersIndex(ibinder);
        blockData.AddParameter(parameter);
        tmpMaterialDatabase.AddBlockMaterial(blockData);
    }

    void LoadMaterialDatabase()
    {
        //実際にはjsonからよみとるようなしくみにする
        InitializeMaterialDatabase();
    }

    //materialDatabaseの初期化　初回起動時に呼ばれる
    void InitializeMaterialDatabase()
    {
        foreach(var prime in GameModeManager.Ins.PrimeNumberPool)
        {
            DefaultMaterialEnumBinder defaultBinder = new DefaultMaterialEnumBinder();
            BlockMaterialData materialData = new BlockMaterialData() {blockNumber = prime, binderIndex=0};
            for(int i=0; i<Enum.GetValues(defaultBinder.EnumType).Length; i++)
            {
                string parameterName = EnumManager.GetStringFromIndex<DefaultBlocksMaterialProperty>(i);
                materialData.AddParameter(GenerateParameterData(parameterName));
            }
            tmpMaterialDatabase.AddBlockMaterial(materialData);
        }
    }

    //MaterialDatabase上で、引数で指定したブロックに引数に指定したバインダーを割り当てる マテリアルのボタンのタップ時に呼び出され、そのマテリアルで初期化されるようにする。
    public void SetBinderToBlock<TEnum>(IEnumParametersBinder ibinder, int blockNumber) where TEnum : Enum
    {
        BlockMaterialData materialData = new BlockMaterialData() { blockNumber = blockNumber , binderIndex=EnumParameterBinderManager.GetBindersIndex(ibinder)};
        for (int i=0; i<Enum.GetValues(ibinder.EnumType).Length; i++)
        {
            string parameterName = EnumManager.GetStringFromIndex<TEnum>(i);
            materialData.AddParameter(GenerateParameterData(parameterName));
        }
    }

    //パラメーターの文字列に対応するParameterDataを返す
    ParameterData GenerateParameterData(string parameterName)
    {
        ParameterData parameterData = new ParameterData();
        if (parameterName.Contains("Color") || parameterName.Contains("color"))
        {
            parameterData.type = 1;
            parameterData.redValue = initColorValue;
            parameterData.greenValue = initColorValue;
            parameterData.blueValue = initColorValue;
        }
        //color以外(今のところfloatのみ)は全て1つだけ生成
        else
        {
            parameterData.type = 0;
            parameterData.floatValue = initFloatValue;
        }
        return parameterData;
    }
}
