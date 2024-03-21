using AWS;
using System.Collections;
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
        static int nowRankButton_isGrobal; //0ならローカル、1ならグローバル
        static int nowRankButton_gameMode;
        static int nowRankButton_diffcultyLevel;

        GameObject singlePlay;
        GameObject multiPlay;
        GameObject ranking;
        GameObject setting;
        GameObject credit;
        GameObject[] menus = new GameObject[5];
        Transform ranking_transform;

        //初期だと全て非アクティブで厄介なので、これらは全てUnityエンジン上から設定する。
        Button[] difficultyLevelButtons = new Button[3];
        Button[] rankButtons_localOrGlobal = new Button[2];
        Button[] rankButtons_gameMode = new Button[1];
        Button[] rankButtons_difficultyLevel = new Button[3];

        DynamoDBManager ddbManager;

        void Awake()
        {
            ddbManager = GameObject.Find("DynamoDBManager").GetComponent<DynamoDBManager>();
            InitializeMenus();
            if (ranking) InitializeRankingButtons();
            if (ranking) DisplayNowStateRanking();
            if (setting) InitializeDifficultyLevelButton();
        }
        void InitializeMenus()
        {
            singlePlay = GameObject.Find("SinglePlay");
            multiPlay = GameObject.Find("MultiPlay");
            ranking = GameObject.Find("Ranking");
            setting = GameObject.Find("Setting");
            credit = GameObject.Find("Credit");
            menus[0] = singlePlay;
            menus[1] = multiPlay;
            menus[2] = ranking;
            menus[3] = setting;
            menus[4] = credit;
        }

        //ランキングに扱うボタンの初期化。変数にボタンを割り当てるほか、最初に表示されているランキングのボタンが光るようにする。
        void InitializeRankingButtons()
        {
            rankButtons_localOrGlobal[0] = GameObject.Find("LocalButton").GetComponent<Button>();
            rankButtons_localOrGlobal[1] = GameObject.Find("GlobalButton").GetComponent<Button>();
            ChooseSingleButton(rankButtons_localOrGlobal, nowRankButton_isGrobal);

            rankButtons_gameMode[0] = GameObject.Find("PileUpButton").GetComponent<Button>();
            ChooseSingleButton(rankButtons_gameMode, nowRankButton_gameMode);

            rankButtons_difficultyLevel[0] = GameObject.Find("NormalButton").GetComponent<Button>();
            rankButtons_difficultyLevel[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
            rankButtons_difficultyLevel[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
            ChooseSingleButton(rankButtons_difficultyLevel, nowRankButton_diffcultyLevel); 
        }
        void InitializeDifficultyLevelButton()
        {
            difficultyLevelButtons[0] = GameObject.Find("NormalButton").GetComponent<Button>();
            difficultyLevelButtons[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
            difficultyLevelButtons[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
            ChooseSingleButton(difficultyLevelButtons, (int)GameModeManager.Ins.NowDifficultyLevel);
        }

        //現在のゲームモード・難易度でスコアを表示する。ランキングを開いて最初の状態。
        void DisplayNowStateRanking()
        {
            //難易度のみ現在の難易度を表示。ゲームの仕様上、私はこの難易度でこのゲームをするといったようなのがある程度決まっていることが多く、自分以外のランキングに興味がない人が多いと考えられるため。
            nowRankButton_diffcultyLevel = (int)GameModeManager.Ins.NowDifficultyLevel;
            //直前までプレイしていたゲームモード
            nowRankButton_gameMode = (int)GameModeManager.Ins.NowGameMode;
            nowRankButton_isGrobal = 0;
            DisplayRankingHandler();
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

        public void DisplayRankingByLocalOrGlobal(int isGrobal)
        {
            ChooseSingleButton(rankButtons_localOrGlobal, isGrobal);
            nowRankButton_isGrobal = isGrobal;
            DisplayRankingHandler();
        }

        public void DisplayRankingByGameMode(int gameMode)
        {
            ChooseSingleButton(rankButtons_gameMode, gameMode);
            nowRankButton_gameMode = gameMode;
            DisplayRankingHandler();
        }

        public void DisplayRankingByDifficultyLevel(int diffLevel)
        {
            ChooseSingleButton(rankButtons_difficultyLevel, diffLevel);
            nowRankButton_diffcultyLevel = diffLevel;
            DisplayRankingHandler();
        }

        public void ChooseSingleButton(Button[] buttons, int buttonsNumber)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == buttonsNumber) ChangeButtonColor_Selected(buttons[i]);
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

        public void DisplayRankingHandler()
        {
            StartCoroutine(DisplayRanking(nowRankButton_diffcultyLevel, nowRankButton_gameMode, nowRankButton_isGrobal));
        }

        public IEnumerator DisplayRanking(int diffLevel, int gameMode, int isGrobal)
        {
            ranking_transform = ranking.transform.Find("RankingTable"); //Rankingは子要素にスコアや順位を表示させるセルを10位まで持っている。

            //ローカルランキングが否か・ゲームモード・難易度によって異なるモノを取得。
            int[] scores = new int[10];
            string[] names = Enumerable.Repeat<string>("", 10).ToArray();

            //globalランキングを表示させる場合
            if (isGrobal == 1)
            {
                string modeAndLevel = $"{(GameModeManager.GameMode)gameMode}_{(GameModeManager.DifficultyLevel)diffLevel}";
                Debug.Log(modeAndLevel);
                bool isCompleted = false; //処理が完了したかどうかを追跡するフラグ

                Debug.Log("非同期処理開始");
                ddbManager.GetTop10Scores(modeAndLevel, (records) =>
                {
                    for (int i = 0; i < records.Count(); i++)
                    {
                        scores[i] = records[i].Score;
                        names[i] = records[i].Name;
                        Debug.Log($"{i+1}位のスコア: {scores[i]} 名前: {names[i]}");
                    }
                    isCompleted = true;
                });
                // 非同期処理が完了するまで待機
                yield return new WaitUntil(() => isCompleted);
                Debug.Log("非同期処理完了");
            }
            //ローカルランキングを表示させる場合。
            else if(isGrobal == 0)
            {
                switch ((GameModeManager.GameMode)gameMode)
                {
                    case GameModeManager.GameMode.PileUp:
                        scores = ScoreManager.Ins.PileUpScores[(GameModeManager.DifficultyLevel)diffLevel];
                        break;
                    default:
                        Debug.LogError("予期せぬゲームモードです");
                        break;
                }
            }
            else
            {
                Debug.LogError($"isGlobalに異常値が入っています isGlobal: {isGrobal}");
            }

            int rankCounter = 0;

            //全てのランキングのセルのスコアと名前を更新する
            foreach (Transform sell in ranking_transform)
            {
                if (sell.name.Substring(0, 4) == "Sell")
                {
                    //スコアの更新
                    TextMeshProUGUI score = sell.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                    score.text = scores[rankCounter].ToString();

                    TextMeshProUGUI name = sell.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                    name.text = names[rankCounter];

                    rankCounter++;
                }
            }
        }
    }
}