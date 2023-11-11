using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 }; //素数配列
    [SerializeField]List<GameObject> allNodes = new List<GameObject>(); //全ノードのリスト
    [SerializeField]Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>(); //各ノードがいくつあるのかを格納したリスト
    List<GameObject> subNodes = new List<GameObject>(); //サブネットワーク用のリスト
    [SerializeField] Dictionary<int, List<GameObject>> subNodesDict = new Dictionary<int, List<GameObject>>(); //探索用のサブネットワークに各ノードがいくつあるのかを格納したリスト


    private void Start()
    {
        //nodeDictの初期化
        foreach(var value in primeNumbers)
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
        //サブグラフの作成
        foreach(GameObject mainNode in allNodes)
        {
            int mainNodeNum = mainNode.GetComponent<BlockInfo>().GetNumber();
            if (subNetPattern.Contains(mainNodeNum)){
                subNodes.Add(mainNode); //サブグラフのノードを更新
                RenuealSubgraphDict(mainNodeNum); //サブグラフの辞書を更新するメソッド
            }
        }
        //エッジの消去
        foreach(var subnode in allNodes)
        {
            subnode.GetComponent<BlockInfo>().DeleteMissNeighberBlock(subNetPattern);
        }

        //デバッグ用
        foreach (var subnode in subNodes)
        {
            foreach (var neighbor in subnode.GetComponent<BlockInfo>().GetNeighborEdge())
            {
                Debug.Log($"{subnode.name}-------------{neighbor.name}");
            }
        }
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

    //最も少ない素数のキーを返す関数
    public int SearchMinNode()
    {
        int nowNodeCount = -1;
        int minNodeCount = int.MaxValue;
        int minNodeNumber = -1;
        foreach(var nodes in nodesDict)
        {
            nowNodeCount = nodes.Value.Count;
            if(minNodeCount > nowNodeCount)
            {
                minNodeCount = nowNodeCount;
                minNodeNumber = nodes.Key;
            }
        }
        Debug.Log("最もネットワーク内部に少ない素数" + minNodeNumber);
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