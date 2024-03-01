using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ブロックの生成が単一であることを保証するクラス
//複数のブロックが同時に生成されないよう、異なるブロックを生成仕様としたとき、初めにあったブロックの方が削除され、上書きされるようにする。
//既にあるブロックと同じボタンを押した場合には、初めにあったブロックが時計回りに回転する。(ここは後に他のクラスで管理できるようにする。)
public class SingleGenerateManager : MonoBehaviour
{
    ScoreManager scoreManager;
    GameObject singleGameObject;
    float spinSpeed = 20f;
    float rotateCounter = 0;
    bool rotateFlag = false;
    Camera mainCamera;
    Vector3 defaultPoint; //初期位置
    private void Start()
    {
        mainCamera = Camera.main;
        defaultPoint = transform.position;
        scoreManager = ScoreManager.ScoreManagerInstance;
    }
    private void Update()
    {
        RotateUntil(45);
        MoveSingleGameObjectPoint();
    }
    public void SetSingleGameObject(GameObject setObject)
    {
        //引数がnullならsingleGameObjectをnullにして処理を終了
        if(setObject == null)
        {
            singleGameObject = null;
            return;
        }

        //singleGameObjectがもともとnullなら普通に代入
        if(singleGameObject == null)
        {
            singleGameObject = setObject;
        }
        //singleGameObjectに何かが入っている状態で呼ばれた場合には
        else
        {
            //元のゲームオブジェクトの数値と後から来たゲームオブジェクトの数値が一致しているなら元のゲームオブジェクトを回転
            if (singleGameObject.GetComponent<BlockInfo>().GetPrimeNumber() == setObject.GetComponent<BlockInfo>().GetPrimeNumber())
            {
                //Debug.Log("回転");
                Destroy(setObject);
                rotateFlag = true;
            }
            //一致していないなら古い方のゲームオブジェクトを消して更新
            else
            {
                //Debug.Log($"oldnum: {singleGameObject.GetComponent<BlockInfo>().GetNumber()}  newnum: {setObject.GetComponent<BlockInfo>().GetNumber()}");
                Destroy(singleGameObject);
                singleGameObject = setObject;
            }
        }
    }

    void RotateUntil(float dMaxAngle)
    {
        //nullかflagがfalseならreturn
        if (singleGameObject == null || !rotateFlag) return;
        singleGameObject.transform.Rotate(0, 0, -spinSpeed);
        rotateCounter += spinSpeed;
        if(rotateCounter >= dMaxAngle)
        {
            
            //余分に回転したらもどる。
            singleGameObject.transform.Rotate(0, 0, rotateCounter - dMaxAngle);
            //回転状態の初期化
            rotateCounter = 0;
            rotateFlag = false;
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleGameObject;
    }

    //ブロックの生成地点をゲームの実行中に変更するメソッド
    void MoveSingleGameObjectPoint()
    {
        if(scoreManager.NowHeight < Info.cameraTrackingStartHeight) return;
        transform.position = new Vector3(defaultPoint.x,scoreManager.NowHeight + 3, defaultPoint.z); //最も高いぶろっぐより3つ上にブロックを生成
    }
}
