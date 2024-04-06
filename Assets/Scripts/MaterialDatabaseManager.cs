using Common;
using MaterialLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Rendering;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MaterialDatabaseManager : MonoBehaviour
{
    static readonly float initColorValue = 0.8f;
    static readonly float initFloatValue = 10f;
    MaterialDatabase materialDatabase;
    public MaterialDatabase MaterialDatabase => materialDatabase;

    private void Start()
    {
        LoadMaterialDatabase();
    }

    //�����Ŏ󂯎�������ɍ��킹�āAmaterialDatabase�̍X�V����
    public void SetShaderParameter(int blockNum, string materialPath, ParameterData parameter)
    {
        BlockMaterialData blockData;
        if (materialDatabase.blockMaterials.Any(b =>  b.blockNumber == blockNum))
        {
            blockData = materialDatabase.GetBlockMaterialData(blockNum);
        }
        else
        {
            Debug.LogError("�w�肳�ꂽ�C���f�b�N�X��������܂���ł����B");
            blockData = new BlockMaterialData() { blockNumber = blockNum };
        }
        blockData.materialPath = materialPath;
        blockData.AddParameter(parameter);
        materialDatabase.AddBlockMaterial(blockData);
    }

    void LoadMaterialDatabase()
    {
        //���ۂɂ�json�����݂Ƃ�悤�Ȃ����݂ɂ���
        materialDatabase = new MaterialDatabase();
    }

    //materialDatabase�̏������@����N�����ɌĂ΂��
    void InitializeMaterialDatabase()
    {
        foreach(var prime in GameModeManager.Ins.PrimeNumberPool)
        {
            DefaultMaterialEnumBinder defaultBinder = new DefaultMaterialEnumBinder();
            string parameterName = defaultBinder.MaterialPathAndName;
            BlockMaterialData materialData = new BlockMaterialData() {blockNumber = prime};
            for(int i=0; i<Enum.GetValues(defaultBinder.EnumType).Length; i++)
            {
                materialData.AddParameter(GenerateParameterData(parameterName));
            }
            materialDatabase.AddBlockMaterial(materialData);
        }
    }

    //MaterialDatabase��ŁA�����Ŏw�肵���u���b�N�Ɉ����Ɏw�肵���o�C���_�[�����蓖�Ă� �}�e���A���̃{�^���̃^�b�v���ɌĂяo����A���̃}�e���A���ŏ����������悤�ɂ���B
    public void SetBinderToBlock(IEnumParametersBinder ibinder, int blockNumber)
    {
        string parameterName = ibinder.MaterialPathAndName;
        BlockMaterialData materialData = new BlockMaterialData() { blockNumber = blockNumber };
        for (int i=0; i<Enum.GetValues(ibinder.EnumType).Length; i++)
        {
            materialData.AddParameter(GenerateParameterData(parameterName));
        }
    }

    //�p�����[�^�[�̕�����ɑΉ�����ParameterData��Ԃ�
    ParameterData GenerateParameterData(string parameterName)
    {
        ParameterData parameterData = new ParameterData();
        if (parameterName.Contains("Color") || parameterName.Contains("color"))
        {
            parameterData.redValue = initColorValue;
            parameterData.greenValue = initColorValue;
            parameterData.blueValue = initColorValue;
        }
        //color�ȊO(���̂Ƃ���float�̂�)�͑S��1��������
        else
        {
            parameterData.floatValue = initFloatValue;
        }
        return parameterData;
    }
}
