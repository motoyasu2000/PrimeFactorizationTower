using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleExplainMenu : MonoBehaviour
{
    int toggleCounter = 0;
    int overCount = -1;
    void Awake()
    {
        overCount = transform.childCount;
    }

    //このメソッドが呼ばれると、ひとつ前のメニューを非表示にして、現在のメニューを表示する。
    public void Toggle()
    {
        if (toggleCounter >= 1) transform.GetChild(toggleCounter - 1).gameObject.SetActive(false);

        if (toggleCounter >= overCount)
        {
            gameObject.SetActive(false);
            toggleCounter = 0;
        }
        else
        {
            transform.GetChild(toggleCounter).gameObject.SetActive(true);
            toggleCounter++;
        }
    }

    private void OnEnable()
    {
        Toggle();
    }
}
