using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//各シングルトンを生成するためのクラス。このスクリプトがアタッチされたゲームオブジェクトがシーン内にあるだけで、必要なインスタンスが無ければすべて生成されるようにする。
public class SingletonGenerator : MonoBehaviour
{
    private void Awake()
    {
        if (GameModeManager.GameModemanagerInstance == null)
        {
            GameObject gameModeManager = new GameObject("GameModeManager");
            GameModeManager gameModeManagerScript = gameModeManager.AddComponent<GameModeManager>();
            gameModeManagerScript.enabled = true;
        }
        if(SoundManager.SoundManagerInstance == null)
        {
            Instantiate(Resources.Load("SoundManager")); //サウンドマネージャーは子要素もあるのでゲームオブジェクトをアタッチするだけでは足りない。
        }
        if(ScoreManager.ScoreManagerInstance == null)
        {
            GameObject scoreManager = new GameObject("ScoreManager");
            ScoreManager scoreManagerScript =  scoreManager.AddComponent<ScoreManager>();
            scoreManagerScript.enabled = true;
        }
    }
}
