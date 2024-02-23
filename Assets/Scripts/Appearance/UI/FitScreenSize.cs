using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    //��ʏ㕔�̂�����u���b�N��ݒ肷�鍇������\��������A�����̍�������\�����邽�߂�UI�̍������v�Z����N���X�B�[���̉�ʕ��ɍ��킹�ēK�؂Ɍv�Z�ł���悤�ɂ���B
    public class FitScreenSize : MonoBehaviour
    {
        RectTransform rectTransform;
        float width;
        float canvasHeight;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            width = rectTransform.rect.width;
            canvasHeight = transform.parent.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
            rectTransform.sizeDelta = new Vector2(0, -canvasHeight + width / 4.2f); //-canvasHeight����[�̈ʒu�ł������牡��/4.2������������ ���̏����ɂ��UI�̃T�C�Y���X�}�z�̉�ʕ��Ɍ��炸���ɂ���B
                                                                                    //Debug.Log(canvasHeight + " " + width);

            //���̑�g�̏������s�����̂��A�q�v�f�̃��T�C�W���O���s���B(�q�v�f�����������邱�ƂŁA���̎q�v�f�̎��X�N���v�g�ɂ�郊�T�C�W���O���s����B)
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "DoneText") continue;
                child.gameObject.SetActive(true);
            }
        }
    }
}