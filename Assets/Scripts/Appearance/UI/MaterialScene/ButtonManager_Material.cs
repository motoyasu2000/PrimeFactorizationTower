using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

public class ButtonManager_Material : MonoBehaviour
{
    public void MoveTitleScene()
    {
        Helper.LoadScene("TitleScene");
    }
}
