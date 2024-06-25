using UnityEngine;

/// <summary>
/// 落下前の単一のブロックを回転させるクラス。
/// </summary>
public class BlockSpiner : MonoBehaviour
{
    SingleBlockManager singleBlockManager;
    private void Start()
    {
        singleBlockManager = GetComponent<SingleBlockManager>();
    }
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
        GameObject singleBlock = singleBlockManager.SingleBlock;
        if(singleBlock == null) Debug.LogError($"singleblockはnullです");
        if (singleBlock != null)
        singleBlock.transform.Rotate(Vector3.forward * angleOfRotation);
    }

}
