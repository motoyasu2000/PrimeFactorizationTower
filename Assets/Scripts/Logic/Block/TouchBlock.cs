using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ブロックをタップやスライドによって操作できるようにするクラス。
/// ユーザーとのインタラクション部分。
/// </summary>
public class TouchBlock : MonoBehaviour
{
    //入力管理
    bool isDragging = false; //ドラッグしているかどうか
    Vector3 touchPosition; //現在タッチしている位置
    Transform draggedObject = null; //現在選択しているゲームオブジェクトを格納する変数　Update内でRaycastを毎秒行っているので、
                                    //選択するゲームオブジェクトが変更されないようにドラッグ中のオブジェクトのみを取得するようにしている。
    //ブロックの処理
    BlockInfo blockInfo;
    GraphicRaycaster graphicRaycaster;
    SingleBlockManager singleBlockManager; //ゲームオブジェクトが単一であることを保証するためのクラス
    GameObject primeNumberGeneratingPoint; //ゲームオブジェクトを生成する場所を示すゲームオブジェクト  
    GameObject blockField;
    GameObject primeNumberCheckField;

    //UI部分
    GameObject canvas;
    EventSystem eventSystem;

    //外部とのやり取り
    GameManager gameManager;

    private void Start()
    {
        Initialize();
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

                //タッチの状態に応じて処理
                switch (touch.phase)
                {
                    //タッチした瞬間であれば
                    case TouchPhase.Began:
                        if (!isDragging) HandleTouchBegan(touch);
                        break;

                    //タッチしている間であれば
                    case TouchPhase.Moved:
                        if (isDragging) HandleTouchMoved(touch);
                        break;

                    //タッチを終わらせたなら(指を離す)
                    case TouchPhase.Ended:
                        if (isDragging) HandleTouchEnded(touch);
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

    //UI上で無ければブロックの生成先を指定できるようにする。その後ドラッグで調整可能
    void HandleTouchBegan(Touch touch)
    {
        //ブロックを移動してよければ
        if (!CheckMovableBlock(touch)) return;

        //タッチした位置にブロックを移動する(x軸方向の移動のみ)
        MoveBlockX(touchPosition.x); //ブロックx座標をタッチしている座標に
        isDragging = true;
    }

    //指を触れている間はその指のx座標にブロックを動かす。
    void HandleTouchMoved(Touch touch)
    {
        MoveBlockX(touchPosition.x);
    }

    //指を話したときの処理、ブロックを落下させ、素数を持ったブロックとして機能するようにする。また、離したブロックをネットワークにノードとして追加する
    void HandleTouchEnded(Touch touch)
    {
        isDragging = false;
        BlockRelease();
    }

    /// <summary>
    /// 画面に触れた際、ブロックを移動できる状態にあるかをチェックする
    /// 上にブロックが存在しない場合や②指の下にUIが存在する場合にfalseを返す
    /// </summary>
    /// <returns>ブロックを移動しても良い(true)か否(false)か</returns>
    bool CheckMovableBlock(Touch touch)
    {
        //ブロックが上部に存在しない場合はブロックを移動する処理は行わない。
        if (singleBlockManager.SingleBlock == null) return false;

        //UI上をタッチしていないかのチェック、UIの上をタッチしている間はブロックを移動するべきではない。
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touch.position; //スクリーン座標で指定することに注意
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        //ヒットしたゲームオブジェクトにUIが含まれていたら処理を行わない。
        foreach (RaycastResult result in results)
        {
            GameObject hitGameObject = result.gameObject;
            Debug.Log(hitGameObject);
            if (hitGameObject != null && !hitGameObject.CompareTag("UnderClickable")) return false;
        }

        return true;
    }

    void MoveBlockX(float newX)
    {
        draggedObject = singleBlockManager.SingleBlock.transform;
        draggedObject.position = new Vector3(newX, primeNumberGeneratingPoint.transform.position.y, primeNumberGeneratingPoint.transform.position.z);
    }

    void BlockRelease()
    {
        draggedObject = null;
        singleBlockManager.SeparateSingleBlockManager(); //SingleGenerateMangerによって次にほかのボタンを押した際にDestroyが呼ばれないように。
        this.enabled = false;
        this.tag = "PrimeNumberBlock";
        gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //レイヤーを変更することにより、初めて他のブロックと衝突するようになる。
        blockInfo.ChangeDynamic(); //重力の影響を受けるようにする。
        StartCoroutine(blockInfo.EnableCollider()); //ゲームオブジェクトの落下地点を視覚化する線の描画のためにコライダーを非活性化しているので復活させる。
        gameObject.transform.parent = primeNumberCheckField.transform;
        BlocksGraphData.AddBlock(gameObject);
        gameManager.SetUpDropTurn();
    }

    //AIが操作する
    public void MoveBlockXAndRelease(float newX)
    {
        MoveBlockX(newX);
        BlockRelease();
    }

    public void Initialize()
    {
        //初期化
        blockInfo = GetComponent<BlockInfo>();
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleBlockManager = primeNumberGeneratingPoint.GetComponent<SingleBlockManager>();
        blockField = GameObject.Find("BlockField");
        primeNumberCheckField = blockField.transform.Find("PrimeNumberCheckField").gameObject;
        canvas = GameObject.Find("Canvas");
        eventSystem = FindObjectOfType<EventSystem>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
}
