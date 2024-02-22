using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager_Play : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject explainPileUp;
    [SerializeField] GameObject settingMenu;
    [SerializeField] GameObject restartMenu;
    [SerializeField] GameObject backMenu;
    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        explainPileUp = canvas.transform.Find("ExplainPileUp").gameObject;
        settingMenu = canvas.transform.Find("SettingMenu").gameObject;
        restartMenu = canvas.transform.Find("RestartMenu").gameObject;
        backMenu = canvas.transform.Find("BackMenu").gameObject;
    }

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

    private void ExplainPileUp()
    {
        explainPileUp.SetActive(true);
    }

}
