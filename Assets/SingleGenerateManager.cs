using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenerateManager : MonoBehaviour
{
    GameObject singleGameObject;

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
        //singleGameObjectに何かが入っている状態で呼ばれた場合には元を削除して引数のものに更新
        else
        {
            Destroy(singleGameObject);
            singleGameObject = setObject;
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleGameObject;
    }
}
