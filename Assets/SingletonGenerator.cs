using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGenerator : MonoBehaviour
{
    private void Awake()
    {
        if (GameModeManager.GameModemanagerInstance == null && FindObjectOfType<GameModeManager>() == null)
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
