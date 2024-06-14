using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全てのブロックを管理するクラス。現状は素数とその素数に対応するブロックの紐づけを行っている。
/// </summary>
public class AllBlocksManager : MonoBehaviour
{
    //素数とその素数に対応するブロックの紐づけ
    Dictionary<int, GameObject> blocksDict = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> BlocksDict => blocksDict;
    void Awake()
    {
        InitializeBlocksDict();
    }

    /// <summary>
    /// 引数で指定した素数を持つブロックをResourcesからロード
    /// </summary>
    /// <param name="primeNumber">どの素数か</param>
    /// <returns>引数で指定した素数に対応するブロック</returns>
    GameObject LoadBlock(int primeNumber)
    {
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }

    void InitializeBlocksDict()
    {
        //全ての素数に対して、ロードの処理
        foreach (int primeNumber in GameModeManager.Ins.PrimeNumberPool)
        {
            GameObject tmpBlock = LoadBlock(primeNumber);
            if (tmpBlock == null) continue;
            
            blocksDict[primeNumber] = tmpBlock; //もし、指定した番号のブロックが存在すれば辞書を更新
        }
    }
}
