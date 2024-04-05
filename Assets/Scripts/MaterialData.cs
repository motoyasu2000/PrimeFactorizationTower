using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Material情報をjson形式でセーブ・ロードを行うためのクラス
[System.Serializable]
public class MaterialData
{
    public string materialPath;
    public List<ParameterData> parameters;
}

//マテリアルのシェーダーの各パラメーターの情報をセーブ・ロードするためのクラス
[System.Serializable]
public class ParameterData
{
    public int parameterEnumIndex;
    public float floatValue;
    public Color colorValue;
}