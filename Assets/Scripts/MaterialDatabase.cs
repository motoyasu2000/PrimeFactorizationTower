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
}

//ブロックごとのMaterial情報をjson形式でセーブ・ロードを行うためのクラス
[System.Serializable]
public class BlockMaterialData
{
    public int blockNumber; //データベースでいう主キー
    public string materialPath;
    public List<ParameterData> parameters;
}

//マテリアルのシェーダーの各パラメーターの情報をセーブ・ロードするためのクラス
[System.Serializable]
public class ParameterData
{
    public int parameterEnumIndex;
    public int type; //パラメーターの型を表す（0: float, 1: Color,）
    public float floatValue;
    public Color colorValue;
}