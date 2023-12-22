using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftZoneSize : MonoBehaviour
{
    RectTransform myTransform;
    [SerializeField] RectTransform upNumbertransform;
    RectTransform parentTransform;
    void Update()
    {
        Resize();
    }
    private void OnEnable()
    {
        Resize();
    }
    void Resize()
    {
        myTransform = GetComponent<RectTransform>();
        parentTransform = transform.parent.gameObject.GetComponent<RectTransform>();

        //�A���J�[�|�C���g�����ɐݒ�
        myTransform.anchorMin = new Vector2(0, 0f);
        myTransform.anchorMax = new Vector2(0, 1f);

        //�v�fupNmbuer�̍��[�̈ʒu���擾
        float leftEdgeOfUpNumber_local = upNumbertransform.anchoredPosition.x - upNumbertransform.rect.width / 2;
        float leftEdgeOfUpNumber = upNumbertransform.TransformPoint(new Vector3(leftEdgeOfUpNumber_local, 0, 0)).x;


        //�s�{�b�g�����[�ɐݒ�
        myTransform.pivot = new Vector2(0, 0.5f);

        //RightZoneSize��UI�̕����v�Z���A�T�C�Y��ݒ�
        float widthForLeftZone = leftEdgeOfUpNumber;
        myTransform.sizeDelta = new Vector2(widthForLeftZone, upNumbertransform.sizeDelta.y);
    }
}
