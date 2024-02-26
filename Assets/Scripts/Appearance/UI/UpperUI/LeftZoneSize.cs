using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI
{
    //画面上部左側に表示される条件のサイズや位置を適切に修正するためのクラス
    public class LeftZoneSize : MonoBehaviour
    {
        RectTransform myTransform;
        RectTransform nowUpNumberTransform;
        void Start ()
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
            //アンカーポイントを左に設定
            myTransform.anchorMin = new Vector2(0, 0f);
            myTransform.anchorMax = new Vector2(0, 1f);

            //要素upNmbuerの左端の位置を取得
            float leftEdgeOfUpNumber_local = nowUpNumberTransform.anchoredPosition.x - nowUpNumberTransform.rect.width / 2;
            float leftEdgeOfUpNumber = nowUpNumberTransform.TransformPoint(new Vector3(leftEdgeOfUpNumber_local, 0, 0)).x;

            //ピボットを左端に設定
            myTransform.pivot = new Vector2(0, 0.5f);

            //RightZoneSizeのUIの幅を計算し、サイズを設定
            float widthForLeftZone = leftEdgeOfUpNumber / CanvasManager.NowScaleFactor;
            myTransform.sizeDelta = new Vector2(widthForLeftZone, nowUpNumberTransform.sizeDelta.y);
        }
    }

}