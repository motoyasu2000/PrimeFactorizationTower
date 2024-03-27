using Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OriginManager : MonoBehaviour
{
    bool isFirstGeneration = true;

    //�L�[���f���A�o�����[�����̑f���̐��̎����̐���
    Dictionary<int, int> originNumberDict = new Dictionary<int, int>();
    Dictionary<int, int> originNextNumberDict = new Dictionary<int, int>();
    public int OriginNumber => Helper.CalculateCompsiteNumberForDict(originNumberDict);
    public int OriginNextNumber => Helper.CalculateCompsiteNumberForDict(originNumberDict);

    GameModeManager gameModeManager;
    UpperUIManager upperUIManager;

    public Dictionary<int, int> OriginNumberDict => originNumberDict;

    void Awake()
    {
        gameModeManager = GameModeManager.Ins;
        upperUIManager = GameObject.Find("UpperUIManager").GetComponent<UpperUIManager>();
        GenerateOrigin();
    }

    private void Update()
    {
        if(string.IsNullOrWhiteSpace(upperUIManager.OriginNumberText.text))
        {
            GenerateOrigin();
            Debug.Log("a");
        }
    }

    //�����𐶐����郁�\�b�h(��Փx���ƂɈقȂ�f���v�[���A�قȂ�f���̐��A�قȂ�l�͈̔͂Œ�)
    public void GenerateOrigin()
    {
        //nextOrigin��origin�ɓ����
        originNumberDict = new Dictionary<int, int>(originNextNumberDict);

        //nextOrigin�̍X�V
        switch (GameModeManager.Ins.NowDifficultyLevel)
        {
            case GameModeManager.DifficultyLevel.Normal:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.NormalPool, int.MaxValue, 3, 5);
                break;

            case GameModeManager.DifficultyLevel.Difficult:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.DifficultPool, int.MaxValue, 2, 5);
                break;

            case GameModeManager.DifficultyLevel.Insane:
                originNextNumberDict = Helper.GenerateCompositeNumberDictCustom(gameModeManager.InsanePool, int.MaxValue, 2, 4);
                break;
        }

        //�������̌v�Z�ƕ\��
        int compositeNumberOrigin = Helper.CalculateCompsiteNumberForDict(originNumberDict);
        int compositeNumberNext = Helper.CalculateCompsiteNumberForDict(originNumberDict);
        upperUIManager.DisplayNumber(UpperUIManager.KindOfUI.Origin , compositeNumberOrigin.ToString());
        upperUIManager.DisplayNumber(UpperUIManager.KindOfUI.NextOrigin, compositeNumberNext.ToString());

        Debug.Log("Keys : " + string.Join(",", originNumberDict.Keys));
        Debug.Log("Values : " + string.Join(",", originNumberDict.Values));

        //�ŏ�����2����s���邱�ƂŁAorigin��next����������������
        if (isFirstGeneration)
        {
            isFirstGeneration = false;
            GenerateOrigin();
        }
    }
}
