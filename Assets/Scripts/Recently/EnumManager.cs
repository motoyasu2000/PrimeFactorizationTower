using System;
namespace Common {
    //Enum型を外から便利に扱うためのクラス
    public static class EnumManager
    {
        //インデックスからEnum型の値の文字列を取得
        public static string GetStringFromIndex<TEnum>(int index) where TEnum : Enum
        {
            return $"{(GetEnumValueFromIndex<TEnum>(index))}";
        }

        //インデックスからEnum型の値を取得
        public static TEnum GetEnumValueFromIndex<TEnum>(int index) where TEnum : Enum
        {
            return (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(index);
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