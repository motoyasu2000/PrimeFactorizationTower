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

        //�A���J�[�|�C���g���E�ɐݒ�
        myTransform.anchorMin = new Vector2(1, 0f);
        myTransform.anchorMax = new Vector2(1, 1f);

        //�v�fupNmbuer�̉E�[�̈ʒu���擾
        float rightEdgeOfUpNumber_local = upNumbertransform.anchoredPosition.x - upNumbertransform.rect.width / 2;
        float rightEdgeOfUpNumber = upNumbertransform.TransformPoint(new Vector3(rightEdgeOfUpNumber_local, 0, 0)).x;


        //�s�{�b�g���E�[�ɐݒ�
        myTransform.pivot = new Vector2(1, 0.5f);

        //RightZoneSize��UI�̕����v�Z���A�T�C�Y��ݒ�
        float widthForRightZone = rightEdgeOfUpNumber;
        myTransform.sizeDelta = new Vector2(widthForRightZone, upNumbertransform.sizeDelta.y);
    }
}
