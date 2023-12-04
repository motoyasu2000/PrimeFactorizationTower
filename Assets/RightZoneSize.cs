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

        //アンカーポイントを右中央に設定
        myTransform.anchorMin = new Vector2(1, 0f);
        myTransform.anchorMax = new Vector2(1, 1f);

        //要素upNmbuerの右端の位置を取得
        float rightEdgeOfUpNumber = upNumbertransform.anchoredPosition.x + upNumbertransform.rect.width;

        //画面全体の幅を取得
        float totalWidth = parentTransform.rect.width;

        //ピボットを右端に設定
        myTransform.pivot = new Vector2(1, 0.5f);

        //RightZoneSizeのUIの幅を計算し、サイズを設定
        float widthForRightZone = totalWidth - rightEdgeOfUpNumber;
        myTransform.sizeDelta = new Vector2(widthForRightZone / 2, upNumbertransform.sizeDelta.y);
    }
}
