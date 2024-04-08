﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurnMangaer
{
    static int nowTurn = 0;
    static int maxTurn = 0;
    static Dictionary<int, string> playerNamesDict = new Dictionary<int, string>();
    static int totalTurn = 0;

    public static void SetNumberOfPlayer(int numberOfPleyer)
    {
        if (numberOfPleyer <= 0) Debug.LogError("プレイヤーは1人以上必要です");
        maxTurn = numberOfPleyer - 1;
    }

    public static void SetPlayerNames(List<string> playerNames)
    {
        playerNamesDict = new Dictionary<int, string>(); //初期化
        for(int i=0; i<=maxTurn; i++)
        {
            playerNamesDict[i] = playerNames[i];
        }
    }
    public static string GetPlayerNames_NowTurn()
    {
        return playerNamesDict[nowTurn];
    }
    public static string GetPlayerNames_BeforeTurn()
    {
        if(nowTurn - 1 < 0)
        {
            return playerNamesDict[maxTurn];
        }
        else
        {
            return playerNamesDict[nowTurn - 1];
        }
    }

    public static void NextTurn()
    {
        totalTurn++;
        if (nowTurn < maxTurn)
        {
            nowTurn++;
        }
        else if(nowTurn == maxTurn)
        {
            nowTurn = 0;
        }
        else
        {
            Debug.LogError("turn数が異常値になりました。");
            Debug.LogError($"現在のターン数{nowTurn} 上限ターン数{maxTurn}");
            nowTurn = 0;
        }
        Debug.Log(GetNowTurn());
    }

    public static int GetNowTurn()
    {
        return nowTurn;
    }
}
