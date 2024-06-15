using Common;
using MaterialLibrary;
using System;
using System.Reflection;
using UnityEngine;


/// <summary>
/// StartでmaterialDatabaseからマテリアル情報をロードして自身のマテリアルに割り当てるクラス。
/// 全てのブロックにアタッチされている。
/// </summary>
public class BlockMaterialSetting : MonoBehaviour
{
    MaterialDatabase materialDatabase;
    BlockInfo blockInfo;

    void Start()
    {
        //\マテリアルデーターベースをplayerinfoから取得
        blockInfo = GetComponent<BlockInfo>();
        materialDatabase = PlayerInfoManager.Ins.MaterialDatabase;
        if (materialDatabase == null) return;
        if (materialDatabase.blockMaterials == null) return;
        if (materialDatabase.blockMaterials.Count == 0) return;

        //マテリアルの取得と、その設定を保存していたデータから読み取って、適切に反映する。
        IBinder binder = BinderManager.Binders[materialDatabase.GetBlockMaterialData(blockInfo.GetPrimeNumber()).binderIndex];
        Type dynamicEnumType = binder.EnumType;

        //ジェネリックで列挙型を指定するメソッドをリフレクションで取得し、ジェネリックを動的に指定
        MethodInfo setPropertyFloatMethod = typeof(IBinder).GetMethod("SetPropertyFloat");
        MethodInfo setPropertyColorMethod = typeof(IBinder).GetMethod("SetPropertyColor");
        MethodInfo getEnumValueFromIndexMethod = typeof(EnumManager).GetMethod("GetEnumValueFromIndex");
        MethodInfo genericSetPropertyFloatMethod = setPropertyFloatMethod.MakeGenericMethod(dynamicEnumType);
        MethodInfo genericSetPropertyColorMethod = setPropertyColorMethod.MakeGenericMethod(dynamicEnumType);
        MethodInfo genericGetEnumValueFromIndexMethod = getEnumValueFromIndexMethod.MakeGenericMethod(dynamicEnumType);

        //全てのパラメーターをデーターベースから受け取ったものに変更
        foreach (var parameter in materialDatabase.GetBlockMaterialData(blockInfo.GetPrimeNumber()).parameters)
        {
            object enumValue = null;
            //EnumのインデックスからEnumの値を動的に取得
            enumValue = genericGetEnumValueFromIndexMethod.Invoke(null, new object[] { parameter.parameterEnumIndex });

            //読み取ったマテリアルデータベースの情報からbinderのマテリアルの更新。
            if (parameter.type == ParameterData.PropertyType.Float)
            {
                genericSetPropertyFloatMethod.Invoke(binder, new object[] { enumValue, parameter.floatValue });
            }
            else if (parameter.type == ParameterData.PropertyType.Color)
            {
                Color color = new Color(parameter.redValue, parameter.greenValue, parameter.blueValue);
                genericSetPropertyColorMethod.Invoke(binder, new object[] { enumValue, color });
            }
            else
            {
                Debug.LogError($"想定外のtypeが指定されました。: {parameter.type}");
            }
        }
        GetComponent<SpriteRenderer>().material = new Material(binder.Material);
    }
}
