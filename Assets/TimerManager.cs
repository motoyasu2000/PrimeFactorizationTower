using TMPro;
using UnityEngine;

/// <summary>
/// PlayScene内で表示されるタイマーを管理するクラス
/// </summary>
public class TimerManager : MonoBehaviour
{
    static readonly float startTime = 10f;
    float timer;

    TextMeshProUGUI timerText;
    GameOverManager gameOverManager;

    private void Start()
    {
        if (GameModeManager.Ins.NowGameMode != GameModeManager.GameMode.PileUp_Timer)
        {
            //gameObject.SetActive(false);
        }
        timerText = GetComponent<TextMeshProUGUI>();
        timer = startTime;
        timerText.text = Mathf.Ceil(timer).ToString();
        gameOverManager = FindAnyObjectByType<GameOverManager>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = Mathf.Ceil(timer).ToString();
        if(timer < 0)
        {
            gameOverManager.SetGameOverReason(GameOverManager.GameOverReason.TimeUp);
            gameOverManager.GameOver();
        }
    }
}
