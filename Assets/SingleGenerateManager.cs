using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenerateManager : MonoBehaviour
{
    GameObject singleGameObject;

    public void SetSingleGameObject(GameObject setObject)
    {
        //������null�Ȃ�singleGameObject��null�ɂ��ď������I��
        if(setObject == null)
        {
            singleGameObject = null;
            return;
        }

        //singleGameObject�����Ƃ���null�Ȃ畁�ʂɑ��
        if(singleGameObject == null)
        {
            singleGameObject = setObject;
        }
        //singleGameObject�ɉ����������Ă����ԂŌĂ΂ꂽ�ꍇ�ɂ͌����폜���Ĉ����̂��̂ɍX�V
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
