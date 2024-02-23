using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
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

        //ゲームオーバー時の画面にて、ゲームオーバーになった理由を表示するメソッド
        void PrintGameOverReason()
        {
            switch (gameModeManager.NowGameMode)
            {
                //ゲームモードがPileUpであるとき、素因数分解を間違えた場合は、自分の選んだ数字で合成数を割った場合どうなるのかの表示、落下してしまった場合はFellDownと表示をする。
                case GameModeManager.GameMode.PileUp:
                    if (gameManager.PrimeNumber_GO != 0) gameOverReason.text = $"{gameManager.CompositeNumber_GO} / {gameManager.PrimeNumber_GO} = {gameManager.CompositeNumber_GO / gameManager.PrimeNumber_GO}...{gameManager.CompositeNumber_GO % gameManager.PrimeNumber_GO}";
                    else gameOverReason.text = "Fell Down";
                    break;
            }
        }

        //ゲームオーバー時のスコアを表示させるメソッド。最高スコアを更新したか否かで異なるスコアの表示の仕方をする。
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

        //スコアを更新しなかった場合のスコアの表示。
        void PrintNonUpdateRecord()
        {
            TextMeshProUGUI newScoreText = Scores_NonUpdateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI oldScoreText = Scores_NonUpdateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
            newScoreText.text = "New Score: " + newScore.ToString();
            oldScoreText.text = "Max Score: " + oldMaxScore.ToString();
        }

        //スコアを更新した場合のスコアの表示。
        void PrintUpdateRecord()
        {
            TextMeshProUGUI newScoreText = Scores_UpdateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI oldScoreText = Scores_UpdateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
            newScoreText.text = newScore.ToString();
            oldScoreText.text = "Old Max: " + oldMaxScore.ToString();
        }
    }
}