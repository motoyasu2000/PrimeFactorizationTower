using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameinfoのAILearningがtrueの場合に、AIが学習するための環境を整えるクラス
public class AILearningModeChanger : MonoBehaviour
{
    Camera UICamera;

    void Start()
    {
        //AI学習時のみ、AI学習環境用に初期化を行う。
        if (GameInfo.AILearning)
        {
            UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            ChangeAILearningMode();
        }
    }

    void ChangeAILearningMode()
    {
        StopUICamera();
        SetAITurnOnly();
    }

    void StopUICamera()
    {
        UICamera.enabled = false;
    }

    void SetAITurnOnly()
    {
        GameModeManager.Ins.SetGameMode(GameModeManager.GameMode.Battle); //ゲームモードを非シングルに
        TurnSetter.Ins.SetNames_AILearning();//AIのみにすることで、AIのみで学習ができるようにする。
    }
}
