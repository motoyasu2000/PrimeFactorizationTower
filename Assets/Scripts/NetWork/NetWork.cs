using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 }; //�f���z��
    [SerializeField] List<GameObject> allNodes = new List<GameObject>(); //�S�m�[�h�̃��X�g
    [SerializeField] Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>(); //�e�m�[�h����������̂����i�[�������X�g
    List<GameObject> subNodes = new List<GameObject>(); //�T�u�l�b�g���[�N�p�̃��X�g
    [SerializeField] Dictionary<int, List<GameObject>> subNodesDict = new Dictionary<int, List<GameObject>>(); //�T���p�̃T�u�l�b�g���[�N�Ɋe�m�[�h����������̂����i�[�������X�g



    private void Start()
    {
        //nodeDict�̏�����
        foreach (var value in primeNumbers)
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
        foreach (GameObject mainNode in allNodes)
        {
            int mainNodeNum = mainNode.GetComponent<BlockInfo>().GetNumber();
            if (subNetPattern.Contains(mainNodeNum))
            {
                subNodes.Add(mainNode); //�T�u�O���t�̃m�[�h���X�V
                RenuealSubgraphDict(mainNodeNum); //�T�u�O���t�̎������X�V���郁�\�b�h
            }
        }
        //�G�b�W�̏���
        foreach (var subnode in allNodes)
        {
            subnode.GetComponent<BlockInfo>().DeleteMissNeighberBlock(subNetPattern);
        }

        //�f�o�b�O�p
        //foreach (var subnode in subNodes)
        //{
        //    foreach (var neighbor in subnode.GetComponent<BlockInfo>().GetNeighborEdge())
        //    {
        //        Debug.Log($"{subnode.name}-------------{neighbor.name}");
        //    }
        //}
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

    //�T�u�O���t�̍ł����Ȃ��f���̃L�[��Ԃ��֐�
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
        //Debug.Log("�ł��l�b�g���[�N�����ɏ��Ȃ��f��" + minNodeNumber);
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

    //���̃l�b�g���[�N���g�����Ă���pattern�Ƀ}�b�`����graph��T������B�g������Ƃ͊g�����ꂽ�C���X�^���X�𐶐����邱�ƂŁA��ނ���Ƃ�beforeNetwork�ɖ߂�A�߂�O�ɒǉ����Ă������X�g���N���[�Y�h���X�g�ɒǉ�����B
    class ExpandNetwork
    {
        public List<GameObject> myNetwork { get; private set; } = new List<GameObject>(); //���݂̃l�b�g���[�N���
        public List<GameObject> closedList { get; private set; } = new List<GameObject>(); //���݂̃l�b�g���[�N�ɑ΂���N���[�Y�h���X�g�B�����菬�����N���[�Y�h���X�g�̏��������p���B
        ExpandNetwork beforeNetwork;�@//�ЂƂO�̃l�b�g���[�N�ɖ߂邱�Ƃ�����̂ŕK�v

        //�R���X�g���N�^�B�Ăяo�������猩�āA���݂̃l�b�g���[�N�ƒǉ��������m�[�h�������Ŏw�肷��B
        public ExpandNetwork(ExpandNetwork beforeNetwork, GameObject nowNode, Dictionary<int, int> requiredNodesDict)
        {
            //beforeNetwork������==�P�Ԗڈȍ~�̃m�[�h => closedList��myNetwork��beforeNetwork�ŏ�����
            if (beforeNetwork != null)
            {
                closedList = new List<GameObject>(beforeNetwork.closedList);
                myNetwork = new List<GameObject>(beforeNetwork.myNetwork);
                this.beforeNetwork = beforeNetwork;
            }
            closedList.Add(nowNode);
            // �m�[�h���v���𖞂����Ă��邩�m�F
            int nodeValue = nowNode.GetComponent<BlockInfo>().GetNumber();
            if (requiredNodesDict.ContainsKey(nodeValue))
            {
                int requiredCount = requiredNodesDict[nodeValue];
                int currentCount = myNetwork.Count(node => node.GetComponent<BlockInfo>().GetNumber() == nodeValue);

                // �m�[�h���v���𖞂����Ă���ꍇ�ɂ̂ݒǉ�
                if (currentCount < requiredCount)
                {
                    myNetwork.Add(nowNode);
                }
                //�v���𖞂����Ă��Ȃ���΂ЂƂO�̃l�b�g���[�N�ɖ߂�B
                else
                {
                    myNetwork = beforeNetwork.myNetwork;
                }
            }
        }

        // �l�b�g���[�N�ɗאڃm�[�h��ǉ����郁�\�b�h�i�d����h���j
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

        // �p�^�[���}�b�`���O�̃��W�b�N���C���i�������g�p���ďd�������e�j
        public bool ContainsAllRequiredNodes(Dictionary<int, int> requiredNodesDict)
        {
            Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);

            // ���݂̃l�b�g���[�N���̃m�[�h�̏o���񐔂��J�E���g
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

            return requiredCounts.Count == 0; // �K�v�ȃm�[�h�����ׂĊ܂܂�Ă����true
        }
    }

    public void SearchMatchingPattern(Dictionary<int, int> requiredNodesDict)
    {

        foreach (var startNode in subNodesDict[SearchMinNode()])
        {
            ExpandNetwork currentNetwork = new ExpandNetwork(null, startNode, requiredNodesDict);

            // �l�b�g���[�N���g�����Ă�������
            ExpandAndSearch(currentNetwork, requiredNodesDict);
        }

    }

    // �l�b�g���[�N���g�����Ȃ���T�u�O���t��T������ċA�I���\�b�h
    private void ExpandAndSearch(ExpandNetwork currentNetwork, Dictionary<int, int> requiredNodesDict)
    {
        Debug.Log(string.Join(", ", currentNetwork));
        if (currentNetwork.ContainsAllRequiredNodes(requiredNodesDict))
        {
            foreach (var node in currentNetwork.myNetwork)
            {
                Destroy(node);
            }
            return;
        }

        // �e�m�[�h�̗אڃm�[�h��T��
        foreach (var node in currentNetwork.myNetwork)
        {
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            // ���݂̃l�b�g���[�N��closedList�Ɋ܂܂�Ă��Ȃ��m�[�h�݂̂�I��
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.closedList.Contains(n) && !currentNetwork.myNetwork.Contains(n)).ToList();

            if (adjacentNodes.Count == 0)
            {
                continue; // �אڂ���V�����m�[�h���Ȃ���΃X�L�b�v
            }

            foreach (var adjacentNode in adjacentNodes)
            {
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, requiredNodesDict);
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