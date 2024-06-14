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

    public bool NowConditionChecking => NowConditionChecking;

    // Start is called before the first frame update
    void Start()
    {
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        criteriaMetProcessor = GameObject.Find("CriteriaMetProcessor").GetComponent<CriteriaMetProcessor>();
    }


    /// <summary>
    /// グラフ全体に条件にマッチするものがないかを探索するためのメソッド
    /// 条件に存在する素数のうち、ネットワーク全体で最小個数の素数を探し、そのノードを条件のチェックを行うキューであるstartExpandNetworksに追加
    /// </summary>
    void CheckConditionBlocksGraph()
    {
        if (!NewConditionGenerating) return;

        int minNode = -1; //最小個数の素数
        int minNodeNum = int.MaxValue; //最小個数の素数の数

        //条件に存在する素数を全探索し、最小個数のものを探す
        foreach (int conditionKey in conditionManager.ConditionNumberDict.Keys)
        {
            if (minNodeNum > BlocksDict[conditionKey].Count)
            {
                minNodeNum = BlocksDict[conditionKey].Count;
                minNode = conditionKey;
            }
        }
        //最小個数の素数はすでに求まっているので、それに対してfor分を回して探索を行う
        foreach (var node in BlocksDict[minNode])
        {
             EnqueueStartExpandNetworks(new ExpandNetwork(null, node, conditionManager.ConditionNumberDict));
        }
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
    /// </summary>
    /// <param name="currentNetwork">現在のExpandNetwork</param>
    public void ExpandAndSearch(ExpandNetwork currentNetwork)
    {
        //拡張したネットワークが条件を満たしていたら
        if (ContainsAllRequiredNodes(currentNetwork.MyNetwork, conditionManager.ConditionNumberDict))
        {
            //条件生成時に既に条件を達成していた場合→条件を再生成して再び調査。
            if (NewConditionGenerating)
            {
                conditionManager.GenerateCondition(); //条件を生成し
                CheckConditionBlocksGraph(); //その条件がすでに達成していないかチェック。達成したら再度ここの条件分岐に入るので、もう一度生成される。
                Debug.Log("再生成");
                return;
            }
            Debug.Log(string.Join(", ", currentNetwork.MyNetwork));
            CompleteConditionsProcess(currentNetwork.MyNetwork);
            return;
        }

        //現在拡張中のネットワークに存在する各ノードの隣接ノードを探索
        foreach (var node in currentNetwork.MyNetwork)
        {
            //今見ているノードに隣接するノードを全てリストに追加
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            //現在のネットワークとclosedListに含まれていないノードのみを選択
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.ClosedList.Contains(n) && !currentNetwork.MyNetwork.Contains(n)).ToList();

            //隣接する新しいノードがなければスキップ
            if (adjacentNodes.Count == 0) continue;

            //今見ているノードに隣接するノードを探索
            foreach (var adjacentNode in adjacentNodes)
            {
                if (adjacentNode.gameObject.GetComponent<BlockInfo>().enabled == false) continue; //ネットワークから切り離されてblockinfoがなくなったブロックがadjacentNodesに含まれれば、処理をスキップ
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, conditionManager.ConditionNumberDict); //拡張

                //場合によっては拡張前に戻る
                if (newNetwork.BackFlag) newNetwork = newNetwork.Beforenetwork;

                //再帰呼び出し
                ExpandAndSearch(newNetwork);
            }
        }
    }

    //条件を満たしたときの処理
    void CompleteConditionsProcess(List<GameObject> nodes)
    {
        switch (GameModeManager.Ins.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                criteriaMetProcessor.ProcessFreeze(nodes);
                break;
            case GameModeManager.GameMode.Battle:
                criteriaMetProcessor.ProcessFreeze(nodes);
                break;
        }
        //後処理
        ClearStartExpandNetworks(); //探索が完了したらもうネットワーク内に条件を満たすものが存在しないと考えられるので、キューをリセットしておく。(あるとバグが発生する)
        SetConditionChecking(true); //条件を達成したため、新しい条件を生成するフェーズにはいる
        CheckConditionBlocksGraph();
        conditionManager.GenerateCondition();
    }
}
