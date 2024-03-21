using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NameInputButton : MonoBehaviour
{
    TMP_InputField nameText;
    TextMeshProUGUI errorText;
    GameObject inputNameMenuBackGround;

    private void Start()
    {
        nameText = GameObject.Find("NameInputField").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();
        inputNameMenuBackGround = GameObject.Find("InputNameMenuBackGround");
    }
    public void ConfirmName()
    {
        string playerName = nameText.text.Trim();
        Debug.Log(playerName);
        if (playerName.Count() > 20)
        {
            errorText.text = "too long";
            return;
        }
        if (!IsAlphanumeric(playerName))
        {
            errorText.text = "alphanumeric characters only";
            return;
        }
        PlayerInfoManager.Ins.InputNameProcess(playerName);
        inputNameMenuBackGround.SetActive(false);
    }

    //アルファベット文字列であればtrue
    static bool IsAlphanumeric(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
    }
}