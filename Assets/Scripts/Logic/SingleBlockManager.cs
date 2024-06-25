using Common;
using UnityEngine;

/// <summary>
/// ボタンをタップした際に生成される単一のゲームオブジェクトを管理するためのクラス。
/// 生成されるブロックが単一であることを保証し、最高のブロックの高さに合わせてブロックの生成ポイントも指定する。
/// </summary>

public class SingleBlockManager : MonoBehaviour
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
    /// ブロックの生成地点をブロックの最高地点の高さに合わせて変更する
    /// </summary>
    void MoveBlockGenerationPoint()
    {
        if(maxHeightCalculator.NowHeight < GameInfo.CameraTrackingStartHeight) return;
        transform.position = GeneratingPoint; //最も高いブロックよりより一定数(dropHeightAbovePeak)上にブロックを生成
    }

    /// <summary>
    /// 生成したブロックが単一であることを保証する
    /// ボタンをクリックし、生成されたブロックが引数に入る
    /// </summary>
    /// <param name="newGeneratedBlock">新たに生成したブロック</param>
    public void EnsureBlockIsSingle(GameObject newGeneratedBlock)
    {
        if (newGeneratedBlock == null) Debug.LogError("Nullのオブジェクトを生成しようとしました。");

        //元のSingleBlockを消して更新する
        UpdateSingleBlock(newGeneratedBlock);
    }

    /// <summary>
    /// 現在のsingleBlockを消去し、引数で受け取ったnewBlockをsingleBlockに変更することで、ブロックの更新を行う。
    /// </summary>
    /// <param name="newBlock">新しく生成するブロック</param>
    void UpdateSingleBlock(GameObject newBlock)
    {
        Destroy(singleBlock);
        singleBlock = newBlock;
    }

    /// <summary>
    /// singleブロックをSingleBlockManagerの管理が行き届かなくなるように切り離す。
    /// </summary>
    public void SeparateSingleBlockManager()
    {
        singleBlock = null;
    }
}
