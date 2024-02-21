using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenuManager : MonoBehaviour
{
    ScoreManager scoreManager;
    GameModeManager gameModeManager;
    int oldMaxScore;
    int newScore;
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI gameOverReason;
    [SerializeField] GameObject Scores_NonUpdateRecord;
    [SerializeField] GameObject Scores_UpdateRecord;
    void Awake()
    {
        scoreManager = ScoreManager.ScoreManagerInstance;
        gameModeManager = GameModeManager.GameModemanagerInstance;

        PrintGameOverReason();
        DisplayScoreMenu();
    }

    void Update()
    {
        
    }

    void PrintGameOverReason()
    {
        if (gameModeManager.NowGameMode == GameModeManager.GameMode.PileUp)
        {
            if (gameManager.PrimeNumber_GO != 0) gameOverReason.text = $"{gameManager.CompositeNumber_GO} / {gameManager.PrimeNumber_GO} = {gameManager.CompositeNumber_GO / gameManager.PrimeNumber_GO}...{gameManager.CompositeNumber_GO % gameManager.PrimeNumber_GO}";
            else gameOverReason.text = "Fell Down";
        }
    }

    void DisplayScoreMenu()
    {
        oldMaxScore = gameManager.OldMaxScore;
        newScore = gameManager.NewScore;
        Debug.Log(oldMaxScore);
        if (newScore <= oldMaxScore)
        {
            Scores_NonUpdateRecord.SetActive(true);
            PrintNonUpdateRecord();
        }
        else
        {
            Scores_UpdateRecord.SetActive(true);
            PrintUpdateRecord();
        }
        
    }

    void PrintNonUpdateRecord()
    {
        TextMeshProUGUI newScoreText = Scores_NonUpdateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI oldScoreText = Scores_NonUpdateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
        newScoreText.text = "New Score: " + newScore.ToString();
        oldScoreText.text = "Max Score: " +oldMaxScore.ToString();
    }

    void PrintUpdateRecord()
    {
        TextMeshProUGUI newScoreText = Scores_UpdateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI oldScoreText = Scores_UpdateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
        newScoreText.text = newScore.ToString();
        oldScoreText.text = "Old Max: " + oldMaxScore.ToString();
    }
}
