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
        //パラメーターを保存するためため、ブロックデータの取得処理
        BlockMaterialData blockData;
        //データベース上に既にブロックの情報があれば、そこから取得
        if (tmpMaterialDatabase.blockMaterials.Any(b =>  b.blockNumber == blockNum))
        {
            blockData = tmpMaterialDatabase.GetBlockMaterialData(blockNum);
        }
        //無ければ生成
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
        InitializeMaterialDatabase<DefaultBlocksMaterialProperty>(new DefaultMaterialEnumBinder());
    }

    //パラメーターの文字列に対応するParameterDataを返す
    ParameterData GenerateParameterData<TEnum>(string parameterName) where TEnum:Enum
    {
        ParameterData parameterData = new ParameterData();
        parameterData.parameterEnumIndex = EnumManager.GetEnumIndexFromString<TEnum>(parameterName);
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

    //materialDatabaseのマテリアルごとの初期化
    public void InitializeMaterialDatabase<TEnum>(IEnumParametersBinder ibinder) where TEnum : Enum
    {
        foreach(var prime in GameModeManager.Ins.PrimeNumberPool)
        {
            BlockMaterialData materialData = new BlockMaterialData() {blockNumber = prime, binderIndex=EnumParameterBinderManager.GetBindersIndex(ibinder)};
            for(int i=0; i<Enum.GetValues(ibinder.EnumType).Length; i++)
            {
                string parameterName = EnumManager.GetStringFromIndex<TEnum>(i);
                materialData.AddParameter(GenerateParameterData<TEnum>(parameterName));
                //Debug.Log($"MDB上の 素数: {prime}, パラメーター{parameterName}");
            }
            tmpMaterialDatabase.AddBlockMaterial(materialData);
        }
    }
}
