using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����O�̒P��̃u���b�N����]������N���X�B
public class BlockSpiner : MonoBehaviour
{
    const float spinSpeed = 1000f;
    float angleCounter = 0; //�ǂ̂��炢��]�����̂�������֐�
    bool isSpiningNow = false;

    //�����O�̒P��̃u���b�N�𔽎��v����45�x��]������
    public void RotateSingleBlock_45()
    {
        StartCoroutine(RotateSingleBlock(45));
    }
    //�����O�̒P��̃u���b�N�𔽎��v����45�x��]������
    public void RotateSingleBlock_45_Reverse()
    {
        StartCoroutine(RotateSingleBlock(-45));
    }
    //�����O�̒P��̃u���b�N�𔽎��v����90�x��]������
    public void RotateSingleBlock_90()
    {
        StartCoroutine(RotateSingleBlock(90));
    }

    //�����ŗ^����ꂽ���l�����������O�̒P��̃u���b�N�𔽎��v���ɉ�]������
    IEnumerator RotateSingleBlock(float angleOfRotation)
    {
        GameObject singleBlock = GetComponent<SingleGenerateManager>().SingleBlock;
        //��]���ɐV���ȉ�]���s���Ȃ��悤��
        if (!isSpiningNow && singleBlock != null)
        {
            isSpiningNow = true;
            while (true)
            {
                float nowFrameRotateValue = spinSpeed * Time.deltaTime;
                angleCounter += nowFrameRotateValue;
                singleBlock.transform.Rotate(Vector3.forward * nowFrameRotateValue);

                //�w��̊p�x�ȏ�ɉ�]������A�w��̊p�x�Ƃ̍��������߂��āAangleCounter�����Z�b�g���A�������I���B
                if (angleCounter > angleOfRotation)
                {
                    singleBlock.transform.Rotate(Vector3.forward * (angleOfRotation - angleCounter));
                    angleCounter = 0;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            isSpiningNow = false;
        }
    }
}
