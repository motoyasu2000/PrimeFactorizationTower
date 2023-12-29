using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadHelper
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SoundManager.LoadSoundData();
        if (sceneName == "PlayScene") SoundManager.SoundManagerInstance.PlayAudio(SoundManager.SoundManagerInstance.BGM_PLAY);
        else Debug.Log(sceneName);
    }
}
