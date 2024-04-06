using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
    //Enum型を外から便利に扱うためのクラス
    public static class EnumManager
    {
        //Enumの要素数を取得
        public static int GetEnumCount<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Length;
        }

        //Enumの値からインデックスを取得
        public static int GetEnumIndexFromValue<TEnum>(TEnum value) where TEnum : Enum
        {
            return Array.IndexOf(Enum.GetValues(typeof(TEnum)), value);
        }

        //文字列からEnumのインデックスを取得
        public static int GetEnumIndexFromString<TEnum>(string name) where TEnum : Enum
        {
            return GetEnumIndexFromValue<TEnum>(GetEnumValueFromString<TEnum>(name));
        }

        //Enumの値から名前を取得
        public static string GetEnumNameFromValue<TEnum>(TEnum value) where TEnum : Enum
        {
            return Enum.GetName(typeof(TEnum), value);
        }

        //文字列からEnumの値を取得
        public static TEnum GetEnumValueFromString<TEnum>(string name) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), name);
        }

        //Enumの値から全ての値を取得
        public static string[] GetEnumNames<TEnum>() where TEnum : Enum
        {
            return Enum.GetNames(typeof(TEnum));
        }


    }
}