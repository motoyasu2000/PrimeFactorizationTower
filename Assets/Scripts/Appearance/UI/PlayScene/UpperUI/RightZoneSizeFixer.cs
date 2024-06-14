using UnityEngine;

namespace UI
{
    /// <summary>
    /// 画面上部右側に表示される条件のサイズや位置を適切に修正するためのクラス
    /// </summary>

    public class RightZoneSizeFixer : MonoBehaviour
    {
        RectTransform myTransform;
        RectTransform originNumberTransform;
        Camera mainCamera;
        Canvas canvas;
        void Start()
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
            //アンカーポイントを右に設定
            myTransform.anchorMin = new Vector2(1, 0f);
            myTransform.anchorMax = new Vector2(1, 1f);

            //ピボットを右端に設定
            myTransform.pivot = new Vector2(1, 0.5f);

            //要素upNmbuerの右端の位置をビューポート座標で取得
            float rightEdgeOfUpNumber_local = originNumberTransform.anchoredPosition.x - originNumberTransform.rect.width / 2;
            float rightEdgeOfUpNumber = originNumberTransform.TransformPoint(new Vector3(rightEdgeOfUpNumber_local, 0, 0)).x;
            float rightEdgeOfUpNumber_Viewport = mainCamera.WorldToViewportPoint(new Vector3(rightEdgeOfUpNumber, 0, 0)).x;

            //Canvasのスケールのずれをビューポート座標に変換してから調整し、ワールド座標に戻してsizeDeltaの変更
            float widthForRightZone_Viewport = rightEdgeOfUpNumber_Viewport / canvas.transform.localScale.x;
            float widthForRightZone_World = mainCamera.ViewportToWorldPoint(new Vector3(widthForRightZone_Viewport, 0, 0)).x;
            myTransform.sizeDelta = new Vector2(widthForRightZone_World, originNumberTransform.sizeDelta.y);
        }
    }
}
