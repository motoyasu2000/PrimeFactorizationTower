using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    //画面上部右側に表示される条件のサイズや位置を適切に修正するためのクラス
    public class RightZoneSize : MonoBehaviour
    {
        RectTransform myTransform;
        RectTransform nowUpNumberTransform;
        void Start()
        {
            myTransform = GetComponent<RectTransform>();
            nowUpNumberTransform = GameObject.Find("NowUpCompositeNumber_Field").GetComponent<RectTransform>();
        }
        void Update()
        {
            Resize();
        }
        void Resize()
        {
            //アンカーポイントを右に設定
            myTransform.anchorMin = new Vector2(1, 0f);
            myTransform.anchorMax = new Vector2(1, 1f);

            //要素upNmbuerの右端の位置を取得
            float rightEdgeOfUpNumber_local = nowUpNumberTransform.anchoredPosition.x - nowUpNumberTransform.rect.width / 2;
            float rightEdgeOfUpNumber = nowUpNumberTransform.TransformPoint(new Vector3(rightEdgeOfUpNumber_local, 0, 0)).x;

            //ピボットを右端に設定
            myTransform.pivot = new Vector2(1, 0.5f);

            //RightZoneSizeのUIの幅を計算し、サイズを設定
            float widthForRightZone = rightEdgeOfUpNumber / CanvasManager.NowScaleFactor;
            myTransform.sizeDelta = new Vector2(widthForRightZone, nowUpNumberTransform.sizeDelta.y);
        }
    }
}
