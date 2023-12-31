using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchBlock : MonoBehaviour
{
    
    protected Vector3 initialPosition; //touchした瞬間の位置の取得
    protected bool isDragging = false; //ドラッグしているかどうか
    protected Transform draggedObject = null; //いま触れているオブジェクトを格納する変数　Update内でRaycastを毎秒行っているので、
                                              //洗濯しているゲームオブジェクトが変更されてしまう可能性があるため、ドラッグ中のオブジェクトのみを取得し続けるようにしている。
    protected BlockInfo blockInfo; //ブロックに関わる様々な情報が格納されたクラス
    protected GameObject primeNumberGeneratingPoint; //ボタンを押した瞬間のblockが生成される地点が格納されたゲームオブジェクト、ゲームオブジェクトが単一であることを保証するためのcomponentがアタッチしてある。
    protected SingleGenerateManager singleGenerateManager; //ゲームオブジェクトが単一であることを保証するためのクラス
    protected GameManager gameManager;
    GameObject blockField;
    GameObject afterField;

    NetWork netWork;

    GameObject canvas;
    EventSystem eventSystem;
    GraphicRaycaster graphicRaycaster;
    private void Start()
    {
        blockInfo = GetComponent<BlockInfo>();
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        blockField = GameObject.Find("BlockField");
        afterField = blockField.transform.Find("AfterField").gameObject;
        netWork = GameObject.Find("NetWork").GetComponent<NetWork>();
        canvas = GameObject.Find("Canvas");
        eventSystem = FindObjectOfType<EventSystem>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        //Input.touchesはフレームごとのtouchの数を取得する(指の本数や、１フレームでの超高速touch)
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); //ワールド座標に変換
                touchPosition.z = Camera.main.nearClipPlane; //クリッピングされないように

                // タッチの状態に応じて処理
                switch (touch.phase)
                {
                    //タッチした瞬間であれば
                    case TouchPhase.Began:
                        if (!isDragging)
                        {
                            Vector3 upCondition_view = new Vector3(0, 0.3f, touchPosition.z - Camera.main.transform.position.z);
                            Vector3 upCondition = Camera.main.ViewportToWorldPoint(upCondition_view);

                            //Debug.Log($"tatchpos.y : {touchPosition.y}");
                            //Debug.Log($"upCondition : {upCondition.y}");

                            /////////////////////////////////////EventSystem.current.currentSelectedGameObjectはボタンやスライダーなどのクリック可能なUIのみ取得可能////////////////////////////////////////////////
                            //if (EventSystem.current.IsPointerOverGameObject(0))
                            //{
                            //    GameObject uppestUI = EventSystem.current.currentSelectedGameObject;
                            //    Debug.Log(uppestUI);
                            //    if(uppestUI != null && !uppestUI.CompareTag("UnderClickable")) return;
                            //}
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            PointerEventData pointerEventData = new PointerEventData(eventSystem);
                            pointerEventData.position = touch.position; //スクリーン座標で指定することに注意
                            List<RaycastResult> results = new List<RaycastResult>();
                            graphicRaycaster.Raycast(pointerEventData, results);

                            foreach(RaycastResult result in results)
                            {
                                GameObject hitGameObject = result.gameObject;
                                Debug.Log(hitGameObject);
                                if (hitGameObject != null && !hitGameObject.CompareTag("UnderClickable")) return;
                            }

                            if (touchPosition.y < upCondition.y) return;
                            if (singleGenerateManager.GetSingleGameObject() == null) return;
                            draggedObject = singleGenerateManager.GetSingleGameObject().transform;
                            draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //ブロックx座標をタッチしている座標に
                            isDragging = true; //ドラッグ状態にする (もしかするといらないかも)
                        }
                        break;

                    //タッチしている間であれば
                    case TouchPhase.Moved:
                        if (isDragging) //もしドラッグ中なら
                        {
                            draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //ブロックx座標をタッチしている座標に
                        }
                        break;

                    //タッチを終わらせたなら(指を離す)
                    case TouchPhase.Ended:
                        if (isDragging)
                        {
                            isDragging = false; //ドラッグ状態を解除
                            draggedObject = null; //何のオブジェクトも洗濯していない状態に
                            singleGenerateManager.SetSingleGameObject(null); //このブロックがsingleGameObjectに入ったままにしていると、ボタンが押された瞬間にDestroyが呼ばれてしまう。
                            this.enabled = false; //このスクリプトのｉｎｓｔａｎｃｅを消去し、タッチできないようにする。
                            this.tag = "PrimeNumberBlock"; //タグを素数オブジェクトに変更する
                            gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //レイヤーも素数ブロックにかえる
                            blockInfo.ChangeDynamic(); //重力の影響を受けるようにする。
                            blockInfo.EnableCollider(); //ラインの描画の際に一時的にコライダーを非表示にするので、ここでコライダーを復活させる。
                            gameObject.transform.parent = afterField.transform;
                            netWork.AddNode(gameObject);//指を話した瞬間にブロックをノードとしてネットワークに追加する。
                        }
                        break;
                }
            }
        }
    }
}
