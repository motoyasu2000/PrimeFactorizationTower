using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode
    {
        PileUp, //�ςݏグ���[�h
    }
    GameMode nowGameMode;
    public GameMode NowGameMode => nowGameMode;
    void Start()
    {
        SetGameMode(GameMode.PileUp); //��U���s���ɐςݏグ���[�h�ɂ��Ă����B
    }

    public void SetGameMode(GameMode newGameMode)
    {
        nowGameMode = newGameMode;
    }
}
