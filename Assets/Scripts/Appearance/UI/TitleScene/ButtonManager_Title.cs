using Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// TitleScene用のボタンによって呼ばれる機能を提供するクラス
    /// </summary>
    public class ButtonManager_Title : MonoBehaviour
    {
        static readonly int menuCount = 5;
        static readonly int diffLevelCount = 3;
        static readonly int gameModeCount = 2; //ランキングを表示する可能性のあるゲームモードの数。

        //Titleで扱うメニュー
        GameObject singlePlay;
        GameObject multiPlay;
        GameObject ranking;
        GameObject setting;
        GameObject credit;
        GameObject[] allMenus = new GameObject[menuCount];

        Button[] difficultyLevelButtons = new Button[diffLevelCount]; //難易度を切り替えるボタン

        int[] scores; //ランキングに表示するスコア
        string[] names; //ランキングに表示する名前
        Transform ranking_transform; //ランキングを表示するセルの親
        GameObject rankingCell; //ランキングを表示するときに生成されるセル

        //現在表示させているランキングが何のランキングであるのかを示す変数。
        public enum LocalOrGlobal {local, global}
        LocalOrGlobal nowRankButton_localOrGlobal; //0ならローカル、1ならグローバル
        int nowRankButton_gameMode;
        int nowRankButton_difficultyLevel;

        //どのランキングを表示するか選ぶタブボタン
        Button[] rankButtons_localOrGlobal = new Button[Enum.GetValues(typeof(LocalOrGlobal)).Length]; //グローバルかローカルかの２つ
        Button[] rankButtons_gameMode = new Button[gameModeCount];
        Button[] rankButtons_difficultyLevel = new Button[diffLevelCount];

        DynamoDBManager ddbManager;

        void Start()
        {
            InitializeMenus();
            InitializeRankingButtons();
            InitializeDifficultyLevelButton();
            InactiveMenus();
            rankingCell = Resources.Load("RankingCell") as GameObject;
            ddbManager = GameObject.Find("DynamoDBManager").GetComponent<DynamoDBManager>();
            ranking_transform = ranking.transform.Find("RankingTable");
        }
        void InitializeMenus()
        {
            //すべてのメニュー画面を変数に格納し
            singlePlay = GameObject.Find("SinglePlay");
            multiPlay = GameObject.Find("MultiPlay");
            ranking = GameObject.Find("Ranking");
            setting = GameObject.Find("Setting");
            credit = GameObject.Find("Credit");

            //すべてのメニューを配列に格納する。
            allMenus[0] = singlePlay;
            allMenus[1] = multiPlay;
            allMenus[2] = ranking;
            allMenus[3] = setting;
            allMenus[4] = credit;
        }

        //ランキングに扱うボタンの初期化。変数にボタンを割り当てるほか、最初に表示されているランキングのボタンが緑になるようにする。
        void InitializeRankingButtons()
        {
            rankButtons_localOrGlobal[0] = GameObject.Find("LocalButton").GetComponent<Button>();
            rankButtons_localOrGlobal[1] = GameObject.Find("GlobalButton").GetComponent<Button>();
            ChooseSingleButton(rankButtons_localOrGlobal, (int)nowRankButton_localOrGlobal);

            rankButtons_gameMode[0] = GameObject.Find("PileUpButton").GetComponent<Button>();
            rankButtons_gameMode[1] = GameObject.Find("PileUp60sButton").GetComponent<Button>();
            ChooseSingleButton(rankButtons_gameMode, nowRankButton_gameMode);

            rankButtons_difficultyLevel[0] = GameObject.Find("NormalButton").GetComponent<Button>();
            rankButtons_difficultyLevel[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
            rankButtons_difficultyLevel[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
            ChooseSingleButton(rankButtons_difficultyLevel, nowRankButton_difficultyLevel); 
        }

        /// <summary>
        /// 難易度のボタンを変数に格納し、現在選択されている難易度のものの色が緑に表示されるようにする。
        /// </summary>
        void InitializeDifficultyLevelButton()
        {
            difficultyLevelButtons[0] = GameObject.Find("ChangeNormalButton").GetComponent<Button>();
            difficultyLevelButtons[1] = GameObject.Find("ChangeDifficultButton").GetComponent<Button>();
            difficultyLevelButtons[2] = GameObject.Find("ChangeInsaneButton").GetComponent<Button>();
            ChooseSingleButton(difficultyLevelButtons, (int)GameModeManager.Ins.NowDifficultyLevel);
        }

        /// <summary>
        /// GameObject.Findでゲームオブジェクトを探したいため、探索対象のゲームオブジェクトははじめはactiveにしておきたいが、
        /// 初期のタイトル画面では非activeでいてほしい。
        /// はじめはactiveで初期化が完了したら非activeにする。
        /// </summary>
        void InactiveMenus()
        {
            foreach(var menu in allMenus)
            {
                menu.SetActive(false);
            }
        }

        //----------------メニューの推移---------------------
        public void DisplayMenu(GameObject menu)
        {
            menu.SetActive(true);
        }

        public void BackFirstMenu()
        {
            foreach (var menu in allMenus)
            {
                if (menu == null) continue;
                menu.SetActive(false);
            }
        }
        //----------------メニューの推移---------------------

        //----------------シーンの推移---------------------
        public void RestartScene()
        {
            Helper.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void MoveTitleScene()
        {
            Helper.LoadScene("TitleScene");
        }
        public void MovePlayScene()
        {
            Helper.LoadScene("PlayScene");
        }
        public void MoveMaterialScene()
        {
            Helper.LoadScene("MaterialScene");
        }
        //----------------シーンの推移---------------------

        //------------------PlaySceneの設定-----------------------
        public void SetupPileUp()
        {
            GameModeManager.Ins.SetGameMode(GameModeManager.GameMode.PileUp);
            TurnNameSetter.Ins.SetNames_Single();//参加者の名前をプレイヤー一人にすることで、シングルで遊べるようにする。
        }

        public void SetupPileUp60s()
        {
            GameModeManager.Ins.SetGameMode(GameModeManager.GameMode.PileUp_60s);
            TurnNameSetter.Ins.SetNames_Single();//参加者の名前をプレイヤー一人にすることで、シングルで遊べるようにする。
        }

        public void SetupAIBattle()
        {
            GameModeManager.Ins.SetGameMode(GameModeManager.GameMode.Battle); //ゲームモードを非シングルに
            TurnNameSetter.Ins.SetNames_AI();//参加者の名前をプレイヤーとAIにすることで、参加者とAIで対戦できるようにする。
        }
        //------------------PlaySceneの設定-----------------------

        //----------------ランキングのタブボタンの操作---------------------
        public void DisplayRankingByLocalOrGlobal(int isGrobal)
        {
            ChooseSingleButton(rankButtons_localOrGlobal, isGrobal);
            nowRankButton_localOrGlobal = (LocalOrGlobal)isGrobal;
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
            nowRankButton_difficultyLevel = diffLevel;
            DisplayRankingHandler();
        }
        //----------------ランキングのタブボタンの操作---------------------

        //----------------難易度変更ボタンの操作---------------------
        public void ChangeDifficultyLevel(int diffLevel)
        {
            GameModeManager.Ins.ChangeDifficultyLevel((GameModeManager.DifficultyLevel)diffLevel);
            ChooseSingleButton(difficultyLevelButtons, diffLevel);
        }
        //----------------難易度変更ボタンの操作---------------------

        //----------------現在選ばれているランキングのタブや難易度選択ボタンが単一であることを保証する---------------------
        /// <summary>
        /// 引数で受け取ったボタンの配列のうち、選択中のものを緑、選択されてないものを赤にして表示する
        /// </summary>
        /// <param name="buttons">どのボタンの配列に対して色の選択を行うか</param>
        /// <param name="buttonsNumber">どのボタンを緑にするか(buttonsのインデックス)</param>
        public void ChooseSingleButton(Button[] buttons, int buttonsNumber)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == buttonsNumber) ChangeButtonColor_Selected(buttons[i]); //選択中のボタンを緑に
                else ChangeButtonColor_Unselected(buttons[i]); //選択されていないボタンを赤に
            }
        }
        public void ChangeButtonColor_Selected(Button button)
        {
            button.GetComponent<Image>().color = Color.green;
        }
        public void ChangeButtonColor_Unselected(Button button)
        {
            button.GetComponent<Image>().color = GameInfo.ButtonGray;
        }
        //----------------現在選ばれているランキングのタブや難易度選択ボタンが単一であることを保証する---------------------

        //----------------ランキングの表示---------------------
        //現在のゲームモード・難易度でスコアを表示する。ランキングを開いて最初の状態。
        public void DisplayNowStateRanking()
        {
            //難易度のみ現在の難易度を表示。ゲームの仕様上、私はこの難易度でこのゲームをするといったようなのがある程度決まっていることが多く、自分以外のランキングに興味がない人が多いと考えられるため。
            nowRankButton_difficultyLevel = (int)GameModeManager.Ins.NowDifficultyLevel;
            //直前までプレイしていたゲームモード
            nowRankButton_gameMode = (int)GameModeManager.Ins.NowGameMode;
            //初期はローカルを表示
            nowRankButton_localOrGlobal = 0;
            DisplayRankingHandler();
            ChooseSingleButton(rankButtons_difficultyLevel, nowRankButton_difficultyLevel); //現在の難易度のものが、表示されていることを可視化する。
        }

        public async void DisplayRankingHandler()
        {
            await DisplayRanking(nowRankButton_difficultyLevel, nowRankButton_gameMode, nowRankButton_localOrGlobal);
        }

        //ローカルランキングが否か・ゲームモード・難易度によって異なるランキングを表示する。
        public async Task DisplayRanking(int diffLevel, int gameMode, LocalOrGlobal localOrGlobal)
        {
            var (scores, names) = await GetNamesAndScores(diffLevel, gameMode, localOrGlobal);
            //ランキングに表示する各セルの生成。
            GenerateRankingCells();
        }

        /// <summary>
        /// トップ10の名前とスコアを計算してタプルとして返す。
        /// </summary>
        /// <param name="diffLevel">どの難易度のスコアを取得したいか</param>
        /// <param name="gameMode">どのゲームモードのスコアを取得したいか</param>
        /// <param name="localOrGlobal">ローカルと世界ランキングどちらのスコアを取得したいか</param>
        /// <returns>トップ10のスコア(int[])と名前(string[])のタプル</returns>
        async Task<(int[], string[])> GetNamesAndScores(int diffLevel, int gameMode, LocalOrGlobal localOrGlobal)
        {
            //スコアと名前のリストの初期化
            scores = new int[10];
            names = new string[10];

            //世界ランキングを表示する場合にはdynamoDBからデータを取得する
            if (localOrGlobal == LocalOrGlobal.global)
            {
                string modeAndLevel = $"{(GameModeManager.GameMode)gameMode}_{(GameModeManager.DifficultyLevel)diffLevel}";

                var records = await ddbManager.GetScoreTop10(modeAndLevel);
                {
                    for (int i = 0; i < records.Count(); i++)
                    {
                        scores[i] = records[i].Score;
                        names[i] = records[i].PlayerName;
                        Debug.Log($"{i + 1}位のスコア: {scores[i]} 名前: {names[i]}");
                    }
                };
            }

            //ローカルランキングを表示させる場合にはScoreManagerの保持するスコアを取得する
            else if (localOrGlobal == LocalOrGlobal.local)
            {
                scores = ScoreManager.Ins.AllScores[(GameModeManager.GameMode)gameMode][(GameModeManager.DifficultyLevel)diffLevel];
                names = Enumerable.Repeat<string>(PlayerInfoManager.Ins.PlayerName, 10).ToArray(); //ローカルランキングでは全て自分の名前。
            }

            else
            {
                Debug.LogError($"localOrGlobalに予期せぬ値が入っています localOrGlobal: {localOrGlobal}");
            }

            return (scores, names);
        }
        void GenerateRankingCells()
        {
            ResetRankingCell(); //過去表示されていたランキングのセルを消去してリセット

            //セルを分割するアンカーポイントの計算
            float[] splitsPoints_y = Helper.CalculateSplitAnchorPoints(GameInfo.RankDisplayLimit); 
            Array.Reverse(splitsPoints_y); //ランキングは上から表示させたいため、上の位置のアンカーポイントが最初の方に来るようにする。

            //スコアが0になるまで生成し続ける。
            for(int i=0; scores[i] > 0 && i < 10; i++)
            {
                //生成・親オブジェクトの設定・スケーリング
                GameObject nowRankingCell = Instantiate(rankingCell);
                nowRankingCell.transform.SetParent(ranking_transform);
                nowRankingCell.transform.localScale = Vector3.one;

                //どの位置でセルを分割するのかの設定
                RectTransform cellRectTransform = nowRankingCell.GetComponent<RectTransform>();
                cellRectTransform.anchorMin = new Vector2(0, splitsPoints_y[i+1]);
                cellRectTransform.anchorMax = new Vector2(1, splitsPoints_y[i]);

                //設定した分割の地点で分割
                cellRectTransform.offsetMin = Vector2.zero;
                cellRectTransform.offsetMax = Vector2.zero;

                //順位・名前・スコアなど、表示される文字の設定
                TextMeshProUGUI rank = nowRankingCell.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
                rank.text = $"{i+1}: ";

                TextMeshProUGUI score = nowRankingCell.transform.Find("Score").GetComponent<TextMeshProUGUI>();
                score.text = scores[i].ToString();

                TextMeshProUGUI name = nowRankingCell.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                name.text = names[i];
            }
        }

        //セルの親オブジェクトとなるゲームオブジェクトの子要素をすべて削除することによりリセットを行う。
        void ResetRankingCell()
        {
            foreach(Transform cell in ranking_transform)
            {
                Destroy(cell.gameObject);
            }
        }
        //----------------ランキングの表示---------------------
    }
}