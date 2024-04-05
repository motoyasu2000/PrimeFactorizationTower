using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum�^���O����֗��Ɉ������߂̃N���X
public static class EnumManager
{
    //Enum�̗v�f�����擾
    public static int GetEnumCount<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum)).Length;
    }

    //Enum�̓���̗v�f�̃C���f�b�N�X���擾
    public static int GetEnumIndex<TEnum>(TEnum value) where TEnum : Enum
    {
        return Array.IndexOf(Enum.GetValues(typeof(TEnum)), value);
    }

    //Enum�̒l���疼�O���擾
    public static string GetEnumName<TEnum>(TEnum value) where TEnum : Enum
    {
        return Enum.GetName(typeof(TEnum), value);
    }

    //���O����Enum�̒l���擾
    public static TEnum GetEnumValue<TEnum>(string name) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), name);
    }
}