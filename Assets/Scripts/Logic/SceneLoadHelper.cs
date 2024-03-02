using UnityEngine.SceneManagement;

//シーンをロードする際に呼び出す静的クラス。通常のシーンのロードに比べ、再生するBGMの管理も行う。
public static class SceneLoadHelper
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SoundManager.LoadSoundSettingData();
        SoundManager soundManager = SoundManager.SoundManagerInstance;
        if (sceneName == "PlayScene") soundManager.PlayAudio(SoundManager.SoundManagerInstance.BGM_PLAY);
        else if (sceneName == "TitleScene") soundManager.PlayAudio(SoundManager.SoundManagerInstance.BGM_TITLE);
    }
}
