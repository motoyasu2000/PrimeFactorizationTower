using Common;
using TMPro;
using UnityEngine;

/// <summary>
/// 現在のターンの人の名前を表示するUIを管理するためのクラス
/// </summary>
public class NowTurnTextManager : MonoBehaviour
{
    TextMeshProUGUI nowTurnNameText;

    void Start()
    {
        nowTurnNameText = GetComponent<TextMeshProUGUI>();
        DisplayNowTurnName(); //初期は現在のターンの人の名前で表示
    }

    public void DisplayNowTurnName()
    {
        string displayName = TurnMangaer.GetPlayerNames_NowTurn();
        if (displayName == GameInfo.AIName) displayName = "AI";
        nowTurnNameText.text = "Now Turn : " + displayName;
    }
}
