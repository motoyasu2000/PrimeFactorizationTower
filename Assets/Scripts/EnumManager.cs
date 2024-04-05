using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum型を外から便利に扱うためのクラス
public static class EnumManager
{
    //Enumの要素数を取得
    public static int GetEnumCount<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum)).Length;
    }

    //Enumの特定の要素のインデックスを取得
    public static int GetEnumIndex<TEnum>(TEnum value) where TEnum : Enum
    {
        return Array.IndexOf(Enum.GetValues(typeof(TEnum)), value);
    }

    //Enumの値から名前を取得
    public static string GetEnumName<TEnum>(TEnum value) where TEnum : Enum
    {
        return Enum.GetName(typeof(TEnum), value);
    }

    //名前からEnumの値を取得
    public static TEnum GetEnumValue<TEnum>(string name) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), name);
    }
}