using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPrimeNumber : MonoBehaviour
{
    
    protected Vector3 initialPosition; //touch�����u�Ԃ̈ʒu�̎擾
    protected bool isDragging = false; //�h���b�O���Ă��邩�ǂ���
    protected Transform draggedObject = null; //���ܐG��Ă���I�u�W�F�N�g���i�[����ϐ��@Update����Raycast�𖈕b�s���Ă���̂ŁA
                                              //���󂵂Ă���Q�[���I�u�W�F�N�g���ύX����Ă��܂��\�������邽�߁A�h���b�O���̃I�u�W�F�N�g�݂̂��擾��������悤�ɂ��Ă���B
    protected BlockInfo blockInfo;

    private void Start()
    {
        blockInfo = GetComponent<BlockInfo>();
    }

    void Update()
    {
        //Input.touches�̓t���[�����Ƃ�touch�̐����擾����(�w�̖{����A�P�t���[���ł̒�����touch)
        foreach (Touch touch in Input.touches)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position); //���[���h���W�ɕϊ�
            touchPosition.z = Camera.main.nearClipPlane; //�N���b�s���O����Ȃ��悤��

            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            // �^�b�`�̏�Ԃɉ����ď���
            switch (touch.phase)
            {
                //�^�b�`�����u�Ԃł����
                case TouchPhase.Began:
                    if (!isDragging && hit.collider != null && hit.transform == this.transform)
                    {
                        isDragging = true; //�h���b�O��Ԃɂ��� (����������Ƃ���Ȃ�����)
                        draggedObject = hit.transform; //�h���b�O���̃I�u�W�F�N�g�����������������u�Ԃ̃I�u�W�F�N�g�ɐݒ�B
                        initialPosition = draggedObject.position; //�����ʒu�����������������̂������ʒu�ɐݒ�
                        DuplicatePrimeNumberBlock(); //���̈ʒu�ɕ������Ă���
                    }
                    break;

                //�^�b�`���Ă���Ԃł����
                case TouchPhase.Moved:
                    if (isDragging) //�����h���b�O���Ȃ�
                    {
                        draggedObject.position = touchPosition; //�u���b�N�̈ʒu���^�b�`���Ă�����W��
                    }
                    break;

                //�^�b�`���I��点���Ȃ�(�w�𗣂�)
                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        isDragging = false; //�h���b�O��Ԃ�����
                        draggedObject = null; //���̃I�u�W�F�N�g�����󂵂Ă��Ȃ���Ԃ�
                        this.enabled = false; //���̃X�N���v�g�̂������������������������A�^�b�`�ł��Ȃ��悤�ɂ���B
                        this.tag = "PrimeNumberBlock"; //�^�O��f���I�u�W�F�N�g�ɕύX����
                        gameObject.layer = LayerMask.NameToLayer("PrimeNumberBlock"); //���C���[���f���u���b�N�ɂ�����
                        blockInfo.AddRigidbody2D(); //�d�͂�������B
                    }
                    break;
            }
        }
    }


    //�N���b�N�����Ƃ��ɕ��������悤�ɂ��A���̂Ԃ���������ł����̈ʒu����h���b�O�ł���悤�ɂ���B
    public void DuplicatePrimeNumberBlock()
    {
        Instantiate(blockInfo.SelfPrefab, initialPosition, Quaternion.identity);
    }
}
