using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// このネットワークを拡張していきpatternにマッチしたgraphを探索する。
/// 拡張するとは拡張されたインスタンスを生成することで、後退するとはbeforeNetworkに戻り、戻る前に追加していたリストをクローズドリストに追加する。
/// ネットワーク内に特定のパターンがあるかどうか見つけるための、拡張されていく特殊なデータ構造
/// 特定のサブグラフを探索するための独自のアルゴリズムに使用する
/// </summary>

public class ExpandNetwork
{
    List<GameObject> network = new List<GameObject>(); //現在のネットワーク情報
    List<GameObject> closedNodes = new List<GameObject>(); //現在のネットワークに対するクローズドリスト。これより小さいクローズドリストの情報も引き継ぐ。

    public List<GameObject> Network => network;
    public List<GameObject> ClosedNodes => closedNodes;

    ExpandNetwork beforeNetwork; //ひとつ前のネットワークに戻ることがあるので必要
    public ExpandNetwork Beforenetwork => beforeNetwork;
    bool backFlag = false;
    public bool BackFlag => backFlag;

    /// <summary>
    /// コンストラクタ。拡張元となるネットワークと、そのネットワークに追加したいノードを指定する。
    /// </summary>
    /// <param name="originNetwork">元となるネットワーク</param>
    /// <param name="addNode">ネットワークに追加するノード</param>
    /// <param name="condition">現在のcondition</param>
    public ExpandNetwork(ExpandNetwork originNetwork, GameObject addNode, Dictionary<int, int> condition)
    {
        if (addNode == null) Debug.LogError("ExpandNetworkに追加しようとしたノードがnullです");

        if (originNetwork != null)
        {
            TakeOverBeforeNetwork(originNetwork);
            originNetwork.closedNodes.Add(addNode); //一個前のネットワークに今追加したノードをクローズドリストに追加する。
        }

        AddNewNode(addNode, condition);

        closedNodes.Add(addNode);
    }

    /// <summary>
    /// 今回生成するネットワークに、元となるネットワークの情報を引き継ぐ (closedListとnetworkを引継ぎ、beforeNetworkに元となるネットワークを追加)
    /// </summary>
    /// <param name="originNetwork">元となるネットワーク</param>
    void TakeOverBeforeNetwork(ExpandNetwork originNetwork)
    {
        closedNodes = new List<GameObject>(originNetwork.closedNodes);
        network = new List<GameObject>(originNetwork.network);
        beforeNetwork = originNetwork;
    }

    /// <summary>
    /// 現在のネットワークに新たなノードを追加する
    /// </summary>
    /// <param name="addNode">追加するノード</param>
    /// <param name="condition">現在のcondition</param>
    void AddNewNode(GameObject addNode, Dictionary<int, int> condition)
    {
        //追加予定のノードの数値
        int addNodeValue = addNode.GetComponent<BlockInfo>().GetPrimeNumber();
        //現在のネットワークの個数
        int currentCount = network.Count(node => node.GetComponent<BlockInfo>().GetPrimeNumber() == addNodeValue);
        //conditionで要求される個数
        int requiredCount = 0;

        //conditionの条件として追加予定のノードが含まれており、ノードを追加しても要求された個数を上回らないなら、追加
        if (condition.TryGetValue(addNodeValue, out requiredCount) && currentCount < requiredCount)
        {
            network.Add(addNode);
        }
        //条件を満たさなければ戻る
        else
        {
            backFlag = true;
        }
    }
}