using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Material����json�`���ŃZ�[�u�E���[�h���s�����߂̃N���X
[System.Serializable]
public class MaterialData
{
    public string materialPath;
    public List<ParameterData> parameters;
}

//�}�e���A���̃V�F�[�_�[�̊e�p�����[�^�[�̏����Z�[�u�E���[�h���邽�߂̃N���X
[System.Serializable]
public class ParameterData
{
    public int parameterEnumIndex;
    public float floatValue;
    public Color colorValue;
}