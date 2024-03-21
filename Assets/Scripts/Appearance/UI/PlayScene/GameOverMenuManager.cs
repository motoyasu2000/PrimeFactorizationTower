using TMPro;
using UnityEngine;

namespace UI
{
    //ゲームオーバー時のメニューを管理するクラス
    public class GameOverMenuManager : MonoBehaviour
    {
        int oldMaxScore;
        int newScore;
        GameModeManager gameModeManager;
        GameManager gameManager;
        TextMeshProUGUI gameOverReason;
        GameObject nonUpdateRecord; //スコアを更新しなかった場合のレコードの表示UI
        GameObject updateRecord; //スコアを更新た場合のレコードの表示UI
        void Awake()
        {
            gameModeManager = GameModeManager.Ins;
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameOverReason = GameObject.Find("GameOverReason").GetComponent<TextMeshProUGUI>();
            //非アクティブなのでtransform.Findで取得
            nonUpdateRecord = transform.Find("Scores_NonUpdateRecord").gameObject;
            updateRecord = transform.Find("Scores_UpdateRecord").gameObject;

            PrintGameOverReason();
            DisplayScoreMenu();
        }

        //ゲームオーバー時の画面にて、ゲームオーバーになった理由を表示するメソッド
        void PrintGameOverReason()
        {
            switch (gameModeManager.NowGameMode)
            {
                //ゲームモードがPileUpであるとき、素因数分解を間違えた場合は、自分の選んだ数字で合成数を割った場合どうなるのかの表示、落下してしまった場合はFellDownと表示をする
                case GameModeManager.GameMode.PileUp:
                    if (gameManager.PrimeNumber_GO != 0) gameOverReason.text = $"{gameManager.CompositeNumber_GO} / {gameManager.PrimeNumber_GO} = {gameManager.CompositeNumber_GO / gameManager.PrimeNumber_GO}...{gameManager.CompositeNumber_GO % gameManager.PrimeNumber_GO}";
                    else gameOverReason.text = "Fell Down";
                    break;
            }
        }

        //ゲームオーバー時のスコアを表示させるメソッド 最高スコアを更新したか否かで異なるスコアの表示の仕方をする
        void DisplayScoreMenu()
        {
            oldMaxScore = gameManager.OldMaxScore;
            newScore = gameManager.NewScore;

            if (!gameManager.IsBreakScore)
            {
                nonUpdateRecord.SetActive(true);
                DisplayNonBreakRecord();
            }
            else
            {
                updateRecord.SetActive(true);
                DisplayBreakRecord();
            }

        }

        //スコアを更新しなかった場合のスコアの表示
        void DisplayNonBreakRecord()
        {
            TextMeshProUGUI newScoreText = nonUpdateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI oldScoreText = nonUpdateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
            newScoreText.text = "New Score: " + newScore.ToString();
            oldScoreText.text = "Max Score: " + oldMaxScore.ToString();
        }

        //スコアを更新した場合のスコアの表示
        void DisplayBreakRecord()
        {
            TextMeshProUGUI newScoreText = updateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI oldScoreText = updateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
            newScoreText.text = newScore.ToString();
            oldScoreText.text = "Old Max: " + oldMaxScore.ToString();
        }
    }
}