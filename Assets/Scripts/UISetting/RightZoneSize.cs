using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightZoneSize : MonoBehaviour
{
    RectTransform myTransform;
    [SerializeField] RectTransform upNumbertransform;
    RectTransform parentTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
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

        //�A���J�[�|�C���g���E�����ɐݒ�
        myTransform.anchorMin = new Vector2(1, 0f);
        myTransform.anchorMax = new Vector2(1, 1f);

        //�v�fupNmbuer�̉E�[�̈ʒu���擾
        float rightEdgeOfUpNumber = upNumbertransform.anchoredPosition.x + upNumbertransform.rect.width;

        //��ʑS�̂̕����擾
        float totalWidth = parentTransform.rect.width;

        //�s�{�b�g���E�[�ɐݒ�
        myTransform.pivot = new Vector2(1, 0.5f);

        //RightZoneSize��UI�̕����v�Z���A�T�C�Y��ݒ�
        float widthForRightZone = totalWidth - rightEdgeOfUpNumber;
        myTransform.sizeDelta = new Vector2(widthForRightZone / 2, upNumbertransform.sizeDelta.y);
    }
}
