using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGenerator : MonoBehaviour
{
    private void Awake()
    {
        //�����A�Q�[�����[�h�}�l�[�W���[��static�C���X�^���X�����݂��Ȃ���΁A�Q�[�����[�h�}�l�[�W���[���A�^�b�`���ꂽ�C���X�^���X�𐶐����A���������s���B
        if (GameModeManager.GameModemanagerInstance == null)
        {
            GameObject gameModeManager = new GameObject("GameModeManager");
            gameModeManager.AddComponent<GameModeManager>();
        }
        if(SoundManager.SoundManagerInstance == null)
        {
            Instantiate(Resources.Load("SoundManager")); //�T�E���h�}�l�[�W���[�͎q�v�f������̂ŃQ�[���I�u�W�F�N�g���A�^�b�`���邾���ł͑���Ȃ��B
        }
    }
}
