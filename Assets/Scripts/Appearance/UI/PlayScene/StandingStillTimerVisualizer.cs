using TMPro;
using UnityEngine;

/// <summary>
/// ブロックの落下後、ブロックが静止していた場合に、どのくらいの間ターンの切り替わりを待つかを可視化するクラス
/// </summary>
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

    /// <summary>
    /// 静止時間を表示するUIの表示非表示を切り替える。
    /// </summary>
    /// <param name="isActive">表示するか否か</param>
    public void SetActiveUI(bool isActive)
    {
        standingStillTimerUI.SetActive(isActive);
    }

    /// <summary>
    /// UIに表示する経過時間を更新する
    /// </summary>
    /// <param name="timer">UIに表示したい時間</param>
    public void UpdateTimer(float timer)
    {
        standingStillText.text = timer.ToString();
    }
}
