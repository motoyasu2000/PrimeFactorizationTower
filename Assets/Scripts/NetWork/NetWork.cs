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
    [SerializeField] List<GameObject> allNodes = new List<GameObject>(); //�S�m�[�h�̃��X�g�g
    [SerializeField] Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>();
    [SerializeField] GameModeManager gameModeManager;
    [SerializeField] SoundManager soundManager;
    [SerializeField] MainTextManager mainTextManager;
    ConditionGenerator conditionGenerator;
    public ConditionGenerator _conditionGenerator => conditionGenerator;
    [SerializeField]Dictionary<int, int> freezeCondition;
    HashSet<int> freezeSet = new HashSet<int>();
    public HashSet<int> FreezeSet => freezeSet;

    public Dictionary<int, int> FreezeCondition => freezeCondition;
    Queue<ExpandNetwork> startExpandNetworks = new Queue<ExpandNetwork>(); //�l�b�g���[�N�̊g�����J�n����ŏ��̃T�u�l�b�g���[�N�����X�g�Ƃ��ĕۑ����Ă����B�񓯊��̏�����������s���邽�߁A�^�v���̂Q�ڂ̗v�f�͏����̎���
    bool wasCriteriaMet = false;


    private void Start()
    {
        gameModeManager = GameObject.Find("GameModeManager").GetComponent<GameModeManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        conditionGenerator = transform.Find("ConditionGenerator").GetComponent<ConditionGenerator>();

        freezeCondition = _conditionGenerator.GenerateCondition();
        foreach(var key in freezeCondition.Keys)
        {
            freezeSet.Add(key);
        }
        foreach (var value in primeNumbers)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

    private void Update()
    {
        if (startExpandNetworks.Count == 0) return;
        //foreach(var currentNetwork in startExpandNetworks)
        //{
        //    ExpandAndSearch(currentNetwork.Item1, currentNetwork.Item2);
        //}
        var item = startExpandNetworks.Dequeue();
        foreach(var block in item.myNetwork)
        {
            if (block.GetComponent<BlockInfo>().enabled == false) return; //�����l�b�g���[�N����؂藣����ď������s���\��łȂ��u���b�N�����̒��Ɋ܂܂�Ă��܂��Ă���A���̂悤�Ȃ��̂͏����𖞂����Ȃ��B
        }
        wasCriteriaMet = false;
        ExpandAndSearch(item);

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
            //foreach(var value in nodesDict.Values)
            //{
            //    Debug.Log(string.Join(",", value));
            //}
        }
        else
        {
            Debug.LogError("�f����`�O�̃m�[�h����`����悤�Ƃ��Ă��܂��B");
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

    //�l�b�g���[�N�������̃m�[�h��Destroy���郁�\�b�h
    private void SafeDestroyNode(GameObject originNode)
    {
        SafeCutNode(originNode);
        Destroy(originNode);
    }

    //�l�b�g���[�N�������̃T�u�l�b�g���[�N��Destroy���郁�\�b�h
    private void SafeDestroyNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeDestroyNode(node);
        }
    }

    //�l�b�g���[�N�������̃m�[�h��؂藣�����\�b�h(Destroy�͂��Ȃ�)
    private void SafeCutNode(GameObject originNode)
    {
        //�؂藣�����̃m�[�h�Ɨאڂ���G�b�W���ꎞ�I�ȕϐ��Ɋi�[(�R���N�V�������C�e���[�V�������ɕύX���Ă͂Ȃ�Ȃ�����)
        List<GameObject> tmpNeighborNode = new List<GameObject>();
        foreach (var neighborNode in originNode.GetComponent<BlockInfo>().GetNeighborEdge())
        {
            tmpNeighborNode.Add(neighborNode);
        }
        //�؂藣�����̃m�[�h�Ɨאڂ���G�b�W�����ۂɍ폜����
        foreach (var neighborNode in tmpNeighborNode)
        {
            DetachNode(neighborNode, originNode);
        }
        //�l�b�g���[�N���炻�̃m�[�h���폜
        allNodes.Remove(originNode);
        //�l�b�g���[�N������������̃m�[�h���폜
        nodesDict[originNode.GetComponent<BlockInfo>().GetNumber()].Remove(originNode);
        //�u���b�N�̏������킹��B
        originNode.GetComponent<BlockInfo>().enabled = false;
    }

    //�l�b�g���[�N�������̃T�u�l�b�g���[�N��؂藣�����\�b�h
    private void SafeCutNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeCutNode(node);
        }
    }

    //�T�u�l�b�g���[�N�̐F��ύX�����郁�\�b�h
    private void ChangeColorNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            node.GetComponent<SpriteRenderer>().color = Color.white;
            node.GetComponent<SpriteRenderer>().material.color = Color.white;
        }
    }

    //�T�u�l�b�g���[�N�𕨗��I�Ɍ������F��ύX���A���z�I�ȃl�b�g���[�N����؂藣�����\�b�h
    private void FriezeNodes(List<GameObject> nodes)
    {
        for(int i=1; i<nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
        SafeCutNodes(nodes);
        ChangeColorNodes(nodes);
    }

    //�����𖞂������Ƃ��̏���
    private void CompleteConditions(List<GameObject> nodes)
    {
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                FriezeNodes(nodes);
                mainTextManager.TmpPrintMainText("Criteria Met");
                soundManager.PlayAudio(soundManager.VOICE_CRITERIAMAT);

                StartCoroutine(soundManager.PlayAudio(soundManager.VOICE_FREEZE,1.5f));
                StartCoroutine(mainTextManager.TmpPrintMainText("Freeze",1.5f));
                freezeCondition = _conditionGenerator.GenerateCondition();
                break;
        }
    }

    //�l�b�g���[�N�S�̂ɏ����Ƀ}�b�`������̂��Ȃ�����T�����邽�߂̃��\�b�h
    void CheckConditionAllNetwork()
    {
        int minNodeNum = int.MaxValue; //�����ɑ��݂���m�[�h�̐����̓��A�l�b�g���[�N���ɍŏ����ł��鐔���̌����i�[����ϐ�
        int minNode = -1; //�ŏ����̑f��
        //�����ɑ��݂���f����S�T�����A�ŏ����̂��̂�T��
        foreach(int valueInFreezeCondition in freezeCondition.Keys)
        {
            //�����A���݂̍ŏ��������A�����Ă�������̌��̂ق��������Ȃ�������A�ŏ����̍X�V�ƍŏ����̑f�������ł���̂��̍X�V������B
            if(minNodeNum > nodesDict[valueInFreezeCondition].Count)
            {
                minNodeNum = nodesDict[valueInFreezeCondition].Count;
                minNode = valueInFreezeCondition;
            }
        }

        //�ŏ����̑f���͂��łɋ��܂��Ă���̂ŁA����ɑ΂���for�����񂵂�startExpandNetworks
        foreach(var node in nodesDict[minNode])
        {
            startExpandNetworks.Enqueue(new ExpandNetwork(null, node, freezeCondition));
        }
    }

    //�p�^�[���}�b�`���O�̃��W�b�N
    bool ContainsAllRequiredNodes(List<GameObject> myNetwork, Dictionary<int, int> requiredNodesDict)
    {
        Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);
        //Debug.Log(string.Join(", ", requiredCounts));

        //���݂̃l�b�g���[�N���̃m�[�h�̏o���񐔂��J�E���g
        foreach (var node in myNetwork)
        {
            int nodeValue = node.GetComponent<BlockInfo>().GetNumber();
            if (requiredCounts.ContainsKey(nodeValue))
            {
                requiredCounts[nodeValue]--;
                if (requiredCounts[nodeValue] == 0)
                    requiredCounts.Remove(nodeValue);
            }
            //�����T�u�l�b�g���[�N���Ɋ֌W�̂Ȃ����l�������false��Ԃ�
            else
            {
                return false;
            }
        }

        return requiredCounts.Count == 0; //�K�v�ȃm�[�h�����ׂĊ܂܂�Ă����true
    }
    public void AddStartExpandNetworks(HashSet<GameObject> neiborSet)
    {
        ExpandNetwork currentNetwork = null;
        foreach (var startNode in neiborSet)
        {
            if(currentNetwork == null)
            {
                currentNetwork = new ExpandNetwork(null, startNode, freezeCondition);
            }
            else
            {
                currentNetwork = new ExpandNetwork(currentNetwork, startNode, freezeCondition);
            }
        }
        //Debug.Log(string.Join(", ",currentNetwork.myNetwork));
        //�l�b�g���[�N���g�����Ă�������
        startExpandNetworks.Enqueue(currentNetwork);
    }

    //�l�b�g���[�N���g�����Ȃ���T�u�O���t��T������ċA�I���\�b�h
    private void ExpandAndSearch(ExpandNetwork currentNetwork)
    {
        //if (wasCriteriaMet) return; //�������݂̃t���[���ŏ�����B���ς݂Ȃ�return���� 1�t���[��������Ɉ�̏Փ˃p�^�[������̌��m�����s��Ȃ��̂�1�t���[�����Ŕ����ς݂Ȃ炻��ȏ�T���Ȃ��Ă悢 �P��̏Փ˂���T����������𖞂����T�u�l�b�g���[�N���������݂���ꍇ�����邪�A������͈̂�ł����Ƃ������ƁB
        //Debug.Log(string.Join(", ", currentNetwork.myNetwork));
        //Debug.Log(string.Join(", ", currentNetwork));

        if (ContainsAllRequiredNodes(currentNetwork.myNetwork, freezeCondition))
        {
            //wasCriteriaMet = true;
            Debug.Log(string.Join(", ", currentNetwork.myNetwork));

            CompleteConditions(currentNetwork.myNetwork);
            startExpandNetworks = new Queue<ExpandNetwork>(); //�T������������������l�b�g���[�N���ɏ����𖞂������̂����݂��Ȃ��ƍl������̂ŁA�L���[�����Z�b�g���Ă����B
            CheckConditionAllNetwork();

            return;
        }

        //�e�m�[�h�̗אڃm�[�h��T��
        foreach (var node in currentNetwork.myNetwork)
        {
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            //���݂̃l�b�g���[�N��closedList�Ɋ܂܂�Ă��Ȃ��m�[�h�݂̂�I��
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.closedList.Contains(n) && !currentNetwork.myNetwork.Contains(n)).ToList();

            if (adjacentNodes.Count == 0)
            {
                continue; //�אڂ���V�����m�[�h���Ȃ���΃X�L�b�v
            }

            foreach (var adjacentNode in adjacentNodes)
            {
                if (adjacentNode.gameObject.GetComponent<BlockInfo>().enabled == false) continue; //�l�b�g���[�N����؂藣�����blockinfo���Ȃ��Ȃ����u���b�N�ɑ΂��Ă�adjacentNodes�Ɋ܂܂��\��������̂ł͂����Ă����B
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, freezeCondition);
                if (newNetwork.BackFlag)
                {
                    newNetwork = newNetwork.Beforenetwork;
                }
                
                ExpandAndSearch(newNetwork);
            }
        }
    }
}