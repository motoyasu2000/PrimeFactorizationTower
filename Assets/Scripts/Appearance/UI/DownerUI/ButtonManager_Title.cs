using System.Linq;
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
        GameObject ranking;
        GameObject setting;
        GameObject credit;
        GameObject[] menus = new GameObject[5];
        Transform ranking_transform;

        //初期だと全て非アクティブで厄介なので、これらは全てUnityエンジン上から設定する。
        Button[] difficultyLevelButtons = new Button[3];
        Button[] rankButton_localOrGlobal = new Button[2];
        Button[] rankButton_gameMode = new Button[1];
        Button[] rankButton_difficultyLevel = new Button[3];

        void Awake()
        {
            singlePlay = GameObject.Find("SinglePlay");
            multiPlay = GameObject.Find("MultiPlay");
            ranking = GameObject.Find("Ranking");
            setting = GameObject.Find("Setting");
            credit = GameObject.Find("Credit");

            if(ranking) InitializeRankingButtons();
            if (ranking) DisplayRankingHandler();
            if (setting) InitializeDifficultyLevelButton();

            menus[0] = singlePlay;
            menus[1] = multiPlay;
            menus[2] = ranking;
            menus[3] = setting;
            menus[4] = credit;
        }

        void InitializeRankingButtons()
        {
            rankButton_difficultyLevel[0] = GameObject.Find("NormalButton").GetComponent<Button>();
            rankButton_difficultyLevel[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
            rankButton_difficultyLevel[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
            ChooseSingleButton(rankButton_difficultyLevel, (int)GameModeManager.Ins.NowDifficultyLevel);
        }
        void InitializeDifficultyLevelButton()
        {
            difficultyLevelButtons[0] = GameObject.Find("NormalButton").GetComponent<Button>();
            difficultyLevelButtons[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
            difficultyLevelButtons[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
            ChooseSingleButton(difficultyLevelButtons, (int)GameModeManager.Ins.NowDifficultyLevel);
        }
        void DisplayRankingHandler()
        {
            int diffLevel = (int)GameModeManager.Ins.NowDifficultyLevel;
            int gameMode = (int)GameModeManager.Ins.NowGameMode;
            DisplayRanking(diffLevel);
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
            ChooseSingleButton(difficultyLevelButtons, diffLevel);
        }

        public void DisplayRankingByDifficultyLevel(int diffLevel)
        {
            ChooseSingleButton(rankButton_difficultyLevel, diffLevel);
            DisplayRanking(diffLevel);
        }

        public void ChooseSingleButton(Button[] buttons, int diffLevel)
        {
            for(int i=0; i<buttons.Length; i++)
            {
                if (i == diffLevel) ChangeButtonColor_Selected(buttons[i]);
                else ChangeButtonColor_Unselected(buttons[i]);
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

        public void DisplayRanking(int diffLevel)
        {
            ranking_transform = ranking.transform.Find("RankingTable"); //Rankingは子要素にスコアや順位を表示させるセルを10位まで持っている。
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