using UnityEngine;

//各シングルトンを生成するためのクラス。このスクリプトがアタッチされたゲームオブジェクトがシーン内にあるだけで、必要なインスタンスが無ければすべて生成されるようにする。
public class SingletonGenerator : MonoBehaviour
{
    private void Awake()
    {
        if (GameModeManager.Ins == null)
        {
            GameObject gameModeManager = new GameObject("GameModeManager");
            GameModeManager gameModeManagerScript = gameModeManager.AddComponent<GameModeManager>();
            gameModeManagerScript.enabled = true;
        }
        if(SoundManager.Ins == null)
        {
            Instantiate(Resources.Load("SoundManager")); //サウンドマネージャーは子要素もあるのでゲームオブジェクトをアタッチするだけでは足りない。
        }
        if(ScoreManager.Ins == null)
        {
            GameObject scoreManager = new GameObject("ScoreManager");
            ScoreManager scoreManagerScript =  scoreManager.AddComponent<ScoreManager>();
            scoreManagerScript.enabled = true;
        }
        if(PlayerInfoManager.Ins == null)
        {
            GameObject playerInfoManager = new GameObject("PlayerInfoManager");
            PlayerInfoManager playerInfoManagerScript = playerInfoManager.AddComponent<PlayerInfoManager>();
            playerInfoManagerScript.enabled = true;
        }
    }
}
