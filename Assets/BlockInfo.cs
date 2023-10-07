using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockInfo : MonoBehaviour
{
    protected int myNumber; //�����̎������B�������Ƃ��̌v�Z�͂���𗘗p����
    protected GameObject selfPrefab; //�������g�̃v���t�@�u���i�[����ϐ�(�p����N���X���猩���������g)
    public GameObject SelfPrefab => selfPrefab;
    public abstract void SetSelfPrefab(); //�������g�̃v���t�@�u�����ł��邩�͌p����̃X�N���v�g�Ō��肷�ׂ�
    public abstract void AddRigidbody2D(); //�u���b�N���Ƃɏd�͂̂��������Ⴄ��������Ȃ��̂ŁA�p����̃N���X�ŋL�q

    //�������g�̔ԍ���ݒ肷��N���X�B�p����ɋL�q�B
    public abstract void SetMyNumber();
}
