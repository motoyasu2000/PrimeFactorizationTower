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

        //アンカーポイントを左に設定
        myTransform.anchorMin = new Vector2(0, 0f);
        myTransform.anchorMax = new Vector2(0, 1f);

        //要素upNmbuerの左端の位置を取得
        float leftEdgeOfUpNumber_local = upNumbertransform.anchoredPosition.x - upNumbertransform.rect.width / 2;
        float leftEdgeOfUpNumber = upNumbertransform.TransformPoint(new Vector3(leftEdgeOfUpNumber_local, 0, 0)).x;


        //ピボットを左端に設定
        myTransform.pivot = new Vector2(0, 0.5f);

        //RightZoneSizeのUIの幅を計算し、サイズを設定
        float widthForLeftZone = leftEdgeOfUpNumber;
        myTransform.sizeDelta = new Vector2(widthForLeftZone, upNumbertransform.sizeDelta.y);
    }
}
