using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    //Playシーン用のボタンの機能を提供するクラス
    public class ButtonManager_Play : MonoBehaviour
    {
        GameObject canvas;
        GameObject explainPileUp;
        GameObject settingMenu;
        GameObject restartMenu;
        GameObject backMenu;
        void Awake()
        {
            canvas = GameObject.Find("Canvas");
            explainPileUp = canvas.transform.Find("ExplainPileUp").gameObject;
            settingMenu = canvas.transform.Find("SettingMenu").gameObject;
            restartMenu = canvas.transform.Find("RestartMenu").gameObject;
            backMenu = canvas.transform.Find("BackMenu").gameObject;
        }

        //様々なメニューの表示
        public void DisplaySettingMenu(bool isDisplay)
        {
            settingMenu.SetActive(isDisplay);
        }
        public void DisplayRestartMenu(bool isDisplay)
        {
            restartMenu.SetActive(isDisplay);
        }
        public void DisplayBackMenu(bool isDisplay)
        {
            backMenu.SetActive(isDisplay);
        }

        //シーン間の推移
        public void MoveTitleScene()
        {
            SceneLoadHelper.LoadScene("TitleScene");
        }
        public void MovePlayScene()
        {
            SceneLoadHelper.LoadScene("PlayScene");
        }
        public void ExplainHowToPlay()
        {
            settingMenu.SetActive(false);
            if (GameModeManager.GameModemanagerInstance.NowGameMode == GameModeManager.GameMode.PileUp) ExplainPileUp();
        }

        //チュートリアルの表示
        private void ExplainPileUp()
        {
            explainPileUp.SetActive(true);
        }

    }

}