using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ネットワーク全体のデータとそれを操作するメソッドを定義した静的なクラス
/// </summary>
public static class BlocksGraphData
{
    /// 新たな条件を生成するフェーズであることを表す。 条件を達成してから、新たに条件を生成しおえるまでの間はtrueになる
    static bool newConditionGenerating = false;

    //ネットワークの構造や基本機能に使用するもの
    static List<GameObject> wholeGraph; //全ノードのリスト、ネットワーク全体
    static Dictionary<int, List<GameObject>> blocksDict; //ネットワーク全体を表す素数ごとのブロックのリスト
    static Queue<ExpandNetwork> startExpandNetworks; //ネットワークの拡張を開始する最初のサブネットワークをリストとして保存しておく。非同期の処理を一つずつ実行するため、タプルの２つ目の要素は条件の辞書

    /// <summary>
    /// 新たな条件を生成するフェーズであることを表す。 条件を達成してから、新たに条件を生成しおえるまでの間はtrueになる
    /// </summary>
    public static bool NewConditionGenerating => newConditionGenerating;

    /// <summary>
    /// ネットワーク全体を表す素数ごとのブロックのリスト
    /// </summary>
    public static List<GameObject> WholeGraph => wholeGraph;
    /// <summary>
    /// ネットワーク全体を表す素数ごとのブロックのリスト
    /// </summary>
    public static Dictionary<int, List<GameObject>> BlocksDict => blocksDict;

    /// <summary>
    /// 条件の探索を始めるExpandNetworkを保持するキュー
    /// </summary>
    public static Queue<ExpandNetwork> StartExpandNetworks => startExpandNetworks;

    /// <summary>
    /// ネットワーク情報の初期化
    /// </summary>
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

    /// <summary>
    /// ネットワークから特定のブロックを取り除く処理
    /// ネットワーク全体を表すリストと辞書から、単一のブロックを消去する
    /// </summary>
    /// <param name="removeBlock">ネットワークから取り除きたいブロック</param>
    public static void BlocksGraphRemoveBlock(GameObject removeBlock)
    {
        WholeGraphRemove(removeBlock);
        BlocksDictRemoveBlock(removeBlock);
    }

    static void WholeGraphRemove(GameObject removeBlock)
    {
        wholeGraph.Remove(removeBlock);
    }
    static void BlocksDictRemoveBlock(GameObject removeBlock) 
    {
        blocksDict[removeBlock.GetComponent<BlockInfo>().GetPrimeNumber()].Remove(removeBlock);
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
    /// ブロック全体のネットワークにノードの追加、wholeGraphとblocksDictの更新を行う。
    /// </summary>
    /// <param name="block">新たに追加するブロック</param>
    public static void AddBlock(GameObject block)
    {
        WholeGraphAdd(block);
        BlockInfo info = block.GetComponent<BlockInfo>();
        if(!System.Array.Exists(GameModeManager.Ins.PrimeNumberPool, element => element == info.GetPrimeNumber()))
        {
            Debug.LogError("素数定義外のノードが定義されようとしています。");
        }
        BlocksDict[info.GetPrimeNumber()].Add(block);
    }

    /// <summary>
    /// ノードごとに隣接するノードの状態を保持しているが、お互いの隣接関係を無くし、切り離す
    /// 引数にはどのノードとどのノードの隣接関係をなくすのかを指定する。
    /// </summary>
    public static void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info1.RemoveNeighborBlock(node2);
        info2.RemoveNeighborBlock(node1);
    }

    /// <summary>
    /// ノードごとに隣接するノードの状態を保持しているが、お互いの隣接関係を追加し、くっつける
    /// 引数にどのノードとどのノードの隣接関係を追加するのかを指定する。
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    public static void AttachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info1.AddNeighborBlock(node2);
        info2.AddNeighborBlock(node1);
    }

    /// <summary>
    /// 新たな条件を生成するフェーズであるnewConditionGeneratingを書き換える。 条件を達成してから、新たに条件を生成しおえるまでの間のみtrueになるように設定する必要がある
    /// </summary>
    public static void SetConditionGenerating(bool flag)
    {
        newConditionGenerating = flag;
    }

}
