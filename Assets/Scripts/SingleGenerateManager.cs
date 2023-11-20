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
        //singleGameObject�ɉ����������Ă����ԂŌĂ΂ꂽ�ꍇ�ɂ�
        else
        {
            //���̃Q�[���I�u�W�F�N�g�̐��l�ƌォ�痈���Q�[���I�u�W�F�N�g�̐��l����v���Ă���Ȃ猳�̃Q�[���I�u�W�F�N�g����]
            if (singleGameObject.GetComponent<BlockInfo>().GetNumber() == setObject.GetComponent<BlockInfo>().GetNumber())
            {
                Debug.Log("��]");
                Destroy(setObject);
                rotateFlag = true;
            }
            //��v���Ă��Ȃ��Ȃ�Â����̃Q�[���I�u�W�F�N�g�������čX�V
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
        //null��flag��false�Ȃ�return
        if (singleGameObject == null || !rotateFlag) return;
        singleGameObject.transform.Rotate(0, 0, spinSpeed);
        rotateCounter += spinSpeed;
        if(rotateCounter >= dMaxAngle)
        {
            
            //�]���ɉ�]��������ǂ�B
            singleGameObject.transform.Rotate(0, 0, dMaxAngle - rotateCounter);
            //��]��Ԃ̏�����
            rotateCounter = 0;
            rotateFlag = false;
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleGameObject;
    }
}
