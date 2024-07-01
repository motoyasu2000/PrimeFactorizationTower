using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MaterialScene内のセーブボタンのUIを操作するクラス
/// </summary>
public class SaveUIManager : MonoBehaviour
{
    static readonly Color saveColor_complete = Color.green;
    static readonly Color saveColor_incomplete = Color.red;
    Image saveUIImage;

    void Start()
    {
        saveUIImage = GetComponent<Image>();
    }

    /// <summary>
    /// マテリアルのパラメーターが保存されているか否かを可視化するために使用している。
    /// </summary>
    /// <param name="changesComplete">
    /// セーブされた状態の見た目に切り替える(true)か否(false)か
    /// マテリアルボタンの発火時や、マテリアルボタンの発火を内部で行うブロックの切り替えボタンのタップ時や、セーブ時に引数にtrue
    /// スライダーを通して値を変更したときにfalse
    /// </param>
    public void ChangeColor(bool changesComplete)
    {
        if(changesComplete) saveUIImage.color = saveColor_complete;
        else                saveUIImage.color = saveColor_incomplete;
    }
}
