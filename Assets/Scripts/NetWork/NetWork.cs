using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 }; //�f���z��
    [SerializeField]List<GameObject> allNodes = new List<GameObject>(); //�S�m�[�h�̃��X�g
    [SerializeField]Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>(); //�e�m�[�h����������̂����i�[�������X�g
    List<GameObject> subNodes = new List<GameObject>(); //�T�u�l�b�g���[�N�p�̃��X�g
    [SerializeField] Dictionary<int, List<GameObject>> subNodesDict = new Dictionary<int, List<GameObject>>(); //�T���p�̃T�u�l�b�g���[�N�Ɋe�m�[�h����������̂����i�[�������X�g


    private void Start()
    {
        //nodeDict�̏�����
        foreach(var value in primeNumbers)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

    //�m�[�h�̒ǉ��AallNodes��nodesDict�̍X�V���s���B
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
            Debug.LogError("�f����`�O�̃m�[�h����`����悤�Ƃ��Ă��܂��B");
        }
    }

    //�^����ꂽ�p�^�[������allnode��؂���A�T�u�l�b�g���[�N�����B(subnodes��subnodesdict�̍X�V)
    public void CreateSubNetwork(HashSet<int> subNetPattern)
    {
        //�T�u�O���t�̍쐬
        foreach(GameObject mainNode in allNodes)
        {
            int mainNodeNum = mainNode.GetComponent<BlockInfo>().GetNumber();
            if (subNetPattern.Contains(mainNodeNum)){
                subNodes.Add(mainNode); //�T�u�O���t�̃m�[�h���X�V
                RenuealSubgraphDict(mainNodeNum); //�T�u�O���t�̎������X�V���郁�\�b�h
            }
        }
        //�G�b�W�̏���
        foreach(var subnode in allNodes)
        {
            subnode.GetComponent<BlockInfo>().DeleteMissNeighberBlock(subNetPattern);
        }

        //�f�o�b�O�p
        foreach (var subnode in subNodes)
        {
            foreach (var neighbor in subnode.GetComponent<BlockInfo>().GetNeighborEdge())
            {
                Debug.Log($"{subnode.name}-------------{neighbor.name}");
            }
        }
    }

    //�G�b�W�̍X�V���s��(�폜)
    public void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.RemoveNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.RemoveNeighborBlock(node1);
    }

    //�G�b�W�̍X�V���s��(�ǉ�)
    public void AttachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.AddNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.AddNeighborBlock(node1);
    }

    //�ł����Ȃ��f���̃L�[��Ԃ��֐�
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
        Debug.Log("�ł��l�b�g���[�N�����ɏ��Ȃ��f��" + minNodeNumber);
        return minNodeNumber;
    }

    //�T�u�O���t�̎������X�V���郁�\�b�h
    void RenuealSubgraphDict(int mainNodeNum)
    {
        //�T�u�O���t�̊e�f���̎������X�V
        if (!subNodesDict.ContainsKey(mainNodeNum)) //�L�[�����񂴂����Ȃ��Ƃ��̂݁A���C����nodesDict�̃L�[�o�����[�̃Z�b�g������BallNodes�ɑ΂���for�����񂵂Ă���̂ŁA�d���̉\�������邽��
        {
            subNodesDict.Add(mainNodeNum, nodesDict[mainNodeNum]);
            //foreach (var pair in subNodesDict)
            //{
            //    foreach (var value in pair.Value)
            //    {
            //        Debug.Log($"{pair.Key} �F {value.name}");
            //    }
            //}

        }
        //�������݂���΃T�u�O���t�̎��������Z�b�g���A�Đ���
        else
        {
            subNodesDict = new Dictionary<int, List<GameObject>>();
            subNodesDict.Add(mainNodeNum, nodesDict[mainNodeNum]);
            //foreach (var pair in subNodesDict)
            //{
            //    foreach (var value in pair.Value)
            //    {
            //        Debug.Log($"{pair.Key} �F {value.name}");
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