using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//落下前の単一のブロックを回転させるクラス。
public class BlockSpiner : MonoBehaviour
{
    const float spinSpeed = 1000f;
    float angleCounter = 0; //どのくらい回転したのか数える関数
    bool isSpiningNow = false;

    //落下前の単一のブロックを反時計回りに45度回転させる
    public void RotateSingleBlock_45()
    {
        StartCoroutine(RotateSingleBlock(45));
    }
    //落下前の単一のブロックを反時計回りに45度回転させる
    public void RotateSingleBlock_45_Reverse()
    {
        StartCoroutine(RotateSingleBlock(-45));
    }
    //落下前の単一のブロックを反時計回りに90度回転させる
    public void RotateSingleBlock_90()
    {
        StartCoroutine(RotateSingleBlock(90));
    }

    //引数で与えられた数値分だけ落下前の単一のブロックを反時計回りに回転させる
    IEnumerator RotateSingleBlock(float angleOfRotation)
    {
        GameObject singleBlock = GetComponent<SingleGenerateManager>().SingleBlock;
        //回転中に新たな回転が行われないように
        if (!isSpiningNow && singleBlock != null)
        {
            isSpiningNow = true;
            while (true)
            {
                float nowFrameRotateValue = spinSpeed * Time.deltaTime;
                angleCounter += nowFrameRotateValue;
                singleBlock.transform.Rotate(Vector3.forward * nowFrameRotateValue);

                //指定の角度以上に回転したら、指定の角度との差分だけ戻って、angleCounterをリセットし、処理を終了。
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
