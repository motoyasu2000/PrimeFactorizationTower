using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    //画面上部のおけるブロックを設定する合成数を表示したり条件の合成数を表示するためのUIの高さを計算するクラス。端末の画面幅に合わせて適切に計算できるようにする。
    public class FitScreenSize : MonoBehaviour
    {
        RectTransform rectTransform;
        float width;
        float canvasHeight;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            width = rectTransform.rect.width;
            canvasHeight = transform.parent.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
            rectTransform.sizeDelta = new Vector2(0, -canvasHeight + width / 4.2f); //-canvasHeightが上端の位置でそこから横幅/4.2分だけ下がる この処理によりUIのサイズをスマホの画面幅に限らず一定にする。
                                                                                    //Debug.Log(canvasHeight + " " + width);
        }
    }
}