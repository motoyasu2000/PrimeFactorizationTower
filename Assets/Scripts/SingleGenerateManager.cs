using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleGenerateManager : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    GameObject singleGameObject;
    float spinSpeed = 20f;
    float rotateCounter = 0;
    bool rotateFlag = false;
    Camera mainCamera;
    CameraCtrl mainCameraCtrl;
    Vector3 defo; //�����ʒu
    private void Start()
    {
        mainCamera = Camera.main;
        mainCameraCtrl = mainCamera.GetComponent<CameraCtrl>();
        defo = transform.position;
        scoreManager = ScoreManager.ScoreManagerInstance;
    }
    private void Update()
    {
        RotateUntil(45);
        MoveSingleGameObjectPoint();
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
                //Debug.Log("��]");
                Destroy(setObject);
                rotateFlag = true;
            }
            //��v���Ă��Ȃ��Ȃ�Â����̃Q�[���I�u�W�F�N�g�������čX�V
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
        //null��flag��false�Ȃ�return
        if (singleGameObject == null || !rotateFlag) return;
        singleGameObject.transform.Rotate(0, 0, -spinSpeed);
        rotateCounter += spinSpeed;
        if(rotateCounter >= dMaxAngle)
        {
            
            //�]���ɉ�]��������ǂ�B
            singleGameObject.transform.Rotate(0, 0, rotateCounter - dMaxAngle);
            //��]��Ԃ̏�����
            rotateCounter = 0;
            rotateFlag = false;
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleGameObject;
    }

    //�u���b�N�̐����n�_���Q�[���̎��s���ɕύX���郁�\�b�h
    void MoveSingleGameObjectPoint()
    {
        if(scoreManager.MaxHeight < mainCameraCtrl.StartHeight) return;
        transform.position = new Vector3(defo.x,scoreManager.MaxHeight + 3, defo.z); //�ł������Ԃ�������3��Ƀu���b�N�𐶐�
    }
}
