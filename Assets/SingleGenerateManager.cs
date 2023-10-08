using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenerateManager : MonoBehaviour
{
    GameObject singleGameObject;

    public void SetSingleGameObject(GameObject setObject)
    {
        if(singleGameObject == null)
        {
            singleGameObject = setObject;
        }
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
