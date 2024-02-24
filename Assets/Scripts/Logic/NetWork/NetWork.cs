using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UI;
public class Network : MonoBehaviour
{
    //�l�b�g���[�N�̍\�����{�@�\�Ɏg�p�������
    static int[] primeNumberPool; //�Q�[�����ň����S�Ă̑f��
    List<GameObject> wholeNetwork = new List<GameObject>(); //�S�m�[�h�̃��X�g�A�l�b�g���[�N�S��
    Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>();

    //�T�u�O���t�̒T��
    const int CheckNumParFrame = 3; //1�t���[��������ɃL���[������o����
    Queue<ExpandNetwork> startExpandNetworks = new Queue<ExpandNetwork>(); //�l�b�g���[�N�̊g�����J�n����ŏ��̃T�u�l�b�g���[�N�����X�g�Ƃ��ĕۑ����Ă����B�񓯊��̏�����������s���邽�߁A�^�v���̂Q�ڂ̗v�f�͏����̎���

    //�����̐���
    bool nowCriteriaMetChecking = false; //������B��������A�������������������݂̃l�b�g���[�N�����ɖ������Ă��邩���ǂ�����\���ϐ�
    ConditionGenerator conditionGenerator;
    Dictionary<int, int> freezeCondition;
    public ConditionGenerator _conditionGenerator => conditionGenerator;
    public Dictionary<int, int> FreezeCondition => freezeCondition;

    //�����B�����̏��� ���Q�[�����[�h���Ƃɏ����B�����̏������ς��\��������B
    GameModeManager gameModeManager;
    SoundManager soundManager;
    EffectTextManager effectTextManager;
    GameObject freezeEffect;

    private void Start()
    {
        //����������
        primeNumberPool = GameModeManager.GameModemanagerInstance.PrimeNumberPool;
        gameModeManager = GameModeManager.GameModemanagerInstance;
        soundManager = SoundManager.SoundManagerInstance;
        conditionGenerator = transform.Find("ConditionGenerator").GetComponent<ConditionGenerator>();
        effectTextManager = GameObject.Find("EffectText").GetComponent<EffectTextManager>();
        freezeEffect = (GameObject)Resources.Load("FreezeEffect");
        freezeCondition = _conditionGenerator.GenerateCondition();
        foreach (var value in primeNumberPool)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

    private void Update()
    {
        //checkNumParFrame�̐����l�񂾂��L���[�ɓ����Ă��ď����𖞂������̂��Ȃ����l�b�g���[�N���Ń`�F�b�N���s���B
        for (int i = 0; i < CheckNumParFrame; i++)
        {
            if (startExpandNetworks.Count == 0)
            {
                nowCriteriaMetChecking = false;
                return;
            }
            var item = startExpandNetworks.Dequeue();
            foreach (var block in item.myNetwork)
            {
                if (block.GetComponent<BlockInfo>().enabled == false) return; //�����l�b�g���[�N����؂藣����ď������s���\��łȂ��u���b�N�����̒��Ɋ܂܂�Ă��܂��Ă���A���̂悤�Ȃ��̂͏����𖞂����Ȃ��B
            }
            ExpandAndSearch(item);
        }

    }

    //�m�[�h�̒ǉ��AwholeNetwork��nodesDict�̍X�V���s���B
    public void AddNode(GameObject node)
    {
        wholeNetwork.Add(node);
        BlockInfo info = node.GetComponent<BlockInfo>();
        if (System.Array.Exists(primeNumberPool, element => element == info.GetPrimeNumber()))
        {
            if (!nodesDict.ContainsKey(info.GetPrimeNumber()))
            {
                nodesDict.Add(info.GetPrimeNumber(), new List<GameObject>());
            }
            nodesDict[info.GetPrimeNumber()].Add(node);
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
        wholeNetwork.Remove(originNode);
        nodesDict[originNode.GetComponent<BlockInfo>().GetPrimeNumber()].Remove(originNode);
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
    private void FreezeNodes(List<GameObject> nodes)
    {
        for(int i=1; i<nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
        SafeCutNodes(nodes);
        ChangeColorNodes(nodes);
    }

    //�������Ŏw�肵�����Ԍ�A�������Ŏw�肵���Q�[���I�u�W�F�N�g�̃��X�g�ɁA�����I�Ȍv�Z���s��Ȃ����郁�\�b�h�B�󒆂ɌŒ肳���B
    IEnumerator StopRigidbodys(List<GameObject> nodes, float second)
    {
        yield return new WaitForSeconds(second);
        foreach (var node in nodes)
        {
            Rigidbody2D rb2d = node.GetComponent<Rigidbody2D>();
            rb2d.velocity = Vector3.zero;
            rb2d.angularVelocity = 0;
            rb2d.isKinematic = true;
            node.GetComponent<SpriteRenderer>().color = new Color(23f / 255f, 1f, 1f);
        }
        yield break;
    }

    //�����𖞂������Ƃ��̏���
    private void CompleteConditionsProcess(List<GameObject> nodes)
    {
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                FreezeNodes(nodes);
                effectTextManager.PrintEffectText("Criteria Met");
                soundManager.PlayAudio(soundManager.VOICE_CRITERIAMAT);
                DelayProcessFreeze(nodes, 1.5f);
                break;
        }
        //�㏈��
        startExpandNetworks = new Queue<ExpandNetwork>(); //�T������������������l�b�g���[�N���ɏ����𖞂������̂����݂��Ȃ��ƍl������̂ŁA�L���[�����Z�b�g���Ă����B(����ƃo�O����������)
        nowCriteriaMetChecking = true;
        CheckConditionAllNetwork();
    }

    //�������Ŏw�肵�����Ԍ�AFreeze�̕����A�T�E���h�A�G�t�F�N�g���o�͂��A�������Ŏw�肵��GameObject�̃��X�g���󒆂ɌŒ肷��
    private void DelayProcessFreeze(List<GameObject> nodes, float delayTime)
    {
        Vector3 nodesCenter = CaluculateCenter(nodes);
        StartCoroutine(StopRigidbodys(nodes, delayTime));
        StartCoroutine(soundManager.PlayAudio(soundManager.VOICE_FREEZE, delayTime));
        StartCoroutine(soundManager.PlayAudio(soundManager.SE_FREEZE, delayTime));
        StartCoroutine(effectTextManager.PrintEffectText("Freeze", delayTime));
        StartCoroutine(InstantiateEffect(freezeEffect, nodesCenter, delayTime));
        freezeCondition = _conditionGenerator.GenerateCondition();
    }

    //�����ŗ^����ꂽ�Q�[���I�u�W�F�N�g�����̏d�S���v�Z���ĕԂ����\�b�h
    private Vector3 CaluculateCenter(List<GameObject> gameObjects)
    {
        Vector3 center = Vector3.zero;
        foreach (var gameObject in gameObjects)
        {
            center += gameObject.transform.position;
        }
        center /= gameObjects.Count;
        return center;
    }

    //��O�����Ŏw�肵�����Ԍ�ɁA�������Ŏw�肵���ʒu�ɁA�������Ŏw�肵��effect(GameObject)�𐶐����郁�\�b�h
    private IEnumerator InstantiateEffect(GameObject effect, Vector3 position, float second)
    {
        yield return new WaitForSeconds (second);
        Instantiate(effect, position, Quaternion.identity);
    }

    //�l�b�g���[�N�S�̂ɏ����Ƀ}�b�`������̂��Ȃ�����T�����邽�߂̃��\�b�h �����ɑ��݂���f���̂����A�l�b�g���[�N�S�̂ōŏ����̑f����T���A���̃m�[�h����T�����n�߂�
    void CheckConditionAllNetwork()
    {
        int minNode = -1; //�ŏ����̑f��
        int minNodeNum = int.MaxValue; //�ŏ����̑f���̐�

        //�����ɑ��݂���f����S�T�����A�ŏ����̂��̂�T��
        foreach (int valueInFreezeCondition in freezeCondition.Keys)
        {
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

    //�������Ŏw�肵�������𖞂����T�u�O���t�̏������A�������Ŏw�肵���l�b�g���[�N���������Ă��邩���`�F�b�N���郁�\�b�h
    bool ContainsAllRequiredNodes(List<GameObject> myNetwork, Dictionary<int, int> requiredNodesDict)
    {
        Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);
        //Debug.Log(string.Join(", ", requiredCounts));

        //���݂̃l�b�g���[�N���̃m�[�h�̏o���񐔂��J�E���g
        foreach (var node in myNetwork)
        {
            int nodeValue = node.GetComponent<BlockInfo>().GetPrimeNumber();
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

    //�l�b�g���[�N����T�u�O���t��T������g���O��ExpandNetwork���A�g������l�b�g���[�N������L���[�ɒǉ�����BUpdate���ŁA���̃L���[����v�f�����o����A�����ŒT�����n�܂�B
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
        startExpandNetworks.Enqueue(currentNetwork);
    }

    //�l�b�g���[�N���g�����Ȃ���T�u�O���t��T������ċA�I���\�b�h
    private void ExpandAndSearch(ExpandNetwork currentNetwork)
    {
        //�g�������l�b�g���[�N�������𖞂����Ă�����
        if (ContainsAllRequiredNodes(currentNetwork.myNetwork, freezeCondition))
        {
            //�����������Ɋ��ɏ�����B�����Ă����ꍇ���������Đ������čĂђ����B
            if (nowCriteriaMetChecking) 
            {
                freezeCondition = _conditionGenerator.GenerateCondition();
                nowCriteriaMetChecking = true;
                CheckConditionAllNetwork();
                //Debug.Log("�Đ���");
                return;
            }
            Debug.Log(string.Join(", ", currentNetwork.myNetwork));
            CompleteConditionsProcess(currentNetwork.myNetwork);
            return;
        }

        //���݊g�����̃l�b�g���[�N�ɑ��݂���e�m�[�h�̗אڃm�[�h��T��
        foreach (var node in currentNetwork.myNetwork)
        {
            //�����Ă���m�[�h�ɗאڂ���m�[�h��S�ă��X�g�ɒǉ�
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            //���݂̃l�b�g���[�N��closedList�Ɋ܂܂�Ă��Ȃ��m�[�h�݂̂�I��
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.closedList.Contains(n) && !currentNetwork.myNetwork.Contains(n)).ToList();

            //�אڂ���V�����m�[�h���Ȃ���΃X�L�b�v
            if (adjacentNodes.Count == 0)
            {
                continue; 
            }

            //�����Ă���m�[�h�ɗאڂ���m�[�h��T��
            foreach (var adjacentNode in adjacentNodes)
            {
                if (adjacentNode.gameObject.GetComponent<BlockInfo>().enabled == false) continue; //�l�b�g���[�N����؂藣�����blockinfo���Ȃ��Ȃ����u���b�N��adjacentNodes�Ɋ܂܂��΁A�������X�L�b�v
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, freezeCondition); //�g��

                //�ꍇ�ɂ���Ă͊g���O�ɖ߂�
                if (newNetwork.BackFlag)
                {
                    newNetwork = newNetwork.Beforenetwork;
                }
                
                //�ċA�Ăяo��
                ExpandAndSearch(newNetwork);
            }
        }
    }
}