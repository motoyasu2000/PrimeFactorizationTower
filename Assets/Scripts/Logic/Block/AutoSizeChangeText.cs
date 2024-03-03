using UnityEngine;

//ブロックに表示される文字のサイズを調整するクラス
//文字はブロックの子要素となっており、ブロックのScaleに応じて文字サイズを小さくする。
//具体的には親ブロックのScaleの逆数をとる。
public class AutoSizeChangeText : MonoBehaviour
{
    void Start()
    {
        float px = gameObject.transform.parent.localScale.x;
        float py = gameObject.transform.parent.localScale.y;
        float pz = gameObject.transform.parent.localScale.z;
        gameObject.transform.localScale = new Vector3(1/px,1/py,1/pz);
    }
}
