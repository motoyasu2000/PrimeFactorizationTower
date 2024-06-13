using System.Collections.Generic;
using UnityEngine;
using Common;

//ターンを管理するクラス
public static class TurnMangaer
{
    static int nowTurn = 0;
    static int maxTurn = 0;
    static List<string> names = new List<string>();
    static int totalTurn = 0;

    //プレイヤーの名前を設定する
    public static void SetPlayerNames(List<string> playerNames)
    {
        names = playerNames;
        maxTurn = names.Count - 1;
    }

    //現在のターンの人の名前を取得する
    public static string GetPlayerNames_NowTurn()
    {
        return names[nowTurn];
    }

    //以前のターンの人の名前を取得する
    public static string GetPlayerNames_BeforeTurn()
    {
        if(nowTurn - 1 < 0)
        {
            return names[maxTurn];
        }
        else
        {
            return names[nowTurn - 1];
        }
    }

    //次のターンに進む
    public static void ChangeNextTurn()
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
        //Debug.Log(GetNowTurn());
    }

    //現在のターンを取得
    public static int GetNowTurn()
    {
        return nowTurn;
    }

    public static int GetTotalTurn()
    {
        return totalTurn;
    }
}
