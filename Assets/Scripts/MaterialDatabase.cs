using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//全てのブロックの情報を保存するためのクラス
[System.Serializable]
public class MaterialDatabase
{
    public List<BlockMaterialData> blockMaterials = new List<BlockMaterialData>();

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
}

//ブロックごとのMaterial情報をjson形式でセーブ・ロードを行うためのクラス
[System.Serializable]
public class BlockMaterialData
{
    public int blockNumber; //データベースでいう主キー
    public int binderIndex;
    public string materialPath;
    public List<ParameterData> parameters = new List<ParameterData>();

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
}