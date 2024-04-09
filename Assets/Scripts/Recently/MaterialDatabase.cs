using Common;
using MaterialLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

//全てのブロックの情報を保存するためのクラス
[System.Serializable]
public class MaterialDatabase
{
    public List<BlockMaterialData> blockMaterials = new List<BlockMaterialData>();

    public MaterialDatabase() { }
    public MaterialDatabase(MaterialDatabase materialDatabase)
    {
        this.blockMaterials = new List<BlockMaterialData>();
        foreach(var blockMaterial in materialDatabase.blockMaterials)
        {
            blockMaterials.Add(new BlockMaterialData(blockMaterial));
        }
    }
    //ブロックマテリアルを追加するためのメソッド
    public void AddBlockMaterial(BlockMaterialData newBlockMaterial)
    {
        //重複している要素があれば削除
        blockMaterials.RemoveAll(b => b.blockNumber == newBlockMaterial.blockNumber);

        //要素の追加
        blockMaterials.Add(newBlockMaterial);
    }

    public BlockMaterialData GetBlockMaterialData(int blockNum)
    {
        return blockMaterials.Find(b => b.blockNumber == blockNum);
    }

    public void PrintMaterialDatabase()
    {
        foreach (var blockMaterial in blockMaterials)
        {
            foreach(var parameter in blockMaterial.parameters)
            {
                Debug.Log($"MDB上の 素数: {blockMaterial.blockNumber}, パラメーター{Enum.GetValues(EnumParameterBinderManager.Binders[blockMaterial.binderIndex].EnumType).GetValue(parameter.parameterEnumIndex)}, float: {parameter.floatValue}, Color: ({parameter.redValue}, {parameter.greenValue}, {parameter.blueValue})");
            }
        }
    }
}

//ブロックごとのMaterial情報をjson形式でセーブ・ロードを行うためのクラス
[System.Serializable]
public class BlockMaterialData
{
    public int blockNumber; //データベースでいう主キー
    public int binderIndex;
    public string materialPath;
    public List<ParameterData> parameters = new List<ParameterData>();

    public BlockMaterialData() { }
    public BlockMaterialData(BlockMaterialData other)
    {
        this.blockNumber = other.blockNumber;
        this.binderIndex = other.binderIndex;
        this.materialPath = other.materialPath;
        this.parameters = new List<ParameterData>();
        foreach(var parameter in other.parameters)
        {
            this.parameters.Add(new ParameterData(parameter));
        }
    }

    //パラメーター情報を追加するためのメソッド
    public void AddParameter(ParameterData newParameterData)
    {
        //重複している設定は消去
        parameters.RemoveAll(p => p.parameterEnumIndex == newParameterData.parameterEnumIndex);

        //新しいパラメーターを追加
        parameters.Add(newParameterData);
    }

    public ParameterData GetParameter(int parameterEnumIndex)
    {
        return parameters.Find(p => p.parameterEnumIndex == parameterEnumIndex);
    }
}

//マテリアルのシェーダーの各パラメーターの情報をセーブ・ロードするためのクラス
[System.Serializable]
public class ParameterData
{
    public int parameterEnumIndex;
    public int type; //パラメーターの型を表す（0: float, 1: Color,）
    public float floatValue;

    //色のパラメーター
    public float redValue;
    public float greenValue;
    public float blueValue;

    public ParameterData() { }
    public ParameterData(ParameterData other)
    {
        this.parameterEnumIndex = other.parameterEnumIndex;
        this.type = other.type;
        this.floatValue = other.floatValue;
        this.redValue = other.redValue;
        this.greenValue = other.greenValue;
        this.blueValue = other.blueValue;
    }
}