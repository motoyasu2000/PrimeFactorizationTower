using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject singlePlay;
    [SerializeField] GameObject multiPlay;
    [SerializeField] GameObject setting;
    [SerializeField] GameObject credit;
    GameObject[] menus = new GameObject[4];

    [SerializeField] Scene playScene;

    void Awake()
    {
        singlePlay = GameObject.Find("SinglePlay");
        multiPlay = GameObject.Find("MultiPlay");
        setting = GameObject.Find("Setting");
        credit = GameObject.Find("Credit");

        menus[0] = singlePlay;
        menus[1] = multiPlay;
        menus[2] = setting;
        menus[3] = credit;
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

    public void MovePlayScene()
    {
        SceneManager.LoadScene("PlayScene");
        GameModeManager.SetGameMode(GameModeManager.GameMode.PileUp);
    }

    public void ChangeDifficultyLevel(int diffLevel)
    {
        GameModeManager.ChangeDifficultyLevel((GameModeManager.DifficultyLevel)diffLevel);
    }
}
