using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 }; //素数配列
    [SerializeField] List<GameObject> allNodes = new List<GameObject>(); //全ノードのリスト
    [SerializeField] Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>(); //各ノードがいくつあるのかを格納したリスト
    List<GameObject> subNodes = new List<GameObject>(); //サブネットワーク用のリスト
    [SerializeField] Dictionary<int, List<GameObject>> subNodesDict = new Dictionary<int, List<GameObject>>(); //探索用のサブネットワークに各ノードがいくつあるのかを格納したリスト
    [SerializeField] GameModeManager gameModeManager;


    private void Start()
    {
        //nodeDictの初期化
        foreach (var value in primeNumbers)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

    //ノードの追加、allNodesとnodesDictの更新を行う。
    public void AddNode(GameObject node)
    {
        allNodes.Add(node);
        BlockInfo info = node.GetComponent<BlockInfo>();
        if (System.Array.Exists(primeNumbers, element => element == info.GetNumber()))
        {
            if (!nodesDict.ContainsKey(info.GetNumber()))
            {
                nodesDict.Add(info.GetNumber(), new List<GameObject>());
            }
            nodesDict[info.GetNumber()].Add(node);
        }
        else
        {
            Debug.LogError("素数定義外のノードが定義されようとしています。");
        }
    }

    //与えられたパターンからallnodeを切り取り、サブネットワークを作る。(subnodesとsubnodesdictの更新)
    public void CreateSubNetwork(HashSet<int> subNetPattern)
    {
        subNodes = new List<GameObject>(); //サブネットワークをリセット
        //サブグラフの作成
        foreach (GameObject mainNode in allNodes)
        {
            int mainNodeNum = mainNode.GetComponent<BlockInfo>().GetNumber();
            if (subNetPattern.Contains(mainNodeNum))
            {
                subNodes.Add(mainNode); //サブグラフのノードを更新
                RenuealSubgraphDict(mainNodeNum); //サブグラフの辞書を更新するメソッド
            }
        }
        //エッジの消去
        foreach (var subnode in allNodes)
        {
            subnode.GetComponent<BlockInfo>().DeleteMissNeighberBlock(subNetPattern);
        }

        //デバッグ用
        //foreach (var subnode in subNodes)
        //{
        //    foreach (var neighbor in subnode.GetComponent<BlockInfo>().GetNeighborEdge())
        //    {
        //        Debug.Log($"{subnode.name}-------------{neighbor.name}");
        //    }
        //}
    }

    //エッジの更新を行う(削除)
    public void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.RemoveNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.RemoveNeighborBlock(node1);
    }

    //エッジの更新を行う(追加)
    public void AttachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.AddNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.AddNeighborBlock(node1);
    }

    //ネットワークから特定のノードをDestroyするメソッド
    private void SafeDestroyNode(GameObject originNode)
    {
        SafeCutNode(originNode);
        Destroy(originNode);
    }

    //ネットワークから特定のサブネットワークをDestroyするメソッド
    private void SafeDestroyNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeDestroyNode(node);
        }
    }

    //ネットワークから特定のノードを切り離すメソッド(Destroyはしない)
    private void SafeCutNode(GameObject originNode)
    {
        //切り離し元のノードと隣接するエッジを一時的な変数に格納(コレクションがイテレーション中に変更してはならないため)
        List<GameObject> tmpNeighborNode = new List<GameObject>();
        foreach (var neighborNode in originNode.GetComponent<BlockInfo>().GetNeighborEdge())
        {
            tmpNeighborNode.Add(neighborNode);
        }
        //切り離し元のノードと隣接するエッジを実際に削除する
        foreach (var neighborNode in tmpNeighborNode)
        {
            DetachNode(neighborNode, originNode);
        }
        //ネットワークからそのノードを削除
        allNodes.Remove(originNode);
    }

    //ネットワークから特定のサブネットワークを切り離すメソッド
    private void SafeCutNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeCutNode(node);
        }
    }

    //サブネットワークの色を変更させるメソッド
    private void ChangeColorNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            node.GetComponent<SpriteRenderer>().color = Color.white;
            node.GetComponent<SpriteRenderer>().material.color = Color.white;
        }
    }

    //サブネットワークを物理的に結合し色を変更し、仮想的なネットワークから切り離すメソッド
    private void FriezeNodes(List<GameObject> nodes)
    {
        for(int i=1; i<nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
        SafeCutNodes(nodes);
        ChangeColorNodes(nodes);
    }

    //条件を満たしたときの処理
    private void CompleteConditions(List<GameObject> nodes)
    {
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                FriezeNodes(nodes);
                break;
        }
    }

    //サブグラフの最も少ない素数のキーを返す関数
    public int SearchMinNode()
    {
        int nowNodeCount = -1;
        int minNodeCount = int.MaxValue;
        int minNodeNumber = -1;
        foreach (var nodes in subNodesDict)
        {
            nowNodeCount = nodes.Value.Count;
            if (minNodeCount > nowNodeCount)
            {
                minNodeCount = nowNodeCount;
                minNodeNumber = nodes.Key;
            }
        }
        //Debug.Log("最もネットワーク内部に少ない素数" + minNodeNumber);
        return minNodeNumber;
    }

    //サブグラフの辞書を更新するメソッド
    void RenuealSubgraphDict(int mainNodeNum)
    {
        //サブグラフの各素数の辞書を更新
        if (!subNodesDict.ContainsKey(mainNodeNum)) //キーがぞんざいしないときのみ、メインのnodesDictのキーバリューのセットを入れる。allNodesに対してfor分を回しているので、重複の可能性もあるため
        {
            subNodesDict.Add(mainNodeNum, nodesDict[mainNodeNum]);
            //foreach (var pair in subNodesDict)
            //{
            //    foreach (var value in pair.Value)
            //    {
            //        Debug.Log($"{pair.Key} ： {value.name}");
            //    }
            //}

        }
        //もし存在すればサブグラフの辞書をリセットし、再生成
        else
        {
            subNodesDict = new Dictionary<int, List<GameObject>>();
            subNodesDict.Add(mainNodeNum, nodesDict[mainNodeNum]);
            //foreach (var pair in subNodesDict)
            //{
            //    foreach (var value in pair.Value)
            //    {
            //        Debug.Log($"{pair.Key} ： {value.name}");
            //    }
            //}
        }
    }

    //このネットワークを拡張していきpatternにマッチしたgraphを探索する。拡張するとは拡張されたインスタンスを生成することで、後退するとはbeforeNetworkに戻り、戻る前に追加していたリストをクローズドリストに追加する。
    class ExpandNetwork
    {
        public List<GameObject> myNetwork { get; private set; } = new List<GameObject>(); //現在のネットワーク情報
        public List<GameObject> closedList { get; private set; } = new List<GameObject>(); //現在のネットワークに対するクローズドリスト。これより小さいクローズドリストの情報も引き継ぐ。
        ExpandNetwork beforeNetwork;　//ひとつ前のネットワークに戻ることがあるので必要
        public ExpandNetwork Beforenetwork => beforeNetwork;
        bool backFlag = false;
        public bool BackFlag => backFlag;

        //コンストラクタ。呼び出す側から見て、現在のネットワークと追加したいノードを引数で指定する。
        public ExpandNetwork(ExpandNetwork beforeNetwork, GameObject nowNode, Dictionary<int, int> requiredNodesDict)
        {
            
            //beforeNetworkがある==１番目以降のノード => closedListとmyNetworkをbeforeNetworkで初期化
            if (beforeNetwork != null)
            {
                closedList = new List<GameObject>(beforeNetwork.closedList);
                myNetwork = new List<GameObject>(beforeNetwork.myNetwork);
                this.beforeNetwork = beforeNetwork;
            }
            if(beforeNetwork != null) beforeNetwork.closedList.Add(nowNode); //一個前のネットワークに今追加したノードをクローズドリストに追加する。
            // ノードが要件を満たしているか確認
            int nodeValue = nowNode.GetComponent<BlockInfo>().GetNumber();
            if (requiredNodesDict.ContainsKey(nodeValue))
            {
                int requiredCount = requiredNodesDict[nodeValue];
                int currentCount = myNetwork.Count(node => node.GetComponent<BlockInfo>().GetNumber() == nodeValue);

                // ノードが要件を満たしている場合にのみ追加
                if (currentCount < requiredCount)
                {
                    myNetwork.Add(nowNode);
                }
                //要件を満たしていなければひとつ前のネットワークに戻る。
                else
                {
                    backFlag = true;
                }
            }
            //Debug.Log(string.Join(", ", closedList));
            //Debug.Log(string.Join(", ", myNetwork));
        }

        // ネットワークに隣接ノードを追加するメソッド（重複を防ぐ）
        public void AddAdjacentNodes(List<GameObject> adjacentNodes)
        {
            foreach (var node in adjacentNodes)
            {
                if (!myNetwork.Contains(node) && !closedList.Contains(node))
                {
                    myNetwork.Add(node);
                    closedList.Add(node);
                }

            }
        }

        //パターンマッチングのロジックを修正
        public bool ContainsAllRequiredNodes(Dictionary<int, int> requiredNodesDict)
        {
            Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);
            //Debug.Log(string.Join(", ", requiredCounts));

            //現在のネットワーク内のノードの出現回数をカウント
            foreach (var node in myNetwork)
            {
                int nodeValue = node.GetComponent<BlockInfo>().GetNumber();
                if (requiredCounts.ContainsKey(nodeValue))
                {
                    requiredCounts[nodeValue]--;
                    if (requiredCounts[nodeValue] == 0)
                        requiredCounts.Remove(nodeValue);
                }
            }

            return requiredCounts.Count == 0; //必要なノードがすべて含まれていればtrue
        }
    }

    public void SearchMatchingPattern(Dictionary<int, int> requiredNodesDict, HashSet<GameObject> neiborSet)
    {
        ExpandNetwork currentNetwork = null;
        foreach (var startNode in neiborSet)
        {
            if(currentNetwork == null)
            {
                currentNetwork = new ExpandNetwork(null, startNode, requiredNodesDict);
            }
            else
            {
                currentNetwork = new ExpandNetwork(currentNetwork, startNode, requiredNodesDict);
            }
        }
        //Debug.Log(string.Join(", ",currentNetwork.myNetwork));
        //ネットワークを拡張していく処理
        ExpandAndSearch(currentNetwork, requiredNodesDict);
    }

    //ネットワークを拡張しながらサブグラフを探索する再帰的メソッド
    private void ExpandAndSearch(ExpandNetwork currentNetwork, Dictionary<int, int> requiredNodesDict)
    {
        //Debug.Log(string.Join(", ", currentNetwork));
        if (currentNetwork.ContainsAllRequiredNodes(requiredNodesDict))
        {
            Debug.Log(string.Join(", ", currentNetwork.myNetwork));

            CompleteConditions(currentNetwork.myNetwork);
            //foreach(var node in currentNetwork.myNetwork)
            //{
            //    node.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            //}

            return;
        }

        //各ノードの隣接ノードを探索
        foreach (var node in currentNetwork.myNetwork)
        {
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            //現在のネットワークとclosedListに含まれていないノードのみを選択
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.closedList.Contains(n) && !currentNetwork.myNetwork.Contains(n)).ToList();

            if (adjacentNodes.Count == 0)
            {
                continue; //隣接する新しいノードがなければスキップ
            }

            foreach (var adjacentNode in adjacentNodes)
            {
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, requiredNodesDict);
                if (newNetwork.BackFlag)
                {
                    newNetwork = newNetwork.Beforenetwork;
                }
                
                ExpandAndSearch(newNetwork, requiredNodesDict);
            }
        }
    }

    


    private void Update()
    {
        //Debug.Log(subNodesDict.Count);
        //foreach (var nodes in nodesDict)
        //{
        //    Debug.Log($"number: {nodes.Key}  nodesCount:{nodes.Value.Count}");
        //}
        //SearchMinNode();
        //if (allNodes.Count > 3) 
        //{

        //foreach(var node in subNodes)
        //{
        //    Debug.Log($"node:{node.name}");
        //}
        //foreach (var nodePair in subNodesDict)
        //{
        //    Debug.Log(nodePair.Key + "  " + nodePair.Value.Count);
        //    foreach (var node in nodePair.Value)
        //    {
        //        Debug.Log($"{nodePair.Key} -----> {node}");
        //    }
        //}

        //}
    }
}