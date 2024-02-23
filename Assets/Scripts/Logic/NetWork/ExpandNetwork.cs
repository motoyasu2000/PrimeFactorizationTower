//���̃l�b�g���[�N���g�����Ă���pattern�Ƀ}�b�`����graph��T������B�g������Ƃ͊g�����ꂽ�C���X�^���X�𐶐����邱�ƂŁA��ނ���Ƃ�beforeNetwork�ɖ߂�A�߂�O�ɒǉ����Ă������X�g���N���[�Y�h���X�g�ɒǉ�����B
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExpandNetwork
{
    public List<GameObject> myNetwork { get; private set; } = new List<GameObject>(); //���݂̃l�b�g���[�N���
    public List<GameObject> closedList { get; private set; } = new List<GameObject>(); //���݂̃l�b�g���[�N�ɑ΂���N���[�Y�h���X�g�B�����菬�����N���[�Y�h���X�g�̏��������p���B
    ExpandNetwork beforeNetwork; //�ЂƂO�̃l�b�g���[�N�ɖ߂邱�Ƃ�����̂ŕK�v
    public ExpandNetwork Beforenetwork => beforeNetwork;
    bool backFlag = false;
    public bool BackFlag => backFlag;

    //�R���X�g���N�^�B�Ăяo�������猩�āA���݂̃l�b�g���[�N�ƒǉ��������m�[�h�������Ŏw�肷��B
    public ExpandNetwork(ExpandNetwork beforeNetwork, GameObject nowNode, Dictionary<int, int> freezeCondition)
    {

        //beforeNetwork������==�P�Ԗڈȍ~�̃m�[�h => closedList��myNetwork��beforeNetwork�ŏ�����
        if (beforeNetwork != null)
        {
            closedList = new List<GameObject>(beforeNetwork.closedList);
            myNetwork = new List<GameObject>(beforeNetwork.myNetwork);
            this.beforeNetwork = beforeNetwork;
        }
        if (beforeNetwork != null) beforeNetwork.closedList.Add(nowNode); //��O�̃l�b�g���[�N�ɍ��ǉ������m�[�h���N���[�Y�h���X�g�ɒǉ�����B
                                                                          // �m�[�h���v���𖞂����Ă��邩�m�F
        int nodeValue = nowNode.GetComponent<BlockInfo>().GetPrimeNumber();
        if (freezeCondition.ContainsKey(nodeValue))
        {
            int requiredCount = freezeCondition[nodeValue];
            int currentCount = myNetwork.Count(node => node.GetComponent<BlockInfo>().GetPrimeNumber() == nodeValue);

            // �m�[�h���v���𖞂����Ă���ꍇ�ɂ̂ݒǉ�
            if (currentCount < requiredCount)
            {
                myNetwork.Add(nowNode);
            }
            //�v���𖞂����Ă��Ȃ���΂ЂƂO�̃l�b�g���[�N�ɖ߂�B
            else
            {
                backFlag = true;
            }
        }
        closedList.Add(nowNode);
        //Debug.Log(string.Join(", ", closedList));
        //Debug.Log(string.Join(", ", myNetwork));
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


}