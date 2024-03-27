using UnityEngine;

namespace UI
{
    //画面上部左側に表示される条件のサイズや位置を適切に修正するためのクラス
    public class LeftZoneSize : MonoBehaviour
    {
        RectTransform myTransform;
        RectTransform originNumberTransform;
        Camera mainCamera;
        Canvas canvas;
        void Start ()
        {
            myTransform = GetComponent<RectTransform>();
            originNumberTransform = GameObject.Find("OriginNumber_Field").GetComponent<RectTransform>();
            mainCamera = Camera.main;
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
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

            //ピボットを左端に設定
            myTransform.pivot = new Vector2(0, 0.5f);

            //要素upNmbuerの左端の位置をビューポート座標で取得
            float leftEdgeOfUpNumber_local = originNumberTransform.anchoredPosition.x - originNumberTransform.rect.width / 2;
            float leftEdgeOfUpNumber_World = originNumberTransform.TransformPoint(new Vector3(leftEdgeOfUpNumber_local, 0, 0)).x;
            float leftEdgeOfUpNumber_Viewport = mainCamera.WorldToViewportPoint(new Vector3(leftEdgeOfUpNumber_World, 0, 0)).x;

            //Canvasのスケールのずれをビューポート座標に変換してから調整し、ワールド座標に戻してsizeDeltaの変更
            float widthForLeftZone_Viewport = leftEdgeOfUpNumber_Viewport / canvas.transform.localScale.x;
            float widthForRightZone_World = mainCamera.ViewportToWorldPoint(new Vector3(widthForLeftZone_Viewport, 0, 0)).x;
            myTransform.sizeDelta = new Vector2(widthForRightZone_World, originNumberTransform.sizeDelta.y);
        }
    }

}