using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchBlock : MonoBehaviour
{
    //入力管理
    bool isDragging = false; //ドラッグしているかどうか
    Vector3 touchPosition; //現在タッチしている位置
    Transform draggedObject = null; //現在選択しているゲームオブジェクトを格納する変数　Update内でRaycastを毎秒行っているので、
                                    //選択するゲームオブジェクトが変更されないようにドラッグ中のオブジェクトのみを取得するようにしている。
    //ブロックの処理
    BlockInfo blockInfo;
    Network network;
    GraphicRaycaster graphicRaycaster;
    SingleGenerateManager singleGenerateManager; //ゲームオブジェクトが単一であることを保証するためのクラス
    GameObject primeNumberGeneratingPoint; //ゲームオブジェクトを生成する場所を示すゲームオブジェクト  
    GameObject blockField;
    GameObject afterField;

    //UI部分
    GameObject canvas;
    EventSystem eventSystem;

    private void Start()
    {
        //初期化
        blockInfo = GetComponent<BlockInfo>();
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        blockField = GameObject.Find("BlockField");
        afterField = blockField.transform.Find("AfterField").gameObject;
        network = GameObject.Find("Network").GetComponent<Network>();
        canvas = GameObject.Find("Canvas");
        eventSystem = FindObjectOfType<EventSystem>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        //Input.touchesはフレームごとの全てのタッチを取得する(画面に同時に触れた全ての指、１フレームでの超高速touch)
        foreach (Touch touch in Input.touches)
        {
            //今回は複数タッチを受け入れず、一つの指の入力のみを受け入れる
            if (touch.fingerId == 0)
            {
                touchPosition = GetTouchWorldPosition(touch);

                // タッチの状態に応じて処理
                switch (touch.phase)
                {
                    //タッチした瞬間であれば
                    case TouchPhase.Began:
                        if (!isDragging)
                        {
                            HandleTouchBegan(touch);
                        }
                        break;

                    //タッチしている間であれば
                    case TouchPhase.Moved:
                        if (isDragging)
                        {
                            HandleTouchMoved(touch);
                        }
                        break;

                    //タッチを終わらせたなら(指を離す)
                    case TouchPhase.Ended:
                        if (isDragging)
                        {
                            HandleTouchEnded(touch);
                        }
                        break;
                }
            }
        }
    }

    //タッチしたスクリーン座標をワールド座標に変換して返す関数
    Vector3 GetTouchWorldPosition(Touch touch)
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        touchPosition.z = Camera.main.nearClipPlane;
        return touchPosition;
    }

    void HandleTouchBegan(Touch touch)
    {
        Vector3 upCondition_view = new Vector3(0, 0.3f, touchPosition.z - Camera.main.transform.position.z);
        Vector3 upCondition = Camera.main.ViewportToWorldPoint(upCondition_view);
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touch.position; //スクリーン座標で指定することに注意
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            GameObject hitGameObject = result.gameObject;
            Debug.Log(hitGameObject);
            if (hitGameObject != null && !hitGameObject.CompareTag("UnderClickable")) return;
        }

        if (touchPosition.y < upCondition.y) return;
        if (singleGenerateManager.GetSingleGameObject() == null) return;
        draggedObject = singleGenerateManager.GetSingleGameObject().transform;
        draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //ブロックx座標をタッチしている座標に
        isDragging = true;
    }

    void HandleTouchMoved(Touch touch)
    {
        draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //ブロックx座標をタッチしている座標に
    }

    void HandleTouchEnded(Touch touch)
    {
        isDragging = false;
        draggedObject = null;
        singleGenerateManager.SetSingleGameObject(null); //このブロックがsingleGameObjectに入ったままにしていると、ボタンが押された瞬間にDestroyが呼ばれてしまう。
        this.enabled = false;
        this.tag = "PrimeNumberBlock";
        gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //レイヤーを変更することにより、初めて他のブロックと衝突するようになる。
        blockInfo.ChangeDynamic(); //重力の影響を受けるようにする。
        blockInfo.EnableCollider(); //ゲームオブジェクトの落下地点を視覚化する線の描画の際に一時的にコライダーを非活性化するので、ここでコライダーを復活させる。
        gameObject.transform.parent = afterField.transform;
        network.AddNode(gameObject);
    }
}
