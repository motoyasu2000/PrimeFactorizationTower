using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�^�b�v�G�t�F�N�g���A�p�[�e�B�N���̍Đ����I�������玩���I��Destroy�����悤�ɂ���N���X�B
public class TapEffectDestroyer : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constant);
    }
}
