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
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
