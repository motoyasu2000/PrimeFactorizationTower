using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TouchPrimeNumber : MonoBehaviour
{
    protected GameObject selfPrefab; //自分自身のプレファブを格納する変数(継承先クラスから見た自分自身)
    protected Vector3 initialPosition; //touchした瞬間の位置の取得
    protected bool isDragging = false; //ドラッグしているかどうか
    protected Transform draggedObject = null; //いま触れているオブジェクトを格納する変数　Update内でRaycastを毎秒行っているので、
                                              //洗濯しているゲームオブジェクトが変更されてしまう可能性があるため、ドラッグ中のオブジェクトのみを取得し続けるようにしている。
    protected int myNumber; //自分の持つ数字。合成数とかの計算はこれを利用する
    void Update()
    {
        //Input.touchesはフレームごとのtouchの数を取得する(指の本数や、１フレームでの超高速touch)
        foreach (Touch touch in Input.touches)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); //ワールド座標に変換
            touchPosition.z = Camera.main.nearClipPlane; //クリッピングされないように

            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            // タッチの状態に応じて処理
            switch (touch.phase)
            {
                //タッチした瞬間であれば
                case TouchPhase.Began:
                    if (!isDragging && hit.collider != null && hit.transform == this.transform)
                    {
                        isDragging = true; //ドラッグ状態にする (もしかするといらないかも)
                        draggedObject = hit.transform; //ドラッグ中のオブジェクトをｔｏｕｃｈした瞬間のオブジェクトに設定。
                        initialPosition = draggedObject.position; //初期位置をｂｌｏｃｋが元のあった位置に設定
                        DuplicatePrimeNumberBlock(); //元の位置に複製しておく
                    }
                    break;

                //タッチしている間であれば
                case TouchPhase.Moved:
                    if (isDragging) //もしドラッグ中なら
                    {
                        draggedObject.position = touchPosition; //ブロックの位置をタッチしている座標に
                    }
                    break;

                //タッチを終わらせたなら(指を離す)
                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        isDragging = false; //ドラッグ状態を解除
                        draggedObject = null; //何のオブジェクトも洗濯していない状態に
                        this.enabled = false; //このスクリプトのｉｎｓｔａｎｃｅを消去し、タッチできないようにする。
                        this.tag = "PrimeNumberBlock"; //タグを素数オブジェクトに変更する
                        gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //レイヤーも素数ブロックにかえる
                        AddRigidbody2D(); //重力を加える。
                    }
                    break;
            }
        }
    }
    public abstract void SetSelfPrefab(); //自分自身のプレファブが何であるかは継承先のスクリプトで決定すべき
    public abstract void AddRigidbody2D(); //ブロックごとに重力のかけ方が違うかもしれないので、継承先のクラスで記述

    //自分自身の番号を設定するクラス。継承先のクラスのstart内に記述。
    public void SetMyNumber(int setNumber)
    {
        myNumber = setNumber;
    }

    //クリックしたときに複製されるようにし、このぶろっぐがいつでも元の位置からドラッグできるようにする。
    public void DuplicatePrimeNumberBlock()
    {
        Instantiate(selfPrefab, initialPosition, Quaternion.identity);
    }
}
