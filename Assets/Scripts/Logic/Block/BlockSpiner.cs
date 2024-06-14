using UnityEngine;

/// <summary>
/// 落下前の単一のブロックを回転させるクラス。
/// </summary>
public class BlockSpiner : MonoBehaviour
{
    //落下前の単一のブロックを反時計回りに45度回転させる
    public void SpinSingleBlock_45()
    {
        //StartCoroutine(RotateSingleBlock(45));
        SpinSingleBlock(45);
    }
    //落下前の単一のブロックを反時計回りに45度回転させる
    public void SpinSingleBlock_45_Reverse()
    {
        //StartCoroutine(RotateSingleBlock(-45));
        SpinSingleBlock(-45);
    }
    void SpinSingleBlock(float angleOfRotation)
    {
        GameObject singleBlock = GetComponent<SingleGenerateManager>().SingleBlock;
        if (singleBlock != null)
        {
            singleBlock.transform.Rotate(Vector3.forward * angleOfRotation);
        }
        else
        {
            Debug.LogError($"singleblockはnullです");
        }
    }

}
