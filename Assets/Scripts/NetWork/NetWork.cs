using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 };
    [SerializeField]List<GameObject> allNodes = new List<GameObject>();
    [SerializeField]Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>();

    private void Start()
    {
        //nodeDictの初期化
        foreach(var value in primeNumbers)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

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

    public void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.RemoveNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.RemoveNeighborBlock(node1);
    }

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
    
    private void Update()
    {
        foreach (var nodes in nodesDict) 
        {
            //Debug.Log($"number: {nodes.Key}  nodesCount:{nodes.Value.Count}");
        }

        SearchMinNode();
    }
}