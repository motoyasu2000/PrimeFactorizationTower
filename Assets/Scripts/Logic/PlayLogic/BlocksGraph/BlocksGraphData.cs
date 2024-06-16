using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ネットワーク全体のデータとそれを操作するメソッドと定義した静的なクラス
/// </summary>
public static class BlocksGraphData
{
    static bool newConditionGenerating; //新たな条件を生成するフェーズであることを表す。 条件を達成してから、新たに条件を生成しおえるまでの間はtrueになる
    public static bool NewConditionGenerating => newConditionGenerating;

    //ネットワークの構造や基本機能に使用するもの
    static List<GameObject> wholeGraph; //全ノードのリスト、ネットワーク全体
    static Dictionary<int, List<GameObject>> blocksDict; //ネットワーク全体を表す素数ごとのブロックのリスト
    static Queue<ExpandNetwork> startExpandNetworks; //ネットワークの拡張を開始する最初のサブネットワークをリストとして保存しておく。非同期の処理を一つずつ実行するため、タプルの２つ目の要素は条件の辞書
    public static List<GameObject> WholeGraph => wholeGraph;
    public static Dictionary<int, List<GameObject>> BlocksDict => blocksDict;
    public static Queue<ExpandNetwork> StartExpandNetworks => startExpandNetworks;

    public static void InitializeBlocksGraph()
    {
        newConditionGenerating = false;
        wholeGraph = new List<GameObject>();
        blocksDict = new Dictionary<int, List<GameObject>>();
        startExpandNetworks = new Queue<ExpandNetwork>();
        foreach (var value in GameModeManager.Ins.PrimeNumberPool)
        {
            blocksDict.Add(value, new List<GameObject>());
        }
    }

    public static void WholeGraphAdd(GameObject block)
    {
        wholeGraph.Add(block);
    }

    public static void WholeGraphRemove(GameObject block)
    {
        wholeGraph.Remove(block);
    }
    public static void BlocksDictRemoveSingleBlock(GameObject singleBlock) 
    {
        blocksDict[singleBlock.GetComponent<BlockInfo>().GetPrimeNumber()].Remove(singleBlock);
    }

    public static ExpandNetwork DequeueStartExpandNetworks()
    {
        return startExpandNetworks.Dequeue();
    }

    public static void EnqueueStartExpandNetworks(ExpandNetwork newNetwork)
    {
        startExpandNetworks.Enqueue(newNetwork);
    }

    public static void ClearStartExpandNetworks()
    {
        startExpandNetworks.Clear();
    }

    /// <summary>
    /// ノードの追加、wholeGraphとblocksDictの更新を行う。
    /// </summary>
    /// <param name="block">新たに追加するブロック</param>
    public static void AddBlock(GameObject block)
    {
        WholeGraphAdd(block);
        BlockInfo info = block.GetComponent<BlockInfo>();
        if (System.Array.Exists(GameModeManager.Ins.PrimeNumberPool, element => element == info.GetPrimeNumber()))
        {
            if (!BlocksDict.ContainsKey(info.GetPrimeNumber()))
            {
                BlocksDict.Add(info.GetPrimeNumber(), new List<GameObject>());
            }
            BlocksDict[info.GetPrimeNumber()].Add(block);
        }
        else
        {
            Debug.LogError("素数定義外のノードが定義されようとしています。");
        }                                                              
    }

    //エッジの更新を行う(削除)
    public static void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info1.RemoveNeighborBlock(node2);
        info2.RemoveNeighborBlock(node1);
    }

    //エッジの更新を行う(追加)
    public static void AttachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info1.AddNeighborBlock(node2);
        info2.AddNeighborBlock(node1);
    }

    public static void SetConditionGenerating(bool flag)
    {
        newConditionGenerating = flag;
    }

}
