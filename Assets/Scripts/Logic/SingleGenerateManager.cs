using Common;
using UnityEngine;

/// <summary>
/// ブロックの生成が単一であることを保証するクラス
/// </summary>

public class SingleGenerateManager : MonoBehaviour
{
    static readonly float dropHeightAbovePeak = 3f; //積み木の最高地点から見た相対的な高さ
    Vector3 defaultPoint; //初期位置
    GameObject singleBlock;
    MaxHeightCalculator maxHeightCalculator; //高さの計算を行う
    public GameObject SingleBlock => singleBlock;
    public Vector3 GeneratingPoint => new Vector3(defaultPoint.x, maxHeightCalculator.NowHeight + dropHeightAbovePeak, defaultPoint.z);
    private void Awake()
    {
        defaultPoint = transform.position;
        maxHeightCalculator = GameObject.Find("MaxHeightCalculator").GetComponent<MaxHeightCalculator>();
    }
    private void Update()
    {
        MoveBlockGenerationPoint();
    }

    /// <summary>
    /// 複数のブロックが同時に生成されないよう、異なるブロックを生成しようとしたとき、初めにあったブロックの方が削除され、上書きされるようにする。
    /// </summary>
    /// <param name="newBlock">新たに生成したいブロック</param>
    public void GenerateSingleBlock(GameObject newBlock)
    {
        //引数がnullならsingleGameObjectをnullにして処理を終了
        if(newBlock == null)
        {
            singleBlock = null;
            return;
        }

        //singleGameObjectがもともとnullなら普通に代入
        if(singleBlock == null)
        {
            singleBlock = newBlock;
        }
        //singleGameObjectに何かが入っている状態で呼ばれた場合には
        else
        {
            //ブロックの数値が一致していなければ、元ブロックを消去して更新
            if(singleBlock.GetComponent<BlockInfo>().GetPrimeNumber() != newBlock.GetComponent<BlockInfo>().GetPrimeNumber())
            {
                //Debug.Log($"oldnum: {singleGameObject.GetComponent<BlockInfo>().GetNumber()}  newnum: {setObject.GetComponent<BlockInfo>().GetNumber()}");
                Destroy(singleBlock);
                singleBlock = newBlock;
            }
            //一致していたら、新しい方を消して更新を行わない。
            else
            {
                Destroy(newBlock);
            }
        }
    }

    public GameObject GetSingleBlock()
    {
        return singleBlock;
    }

    //ブロックの生成地点をブロックの高さに合わせて変更する
    void MoveBlockGenerationPoint()
    {
        if(maxHeightCalculator.NowHeight < GameInfo.CameraTrackingStartHeight) return;
        transform.position = GeneratingPoint; //最も高いブロックよりより一定数(dropHeightAbovePeak)上にブロックを生成
    }
}
