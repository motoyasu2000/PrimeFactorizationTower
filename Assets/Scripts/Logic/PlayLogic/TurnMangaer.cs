using System.Collections.Generic;
using UnityEngine;
using Common;

//ターンを管理するクラス
public static class TurnMangaer
{
    static int nowNamesIndex = 0; //今何番目の人か
    static int maxNamesIndex = 0; //何番目の人までいるか
    static int totalTurn = 0;
    static List<string> names = new List<string>(); //すべての名前を保存するリスト

    //プレイヤーの名前を設定する
    public static void SetPlayerNames(List<string> playerNames)
    {
        names = playerNames;
        maxNamesIndex = names.Count-1;
        if (names.Count == 0) Debug.LogError("人が一人もいません。");
    }

    //現在のターンの人の名前を取得する
    public static string GetPlayerNames_NowTurn()
    {
        return names[nowNamesIndex];
    }

    //以前のターンの人の名前を取得する
    public static string GetPlayerNames_BeforeTurn()
    {
        if(nowNamesIndex - 1 < 0)
        {
            return names[maxNamesIndex];
        }
        else
        {
            return names[nowNamesIndex - 1];
        }
    }

    //次のターンに進む
    public static void ChangeNextTurn()
    {
        totalTurn++;
        if (nowNamesIndex < maxNamesIndex)
        {
            nowNamesIndex++;
        }
        else if(nowNamesIndex == maxNamesIndex)
        {
            nowNamesIndex = 0;
        }
        else
        {
            Debug.LogError("turn数が異常値になりました。");
            Debug.LogError($"現在のターン数{nowNamesIndex} 上限ターン数{maxNamesIndex}");
            nowNamesIndex = 0;
        }
        //Debug.Log(GetNowNamesIndex());
    }

    //現在のターンを取得
    public static int GetNowNamesIndex()
    {
        return nowNamesIndex;
    }

    public static int GetTotalTurn()
    {
        return totalTurn;
    }
}
