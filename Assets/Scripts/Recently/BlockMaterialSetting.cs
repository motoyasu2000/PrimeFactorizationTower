using Common;
using MaterialLibrary;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockMaterialSetting : MonoBehaviour
{
    MaterialDatabase materialDatabase;
    BlockInfo blockInfo;

    void Start()
    {
        //\�}�e���A���f�[�^�[�x�[�X��playerinfo����擾
        blockInfo = GetComponent<BlockInfo>();
        materialDatabase = PlayerInfoManager.Ins.MaterialDatabase;
        if (materialDatabase == null) return;


        //�}�e���A���̎擾�ƁA���̐ݒ��ۑ����Ă����f�[�^����ǂݎ���āA�K�؂ɔ��f����B
        IEnumParametersBinder binder = EnumParameterBinderManager.Binders[materialDatabase.GetBlockMaterialData(blockInfo.GetPrimeNumber()).binderIndex];
        Type dynamicEnumType = binder.EnumType;

        //�W�F�l���b�N�ŗ񋓌^���w�肷�郁�\�b�h�����t���N�V�����Ŏ擾���A�W�F�l���b�N�𓮓I�Ɏw��
        MethodInfo setPropertyFloatMethod = typeof(IEnumParametersBinder).GetMethod("SetPropertyFloat");
        MethodInfo setPropertyColorMethod = typeof(IEnumParametersBinder).GetMethod("SetPropertyColor");
        MethodInfo getEnumValueFromIndexMethod = typeof(EnumManager).GetMethod("GetEnumValueFromIndex");
        MethodInfo genericSetPropertyFloatMethod = setPropertyFloatMethod.MakeGenericMethod(dynamicEnumType);
        MethodInfo genericSetPropertyColorMethod = setPropertyColorMethod.MakeGenericMethod(dynamicEnumType);
        MethodInfo genericGetEnumValueFromIndexMethod = getEnumValueFromIndexMethod.MakeGenericMethod(dynamicEnumType);

        //�S�Ẵp�����[�^�[���f�[�^�[�x�[�X����󂯎�������̂ɕύX
        foreach (var parameter in materialDatabase.GetBlockMaterialData(blockInfo.GetPrimeNumber()).parameters)
        {
            object enumValue = null;
            //Enum�̃C���f�b�N�X����Enum�̒l�𓮓I�Ɏ擾
            enumValue = genericGetEnumValueFromIndexMethod.Invoke(null, new object[] { parameter.parameterEnumIndex });

            //�ǂݎ�����}�e���A���f�[�^�x�[�X�̏�񂩂�binder�̃}�e���A���̍X�V�B
            if (parameter.type == 0)
            {
                genericSetPropertyFloatMethod.Invoke(binder, new object[] { enumValue, parameter.floatValue });
            }
            else if (parameter.type == 1)
            {
                Color color = new Color(parameter.redValue, parameter.greenValue, parameter.blueValue);
                genericSetPropertyColorMethod.Invoke(binder, new object[] { enumValue, color });
            }
            else
            {
                Debug.LogError($"�z��O��type���w�肳��܂����B: {parameter.type}");
            }
        }
        GetComponent<SpriteRenderer>().material = new Material(binder.Material);
    }
}
