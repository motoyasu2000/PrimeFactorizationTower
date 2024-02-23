using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    //�`���[�g���A���̃e�L�X�g���g�O������N���X
    public class ToggleExplainText : MonoBehaviour
    {
        int toggleCounter = 0; //toggle�����񐔂𐔂���ϐ�
        int overCount = -1; //toggleCounter�̒l��������toggle�����I�[�o�[���邩
        void Awake()
        {
            overCount = transform.childCount;
        }

        //�ЂƂO�̃��j���[���\���ɂ��āA���݂̃��j���[��\������B
        public void Toggle()
        {
            //�ЂƂO�̉�ʂ��\���ɂ���
            if (toggleCounter >= 1) transform.GetChild(toggleCounter - 1).gameObject.SetActive(false);

            //��ʂ̍X�V��toggleCounter�̍X�V�ƏI��
            if (toggleCounter >= overCount)
            {
                gameObject.SetActive(false);
                toggleCounter = 0;
            }
            else
            {
                transform.GetChild(toggleCounter).gameObject.SetActive(true);
                toggleCounter++;
            }
        }

        private void OnEnable()
        {
            Toggle();
        }
    }
}