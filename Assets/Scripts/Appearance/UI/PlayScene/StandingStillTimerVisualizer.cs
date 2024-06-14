using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//ブロックの落下後、ブロックが静止していた場合に、どのくらいの間ターンの切り替わりを待つかを表示するクラス
public class StandingStillTimerVisualizer : MonoBehaviour
{
    GameObject standingStillTimerUI;
    TextMeshProUGUI standingStillText;
    private void Start()
    {
        standingStillTimerUI = GameObject.Find("StandingStillTimerUI");
        standingStillText = standingStillTimerUI.GetComponentInChildren<TextMeshProUGUI>();
        standingStillTimerUI.SetActive(false); //Findで見つけるために初期はアクティブだがゲームの初めは非アクティブであるべき
    }
    public void SetActiveUI(bool isActive)
    {
        standingStillTimerUI.SetActive(isActive);
    }

    public void UpdateTimer(float timer)
    {
        standingStillText.text = timer.ToString();
    }
}
