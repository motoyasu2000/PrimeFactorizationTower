using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブロックの最高地点の高さを計算するクラス
/// 一定以上の高さになるとカメラの範囲が大きくなって上に行く必要があり、スコアを計算するためにも高さの情報は必要となる。
/// </summary>
public class MaxHeightCalculator : MonoBehaviour
{
    float nowHeight = 0;
    public float NowHeight => nowHeight;

    GameObject blockField;
    GameObject primeNumberCheckField;
    GameObject completedField;

    private void Start()
    {
        blockField = GameObject.Find("BlockField");
        primeNumberCheckField = blockField.transform.Find("PrimeNumberCheckField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
    }

    /// <summary>
    /// 全Blockの頂点から最も高い頂点のy座標を返すメソッド(GameObjectのpivotではなく、頂点レベルで高さを計算する。)
    /// </summary>
    /// <returns>最も高いBlockの頂点</returns>
    public float CalculateAllGameObjectsMaxHeight()
    {
        List<Vector3> allVertices = new List<Vector3>();
        if (primeNumberCheckField != null)
        {
            foreach (Transform block in primeNumberCheckField.transform)
            {
                nowHeight = Mathf.Max(nowHeight, CalculateGameObjectMaxHeight(block.gameObject)); //現在見ているゲームオブジェクトの最も高い頂点とmaxの比較                                                                          //Debug.Log(block.gameObject.name);
            }
        }
        if (completedField != null)
        {
            foreach (Transform block in completedField.transform)
            {
                nowHeight = Mathf.Max(nowHeight, CalculateGameObjectMaxHeight(block.gameObject)); //現在見ているゲームオブジェクトの最も高い頂点とmaxの比較
            }
        }
        return nowHeight;
    }

    /// <summary>
    /// 指定されたBlockの全頂点の中で、最も高い頂点のy座標を計算し、返す
    /// </summary>
    /// <param name="block">どのブロックの最も高い頂点を計算するか</param>
    /// <returns>引数で受け取ったブロックの最も高い頂点</returns>
    float CalculateGameObjectMaxHeight(GameObject block)
    {
        Vector2[] vertices = block.GetComponent<SpriteRenderer>().sprite.vertices;
        float max = 0;
        foreach (Vector2 vartex in vertices)
        {
            Vector2 worldPoint = block.transform.TransformPoint(vartex);
            max = Mathf.Max(max, worldPoint.y);
            //Debug.Log(worldPoint.y);
        }
        return max - GameInfo.GroundHeight; //初めの高さを0にするために元の高さ分引く
    }
}
