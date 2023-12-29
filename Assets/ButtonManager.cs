using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject singlePlay;
    [SerializeField] GameObject multiPlay;
    [SerializeField] GameObject history;
    [SerializeField] GameObject setting;
    [SerializeField] GameObject credit;
    Button[] difficultyLevelButtons = new Button[3];
    GameObject[] menus = new GameObject[5];

    [SerializeField] Scene playScene;

    void Awake()
    {
        singlePlay = GameObject.Find("SinglePlay");
        multiPlay = GameObject.Find("MultiPlay");
        history = GameObject.Find("History");
        setting = GameObject.Find("Setting");
        credit = GameObject.Find("Credit");

        InitializeDifficultyLevelButton();

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
            if(menu==null) continue;
            menu.gameObject.SetActive(false);
        }
    }

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
        GameModeManager.GameModemanagerInstance.SetGameMode(GameModeManager.GameMode.PileUp);
    }

    public void ChangeDifficultyLevel(int diffLevel)
    {
        GameModeManager.GameModemanagerInstance.ChangeDifficultyLevel((GameModeManager.DifficultyLevel)diffLevel);
        for(int i=0; i<3; i++)
        {
            if(i == diffLevel)
            {
                ChangeButtonColor_Selected(difficultyLevelButtons[i]);
            }
            else
            {
                ChangeButtonColor_Unselected(difficultyLevelButtons[i]);
            }
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

    void InitializeDifficultyLevelButton()
    {
        if (setting == null) return; //ê›íËâÊñ Ç≈Ç»Ç¢èÍçáÅAèàóùÇçsÇÌÇ»Ç¢ÅB
        difficultyLevelButtons[0] = GameObject.Find("NormalButton").GetComponent<Button>();
        difficultyLevelButtons[1] = GameObject.Find("DifficultButton").GetComponent<Button>();
        difficultyLevelButtons[2] = GameObject.Find("InsaneButton").GetComponent<Button>();
        ChangeDifficultyLevel((int)GameModeManager.GameModemanagerInstance.NowDifficultyLevel);
    }
}