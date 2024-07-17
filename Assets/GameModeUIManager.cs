using UnityEngine;

/// <summary>
/// ゲームモードに合わせて表示するUIを切り替えるクラス
/// </summary>
public class GameModeUIManager : MonoBehaviour
{
    GameObject timerUI; //タイマーのUI
    GameObject scoresAndTimerUI; //シングルプレイ時に表示されるスコアのUIとタイマーのUIを内包するUI
    GameObject nowTurnNameUI; //マルチプレイ時に表示される、現在誰のターンであるのかを表すUI

    private void Start()
    {
        timerUI = FindObjectOfType<TimerManager>().gameObject;
        scoresAndTimerUI = GameObject.Find("ScoresAndTimer");
        nowTurnNameUI = GameObject.Find("NowTurnName");

        //可読性向上のために、いったんすべて非表示にしておいて、必要なものを表示するようにする。
        timerUI.SetActive(false);
        scoresAndTimerUI.SetActive(false);
        nowTurnNameUI.SetActive(false);

        ChooseUIWithGameMode();
    }
    void ChooseUIWithGameMode()
    {
        switch (GameModeManager.Ins.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                scoresAndTimerUI.SetActive(true);
                break;

            case GameModeManager.GameMode.PileUp_60s:
                scoresAndTimerUI.SetActive(true);
                timerUI.SetActive(true);
                break;

            case GameModeManager.GameMode.Battle:
                nowTurnNameUI.SetActive(true);
                break;

            default:
                Debug.LogError("予期せぬゲームモードです。");
                break;
        }
    }
}
