using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode
    {
        PileUp, //積み上げモード
    }
    GameMode nowGameMode;
    public GameMode NowGameMode => nowGameMode;
    void Start()
    {
        SetGameMode(GameMode.PileUp); //一旦実行時に積み上げモードにしておく。
    }

    public void SetGameMode(GameMode newGameMode)
    {
        nowGameMode = newGameMode;
    }
}
