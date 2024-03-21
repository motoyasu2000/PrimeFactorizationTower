using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NameInputButton : MonoBehaviour
{
    TMP_InputField nameText; //名前を入力する欄
    TextMeshProUGUI errorText; //エラー理由を表示するテキスト
    GameObject inputNameMenuBackGround; //名前を入力する画面の全体。これを見えるようにすることで、名前入力の画面を表示させる。

    //初期化
    private void Start()
    {
        nameText = GameObject.Find("NameInputField").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();
        inputNameMenuBackGround = GameObject.Find("InputNameMenuBackGround");
    }

    //名前の登録処理
    public void ConfirmName()
    {
        string playerName = nameText.text.Trim(); //入力された名前の取得

        if (CheckError(playerName)) return; //エラーがあれば以降の処理を行わない。

        //名前を保存して、画面を非表示。
        PlayerInfoManager.Ins.InputNameProcess(playerName);
        inputNameMenuBackGround.SetActive(false);
    }

    //引数で受け取った文字列が利用可能な名前化を調査する。エラーがあればtrueを返す。
    bool CheckError(string playerName)
    {
        //もし16文字より多ければエラー
        if (playerName.Count() > 16)
        {
            errorText.text = "too long";
            return true;
        }
        //もしアルファベット文字列以外が使われていたらエラー
        if (!IsAlphanumeric(playerName))
        {
            errorText.text = "alphanumeric characters only";
            return true;
        }
        return false;
    }

    //アルファベット文字列であればtrue
    static bool IsAlphanumeric(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
    }
}