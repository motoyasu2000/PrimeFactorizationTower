using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ブロックの最高地点の高さを計算するクラス
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

    //全ゲームオブジェクトの頂点から最も高い頂点のy座標を返すメソッド(GameObjectのpivotではなく、頂点レベルで高さを計算する。)
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

    //指定されたゲームオブジェクトの全頂点の中で、最も高い頂点のy座標を計算し、返す
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
