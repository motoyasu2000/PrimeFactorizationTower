using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapEffectSceneLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("TapEffectScene", LoadSceneMode.Additive);
    }
}
