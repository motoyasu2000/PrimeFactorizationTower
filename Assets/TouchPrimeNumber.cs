using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPrimeNumber : MonoBehaviour
{
    
    protected Vector3 initialPosition; //touch�����u�Ԃ̈ʒu�̎擾
    protected bool isDragging = false; //�h���b�O���Ă��邩�ǂ���
    protected Transform draggedObject = null; //���ܐG��Ă���I�u�W�F�N�g���i�[����ϐ��@Update����Raycast�𖈕b�s���Ă���̂ŁA
                                              //���󂵂Ă���Q�[���I�u�W�F�N�g���ύX����Ă��܂��\�������邽�߁A�h���b�O���̃I�u�W�F�N�g�݂̂��擾��������悤�ɂ��Ă���B
    protected BlockInfo blockInfo; //�u���b�N�Ɋւ��l�X�ȏ�񂪊i�[���ꂽ�N���X
    protected GameObject primeNumberGeneratingPoint; //�{�^�����������u�Ԃ�block�����������n�_���i�[���ꂽ�Q�[���I�u�W�F�N�g�A�Q�[���I�u�W�F�N�g���P��ł��邱�Ƃ�ۏ؂��邽�߂�component���A�^�b�`���Ă���B
    protected SingleGenerateManager singleGenerateManager; //�Q�[���I�u�W�F�N�g���P��ł��邱�Ƃ�ۏ؂��邽�߂̃N���X
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
        //Input.touches�̓t���[�����Ƃ�touch�̐����擾����(�w�̖{����A�P�t���[���ł̒�����touch)
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); //���[���h���W�ɕϊ�
                touchPosition.z = Camera.main.nearClipPlane; //�N���b�s���O����Ȃ��悤��

                // �^�b�`�̏�Ԃɉ����ď���
                switch (touch.phase)
                {
                    //�^�b�`�����u�Ԃł����
                    case TouchPhase.Began:
                        if (!isDragging)
                        {
                            if (EventSystem.current.IsPointerOverGameObject(0)) return;
                            if (singleGenerateManager.GetSingleGameObject() == null) return;
                            draggedObject = singleGenerateManager.GetSingleGameObject().transform;
                            draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //�u���b�Nx���W���^�b�`���Ă�����W��
                            isDragging = true; //�h���b�O��Ԃɂ��� (����������Ƃ���Ȃ�����)
                        }
                        break;

                    //�^�b�`���Ă���Ԃł����
                    case TouchPhase.Moved:
                        if (isDragging) //�����h���b�O���Ȃ�
                        {
                            draggedObject.position = new Vector3(touchPosition.x, primeNumberGeneratingPoint.transform.position.y, touchPosition.z); //�u���b�Nx���W���^�b�`���Ă�����W��
                        }
                        break;

                    //�^�b�`���I��点���Ȃ�(�w�𗣂�)
                    case TouchPhase.Ended:
                        if (isDragging)
                        {
                            isDragging = false; //�h���b�O��Ԃ�����
                            draggedObject = null; //���̃I�u�W�F�N�g�����󂵂Ă��Ȃ���Ԃ�
                            singleGenerateManager.SetSingleGameObject(null); //���̃u���b�N��singleGameObject�ɓ������܂܂ɂ��Ă���ƁA�{�^���������ꂽ�u�Ԃ�Destroy���Ă΂�Ă��܂��B
                            this.enabled = false; //���̃X�N���v�g�̂������������������������A�^�b�`�ł��Ȃ��悤�ɂ���B
                            this.tag = "PrimeNumberBlock"; //�^�O��f���I�u�W�F�N�g�ɕύX����
                            gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //���C���[���f���u���b�N�ɂ�����
                            blockInfo.AddRigidbody2D(); //�d�͂�������B
                        }
                        break;
                }
            }
        }
    }
}
