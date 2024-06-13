using TMPro;
using UnityEngine;
using Common;

namespace UI
{
    //ゲームオーバー時のメニューを管理するクラス
    public class GameOverMenuManager : MonoBehaviour
    {
        int oldMaxScore;
        int newScore;
        GameOverManager gameOverManager;
        GameObject UI_TextGameOver;
        GameObject UI_TextFellDown;
        GameObject UI_nonUpdate; //スコアを更新しなかった場合のレコードの表示UI
        GameObject UI_updateRecord; //スコアを更新た場合のレコードの表示UI
        GameObject UI_winOrLose; //対戦モードで勝敗が決まった時に表示するUI
        void Awake()
        {
            gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();

            //非アクティブなのでtransform.Findで取得
            UI_TextGameOver = transform.Find("Text_GameOver").gameObject;
            UI_TextFellDown = transform.Find("TextFellDown").gameObject;
            UI_nonUpdate = transform.Find("Scores_NonUpdateRecord").gameObject;
            UI_updateRecord = transform.Find("Scores_UpdateRecord").gameObject;
            UI_winOrLose = transform.Find("WinOrLose").gameObject;

            if (GameModeManager.Ins.NowGameMode == GameModeManager.GameMode.Battle)
            {
                UI_winOrLose.SetActive(true);
                PrintWinOrLose();
            }
            else
            {
                UI_TextGameOver.SetActive(true);
                UI_TextFellDown.SetActive(true);
                DisplayScoreMenu();
            }
        }

        void PrintWinOrLose()
        {
            TextMeshProUGUI text = UI_winOrLose.GetComponent<TextMeshProUGUI>();
            if (TurnMangaer.GetPlayerNames_NowTurn() == PlayerInfoManager.Ins.PlayerName) text.text = "You Lose";
            else text.text = "You Win";
        }

        //ゲームオーバー時のスコアを表示させるメソッド 最高スコアを更新したか否かで異なるスコアの表示の仕方をする
        void DisplayScoreMenu()
        {
            oldMaxScore = GameInfo.Variables.GetOldMaxScore();
            newScore = GameInfo.Variables.GetNowScore();

            if (!gameOverManager.IsBreakScore)
            {
                UI_nonUpdate.SetActive(true);
                DisplayNonBreakRecord();
            }
            else
            {
                UI_updateRecord.SetActive(true);
                DisplayBreakRecord();
            }

        }

        //スコアを更新しなかった場合のスコアの表示
        void DisplayNonBreakRecord()
        {
            TextMeshProUGUI newScoreText = UI_nonUpdate.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI oldScoreText = UI_nonUpdate.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
            newScoreText.text = "New Score: " + newScore.ToString();
            oldScoreText.text = "Max Score: " + oldMaxScore.ToString();
        }

        //スコアを更新した場合のスコアの表示
        void DisplayBreakRecord()
        {
            TextMeshProUGUI newScoreText = UI_updateRecord.transform.Find("NewScore").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI oldScoreText = UI_updateRecord.transform.Find("OldScore").GetComponent<TextMeshProUGUI>();
            newScoreText.text = newScore.ToString();
            oldScoreText.text = "Old Max: " + oldMaxScore.ToString();
        }
    }
}