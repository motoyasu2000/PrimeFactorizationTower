using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 };
    List<GameObject> allNodes = new List<GameObject>();
    Dictionary<int, GameObject> nodesDict = new Dictionary<int, GameObject>();
    public void AddNode(GameObject node)
    {
        allNodes.Add(node);
        BlockInfo info = node.GetComponent<BlockInfo>();
        if (System.Array.Exists(primeNumbers, element => element == info.GetNumber()))
        {
            if (!nodesDict.ContainsKey(info.GetNumber()))
            {
                nodesDict.Add(info.GetNumber(), node);
            }
            else
            {
                nodesDict[info.GetNumber()] = node;
            }
        }
        else
        {
            Debug.LogError("素数定義外のノードが定義されようとしています。");
        }
    }

    public void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.RemoveNeighjborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.RemoveNeighjborBlock(node1);
    }

    public void AttachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.RemoveNeighjborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.RemoveNeighjborBlock(node1);
    }

}