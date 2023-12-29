using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
}
