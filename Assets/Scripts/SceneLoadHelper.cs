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
        SoundManager soundManager = SoundManager.SoundManagerInstance;
        if (sceneName == "PlayScene") soundManager.PlayAudio(SoundManager.SoundManagerInstance.BGM_PLAY);
        else if (sceneName == "TitleScene") soundManager.PlayAudio(SoundManager.SoundManagerInstance.BGM_TITLE);
    }
}
