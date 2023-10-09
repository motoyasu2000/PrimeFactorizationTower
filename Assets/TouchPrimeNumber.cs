using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPrimeNumber : MonoBehaviour
{
    
    protected Vector3 initialPosition; //touchした瞬間の位置の取得
    protected bool isDragging = false; //ドラッグしているかどうか
    protected Transform draggedObject = null; //いま触れているオブジェクトを格納する変数　Update内でRaycastを毎秒行っているので、
                                              //洗濯しているゲームオブジェクトが変更されてしまう可能性があるため、ドラッグ中のオブジェクトのみを取得し続けるようにしている。
    protected BlockInfo blockInfo; //ブロックに関わる様々な情報が格納されたクラス
    protected GameObject primeNumberGeneratingPoint; //ボタンを押した瞬間のblockが生成される地点が格納されたゲームオブジェクト、ゲームオブジェクトが単一であることを保証するためのcomponentがアタッチしてある。
    protected SingleGenerateManager singleGenerateManager; //ゲームオブジェクトが単一であることを保証するためのクラス
    protected GameManager gameManager;
    private void Start()
    {
        blockInfo = GetComponent<BlockInfo>();
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
                            if (EventSystem.current.IsPointerOverGameObject(0)) return;
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
                            blockInfo.AddRigidbody2D(); //重力を加える。
                        }
                        break;
                }
            }
        }
    }
}
