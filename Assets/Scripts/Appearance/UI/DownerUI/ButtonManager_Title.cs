using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    //タイトルシーンで扱うボタンが呼び出すメソッドを持つクラス
    public class ButtonManager_Title : MonoBehaviour
    {
        GameObject singlePlay;
        GameObject multiPlay;
        GameObject history;
        GameObject setting;
        GameObject credit;
        Transform ranking_transform;
        Button[] difficultyLevelButtons = new Button[3];
        GameObject[] menus = new GameObject[5];

        void Awake()
        {
            singlePlay = GameObject.Find("SinglePlay");
            multiPlay = GameObject.Find("MultiPlay");
            history = GameObject.Find("History");
            setting = GameObject.Find("Setting");
            credit = GameObject.Find("Credit");

            InitializeDifficultyLevelButton();
            InitializeHistory();

            menus[0] = singlePlay;
            menus[1] = multiPlay;
            menus[2] = history;
            menus[3] = setting;
            menus[4] = credit;
        }

        public void DisplayMenu(GameObject menu)
        {
            menu.SetActive(true);
        }

        public void BackFirstMenu()
        {
            foreach (var menu in menus)
            {
                if (menu == null) continue;
                menu.gameObject.SetActive(false);
            }
        }

        //シーンを推移するメソッド達
        public void RestartScene()
        {
            SceneLoadHelper.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void MoveTitleScene()
        {
            SceneLoadHelper.LoadScene("TitleScene");
        }
        public void MovePlayScene()
        {
            SceneLoadHelper.LoadScene("PlayScene");
            GameModeManager.Ins.SetGameMode(GameModeManager.GameMode.PileUp);
        }

        //ランキングメニューのボタンで扱うメソッドたち
        //現在選択している難易度のタブを緑に、非選択のタブを赤に表示するメソッド
        public void ChangeDifficultyLevel(int diffLevel)
        {
            GameModeManager.Ins.ChangeDifficultyLevel((GameModeManager.DifficultyLevel)diffLevel);
            for (int i = 0; i < 3; i++)
            {
                if (i == diffLevel)　ChangeButtonColor_Selected(difficultyLevelButtons[i]);
                else ChangeButtonColor_Unselected(difficultyLevelButtons[i]);
            }
        }
        public void ChangeButtonColor_Selected(Button button)
        {
            button.GetComponent<Image>().color = Color.green;
        }
        public void ChangeButtonColor_Unselected(Button button)
        {
            button.GetComponent<Image>().color = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1);
        }
        //最初にどの難易度ボタンが光っているか
        void InitializeDifficultyLevelButton()
        {
            if (setting == null && history == null) return; //設定画面かランキング画面でない場合、処理を行わない。
            difficultyLevelButtons[0] = GameObject.Find("NormalButton").GetComponent<Button>();
            difficultyLevelButtons[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
            difficultyLevelButtons[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
            ChangeDifficultyLevel((int)GameModeManager.Ins.NowDifficultyLevel);
        }
        void InitializeHistory()
        {
            int diffLevel = (int)GameModeManager.Ins.NowDifficultyLevel;
            int gameMode = (int)GameModeManager.Ins.NowGameMode;
            DisplayRankingScores(diffLevel);
        }
        public void DisplayRankingScores(int diffLevel)
        {
            if (history == null) return;
            ranking_transform = history.transform.Find("Ranking"); //Rankingは子要素にスコアや順位を表示させるセルを10位まで持っている。
            int rankCounter = 0;

            //※他のゲームモードが追加されたならこのあたりに条件分岐で今のゲームモードのスコアのものを取得する処理を書く※

            //スコアの配列に現在の難易度にあったランキングを入れ、10位までのスコアを表示するGameObjectのセルに対してfor分を回して描画。
            int[] scores;
            scores = ScoreManager.Ins.PileUpScores[(GameModeManager.DifficultyLevel)diffLevel];
            foreach (Transform sell in ranking_transform)
            {
                if (sell.name.Substring(0, 4) != "Sell") return;
                TextMeshProUGUI rank = sell.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                rank.text = scores[rankCounter++].ToString();
            }
        }
    }
}