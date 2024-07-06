using MaterialLibrary;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全てのブロックのに割り当てるマテリアルや、そのパラメーターの情報を保存するためのクラス
/// </summary>
[System.Serializable]
public class MaterialDatabase
{
    public List<BlockMaterialData> blockMaterials = new List<BlockMaterialData>();
    public MaterialDatabase() { }
    public MaterialDatabase(MaterialDatabase materialDatabase)
    {
        blockMaterials = new List<BlockMaterialData>();
        foreach(var blockMaterial in materialDatabase.blockMaterials)
        {
            blockMaterials.Add(new BlockMaterialData(blockMaterial));
        }
    }
    //BlockMaterialDataを追加する
    public void AddBlockMaterial(BlockMaterialData newBlockMaterial)
    {
        //重複している要素があれば削除
        blockMaterials.RemoveAll(b => b.blockNumber == newBlockMaterial.blockNumber);

        //要素の追加
        blockMaterials.Add(newBlockMaterial);
    }

    //BlockMaterialDataをブロックの番号から取得する
    public BlockMaterialData GetBlockMaterialData(int blockNum)
    {
        BlockMaterialData resultData = blockMaterials.Find(b => b.blockNumber == blockNum);
        if (resultData == null) return null;
        return new BlockMaterialData(resultData);
    }

    //中身の表示
    public void PrintMaterialDatabase()
    {
        foreach (var blockMaterial in blockMaterials)
        {
            foreach(var parameter in blockMaterial.parameters)
            {
                Debug.Log($"MDB上の 素数: {blockMaterial.blockNumber}, パラメーター{Enum.GetValues(BinderManager.Binders[blockMaterial.binderIndex].EnumType).GetValue(parameter.parameterEnumIndex)}, float: {parameter.floatValue}, Color: ({parameter.redValue}, {parameter.greenValue}, {parameter.blueValue})");
            }
        }
    }
}

/// <summary>
/// ブロックごとのMaterial情報をjson形式でセーブ・ロードを行うためのクラス
/// </summary>
[System.Serializable]
public class BlockMaterialData
{
    public int blockNumber; //データベースでいう主キー
    public int binderIndex;
    public List<ParameterData> parameters = new List<ParameterData>();

    public BlockMaterialData() { }
    public BlockMaterialData(BlockMaterialData other)
    {
        this.blockNumber = other.blockNumber;
        this.binderIndex = other.binderIndex;
        this.parameters = new List<ParameterData>();
        foreach(var parameter in other.parameters)
        {
            this.parameters.Add(new ParameterData(parameter));
        }
    }

    //ParameterData情報の追加
    public void AddParameter(ParameterData newParameterData)
    {
        //重複している設定は消去
        parameters.RemoveAll(p => p.parameterEnumIndex == newParameterData.parameterEnumIndex);

        //新しいパラメーターを追加
        parameters.Add(newParameterData);
    }

    //ParameterDataをparameterEnumIndexを使って取得
    public ParameterData GetParameter(int parameterEnumIndex)
    {
        return new ParameterData(parameters.Find(p => p.parameterEnumIndex == parameterEnumIndex));
    }

    /// <summary>
    /// 2つのブロックのマテリアルが内部のパラメーターの値含めてすべて一致しているかをチェックする
    /// </summary>
    /// <param name="compareData">比較対象</param>
    /// <returns>2つのブロックのマテリアルが完全に一致した(true)か否(false)か</returns>
    public bool Equal(BlockMaterialData compareData)
    {
        int paramCount = parameters.Count;
        int compareParamCount = compareData.parameters.Count;
        if (paramCount != compareParamCount) return false;
        for (int i=0; i<paramCount; i++)
        {
            if (!parameters[i].Equal(compareData.parameters[i])) return false;
        }
        return true;
    }
}

/// <summary>
/// マテリアルのシェーダーの各パラメーターの情報をセーブ・ロードするためのクラス
/// </summary>
[System.Serializable]
public class ParameterData
{
    public enum PropertyType
    {
        Float = 0,
        Color = 1,
        Invalid = -1
    }

    public int parameterEnumIndex;//BlocksMaterialPropertyの値
    public PropertyType type;

    //数値のパラメーター
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

    /// <summary>
    /// ParameterDataの要素が完全に一致しているかの比較
    /// </summary>
    /// <param name="compareData">比較対象のParameterData</param>
    /// <returns>要素がすべて一致した(true)か否(false)か</returns>
    public bool Equal(ParameterData compareData)
    {
        if(parameterEnumIndex != compareData.parameterEnumIndex) return false;
        if(type != compareData.type) return false;
        if(floatValue != compareData.floatValue) return false;
        if(redValue != compareData.redValue) return false;
        if(greenValue != compareData.greenValue) return false;
        if(blueValue != compareData.blueValue) return false;
        return true;
    }
}