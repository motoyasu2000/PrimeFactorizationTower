using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //���̑�g�̏������s�����̂��A�q�v�f�̃��T�C�W���O���s���B
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "DoneText") continue;
            child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
