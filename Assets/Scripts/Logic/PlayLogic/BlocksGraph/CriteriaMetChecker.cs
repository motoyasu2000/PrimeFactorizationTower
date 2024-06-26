using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BlocksGraphData;

/// <summary>
/// 条件が満たしているのかチェックするクラス
/// </summary>
public class CriteriaMetChecker : MonoBehaviour
{
    //条件の生成
    ConditionManager conditionManager;
    CriteriaMetProcessor criteriaMetProcessor;

    // Start is called before the first frame update
    void Start()
    {
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        criteriaMetProcessor = GameObject.Find("CriteriaMetProcessor").GetComponent<CriteriaMetProcessor>();
    }

    /// <summary>
    /// グラフ全体に条件にマッチするものがないかを探索する
    /// 条件に存在する素数のうち、ネットワーク全体で最小個数の素数を探し、そのノードを条件のチェックを行うキューであるstartExpandNetworksに追加
    /// conditionの生成時に、conditionに存在しないグラフを生成するために使用する。
    /// </summary>
    void CheckConditionBlocksGraph()
    {
        if (!NewConditionGenerating) return; //条件のチェック中でなければ以降の処理を飛ばす
        int minCountPrime = SearchMinCountPrime();

        //最小個数の素数に対してfor分を回して探索を行う
        foreach (var node in NodesDict[minCountPrime])
        {
             EnqueueStartExpandNetworks(new ExpandNetwork(null, node, conditionManager.ConditionNumberDict));
        }
    }

    /// <summary>
    /// グラフから最小個数の素数を見つけて返す
    /// </summary>
    /// <returns>グラフ上の最小個数の素数</returns>
    int SearchMinCountPrime()
    {
        int minCountPrime = -1; //最小個数の素数
        int minCount = int.MaxValue; //最小個数の素数の数

        //条件に存在する素数を全探索し、最小個数のものを探す
        foreach (int conditionKey in conditionManager.ConditionNumberDict.Keys)
        {
            if (minCount > NodesDict[conditionKey].Count)
            {
                minCount = NodesDict[conditionKey].Count;
                minCountPrime = conditionKey;
            }
        }

        return minCountPrime;
    }

    //第二引数で指定した条件を満たすサブグラフの条件を、第一引数で指定したネットワークが満たしているかをチェックするメソッド
    bool ContainsAllRequiredNodes(List<GameObject> blocksGraph, Dictionary<int, int> requiredNodesDict)
    {
        Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);

        //現在のネットワーク内のノードの出現回数をカウント
        foreach (var node in blocksGraph)
        {
            BlockInfo blockInfo = node.GetComponent<BlockInfo>();
            int nodeValue = blockInfo.GetPrimeNumber();
            if (requiredCounts.ContainsKey(nodeValue))
            {
                requiredCounts[nodeValue]--;
                if (requiredCounts[nodeValue] == 0)
                    requiredCounts.Remove(nodeValue);
            }
            //もしサブネットワーク内に関係のない数値があればfalseを返す
            else
            {
                return false;
            }
        }

        return requiredCounts.Count == 0; //必要なノードがすべて含まれていればtrue
    }

    /// <summary>
    /// ネットワークを拡張しながらサブグラフを探索する独自のアルゴリズム
    /// https://www.youtube.com/watch?v=syBXS7UtUN0&t=3s
    /// </summary>
    /// <param name="currentNetwork">現在のExpandNetwork</param>
    public void ExpandAndSearch(ExpandNetwork currentNetwork)
    {
        //拡張したネットワークが条件を満たしていたら
        if (ContainsAllRequiredNodes(currentNetwork.Network, conditionManager.ConditionNumberDict))
        {
            //条件生成時に既に条件を達成していた場合→条件を再生成して再び調査。
            if (NewConditionGenerating)
            {
                conditionManager.GenerateCondition(); //条件を生成し
                CheckConditionBlocksGraph(); //その条件がすでに達成していないかチェック。達成したら再度ここの条件分岐に入るので、もう一度生成される。
                Debug.Log("再生成");
                return;
            }
            //ネットワークから条件を満たし、条件を探索するフェーズの場合には、条件を満たした時に実行する処理をまとめたProcessCriteriaMetを実行
            else
            {
                Debug.Log(string.Join(", ", currentNetwork.Network));
                ProcessCriteriaMet(currentNetwork.Network);
                return;
            }
        }

        //現在拡張中のネットワークに存在する各ノードの隣接ノードを探索
        foreach (GameObject node in currentNetwork.Network)
        {
            //今見ているノードに隣接しているノードのうち、拡張できそうなノードを取得する
            var adjacentNodes = GetValidAdjacentNode(currentNetwork, node);

            //求めたすべてのadjacentNodesについて、拡張を試みる
            foreach (GameObject adjacentNode in adjacentNodes)
            {
                //拡張し、場合によっては拡張に前に戻り、また探索を行う。
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, conditionManager.ConditionNumberDict);
                if (newNetwork.BackFlag) newNetwork = newNetwork.Beforenetwork;
                ExpandAndSearch(newNetwork);
            }
        }
    }

    /// <summary>
    /// 条件を満たしたときに行う処理
    /// </summary>
    /// <param name="nodes"></param>
    void ProcessCriteriaMet(List<GameObject> nodes)
    {
        criteriaMetProcessor.ProcessCriteriaMet(nodes);
        //探索が完了したらもうネットワーク内に条件を満たすものが存在しないと考えられるので、キューをリセットしておく。
        //(あるとGameInfoが消されたゲームオブジェクトが残り続けることになるためバグが発生する)
        ClearStartExpandNetworks(); 
        SetConditionGenerating(true); //条件を達成したため、新しい条件を生成するフェーズにはいる
        CheckConditionBlocksGraph();
        conditionManager.GenerateCondition();
    }

    /// <summary>
    /// 条件を探索するExpandNetworkに追加可能できる可能性のあるノードのリストを返す。
    /// adjacentNodesのうち、closedNodesに含まれておらず、currentNetwork上にないものを計算して返す
    /// </summary>
    /// <param name="currentNetwork">現在拡張中のネットワーク</param>
    /// <param name="adjacentNodes">現在見ているノードに隣接しているもの、ここから必要なノードのみを抽出する。</param>
    /// <returns>ExpandNetworkに追加可能できる可能性のあるノードのリスト</returns>
    List<GameObject> GetValidAdjacentNode(ExpandNetwork currentNetwork, GameObject node)
    {
        List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();
        return
        adjacentNodes = adjacentNodes.Where(n =>
        !currentNetwork.ClosedNodes.Contains(n) && 
        !currentNetwork.Network.Contains(n)).ToList();
    }
}
