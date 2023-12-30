using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager_Play : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject settingMenu;
    [SerializeField] GameObject backMenu;
    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        settingMenu = canvas.transform.Find("SettingMenu").gameObject;
        backMenu = canvas.transform.Find("BackMenu").gameObject;
    }

    public void DisplaySettingMenu(bool isDisplay)
    {
        settingMenu.SetActive(isDisplay);
    }

    public void DisplayBackMenu(bool isDisplay)
    {
        backMenu.SetActive(isDisplay);
    }
    public void MoveTitleScene()
    {
        SceneLoadHelper.LoadScene("TitleScene");
    }

}