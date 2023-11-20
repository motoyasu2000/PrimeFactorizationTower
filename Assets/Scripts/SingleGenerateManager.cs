using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenerateManager : MonoBehaviour
{
    GameObject singleGameObject;
    float spinSpeed = 10f;
    float rotateCounter = 0;
    bool rotateFlag = false;

    private void Update()
    {
        RotateUntil(60);
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
            if (singleGameObject.GetComponent<BlockInfo>().GetNumber() == setObject.GetComponent<BlockInfo>().GetNumber())
            {
                Debug.Log("回転");
                Destroy(setObject);
                rotateFlag = true;
            }
            //一致していないなら古い方のゲームオブジェクトを消して更新
            else
            {
                Debug.Log($"oldnum: {singleGameObject.GetComponent<BlockInfo>().GetNumber()}  newnum: {setObject.GetComponent<BlockInfo>().GetNumber()}");
                Destroy(singleGameObject);
                singleGameObject = setObject;
            }
        }
    }

    void RotateUntil(float dMaxAngle)
    {
        //nullかflagがfalseならreturn
        if (singleGameObject == null || !rotateFlag) return;
        singleGameObject.transform.Rotate(0, 0, spinSpeed);
        rotateCounter += spinSpeed;
        if(rotateCounter >= dMaxAngle)
        {
            
            //余分に回転したらもどる。
            singleGameObject.transform.Rotate(0, 0, dMaxAngle - rotateCounter);
            //回転状態の初期化
            rotateCounter = 0;
            rotateFlag = false;
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleGameObject;
    }
}
