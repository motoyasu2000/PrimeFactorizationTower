using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGenerator : MonoBehaviour
{
    private void Awake()
    {
        //もし、ゲームモードマネージャーのstaticインスタンスが存在しなければ、ゲームモードマネージャーがアタッチされたインスタンスを生成し、初期化を行う。
        if (GameModeManager.GameModemanagerInstance == null)
        {
            GameObject gameModeManager = new GameObject("GameModeManager");
            gameModeManager.AddComponent<GameModeManager>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
