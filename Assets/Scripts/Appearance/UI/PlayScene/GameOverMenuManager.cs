using TMPro;
using UnityEngine;
using Common;

namespace UI
{
    /// <summary>
    /// ゲームオーバー時に表示されるメニューを管理するクラス
    /// </summary>
    public class GameOverMenuManager : MonoBehaviour
    {
        int oldMaxScore;
        int newScore;
        GameOverManager gameOverManager;
        GameObject UI_TextGameOver;
        GameObject UI_TextGameOverReason;
        GameObject UI_nonUpdate; //スコアを更新しなかった場合のレコードの表示UI
        GameObject UI_updateRecord; //スコアを更新た場合のレコードの表示UI
        GameObject UI_winOrLose; //対戦モードで勝敗が決まった時に表示するUI
        void Awake()
        {
            gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();

            //非アクティブなのでtransform.Findで取得
            UI_TextGameOver = transform.Find("Text_GameOver").gameObject;
            UI_TextGameOverReason = transform.Find("TextGameOverReason").gameObject;
            UI_nonUpdate = transform.Find("Scores_NonUpdateRecord").gameObject;
            UI_updateRecord = transform.Find("Scores_UpdateRecord").gameObject;
            UI_winOrLose = transform.Find("WinOrLose").gameObject;

            //Battleモードなら勝敗を表示、そうでなければスコアを表示
            if (GameModeManager.Ins.NowGameMode == GameModeManager.GameMode.Battle)
            {
                UI_winOrLose.SetActive(true);
                PrintWinOrLose();
            }
            else
            {
                UI_TextGameOver.SetActive(true);
                UI_TextGameOverReason.SetActive(true);
                PrintGameOverReason();
                DisplayScoreMenu();
            }
        }

        /// <summary>
        /// ゲームオーバーになった理由を表示する。
        /// GameOverManagerが内部にゲームオーバーの際にその理由を保存するようになっており、その値に応じてテキストを変化させる。
        /// </summary>
        void PrintGameOverReason()
        {
            TextMeshProUGUI gameOverUIText = UI_TextGameOverReason.GetComponent<TextMeshProUGUI>();
            switch (gameOverManager.Reason)
            {
                case GameOverManager.GameOverReason.DropDown:
                    gameOverUIText.text = "Drop Down";
                    break;
                case GameOverManager.GameOverReason.TimeUp:
                    gameOverUIText.text = "Time Up";
                    break;
                default:
                    Debug.LogError("ゲームオーバーの理由に予期せぬ列挙型の値が使われています。");
                    break;
            }
        }

        //勝敗を表示する
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