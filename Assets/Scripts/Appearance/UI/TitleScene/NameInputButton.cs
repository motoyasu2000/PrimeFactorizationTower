using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NameInputButton : MonoBehaviour
{
    TMP_InputField nameText; //���O����͂��闓
    TextMeshProUGUI errorText; //�G���[���R��\������e�L�X�g
    GameObject inputNameMenuBackGround; //���O����͂����ʂ̑S�́B�����������悤�ɂ��邱�ƂŁA���O���͂̉�ʂ�\��������B

    //������
    private void Start()
    {
        nameText = GameObject.Find("NameInputField").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();
        inputNameMenuBackGround = GameObject.Find("InputNameMenuBackGround");
    }

    //���O�̓o�^����
    public void ConfirmName()
    {
        string playerName = nameText.text.Trim(); //���͂��ꂽ���O�̎擾

        if (CheckError(playerName)) return; //�G���[������Έȍ~�̏������s��Ȃ��B

        //���O��ۑ����āA��ʂ��\���B
        PlayerInfoManager.Ins.InputNameProcess(playerName);
        inputNameMenuBackGround.SetActive(false);
    }

    //�����Ŏ󂯎���������񂪗��p�\�Ȗ��O���𒲍�����B�G���[�������true��Ԃ��B
    bool CheckError(string playerName)
    {
        //����16������葽����΃G���[
        if (playerName.Count() > 16)
        {
            errorText.text = "too long";
            return true;
        }
        //�����A���t�@�x�b�g������ȊO���g���Ă�����G���[
        if (!IsAlphanumeric(playerName))
        {
            errorText.text = "alphanumeric characters only";
            return true;
        }
        return false;
    }

    //�A���t�@�x�b�g������ł����true
    static bool IsAlphanumeric(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
    }
}