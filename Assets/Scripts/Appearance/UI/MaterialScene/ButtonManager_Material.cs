using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager_Material : MonoBehaviour
{
    public void MoveTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
