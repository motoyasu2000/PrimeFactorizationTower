using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    //画面上部中央に表示される、生成可能なブロックの条件を記載した数値を正方形のブロック状に表示させる関数。色々な画面幅のあるスマホに動的に対応するため。
    public class NowUpCompositeNumberIntoSquare : MonoBehaviour
    {
        public RectTransform myRectTransform;
        private void Update()
        {
            AdjustSize();
        }
        private void AdjustSize()
        {

            float height = myRectTransform.rect.height;
            myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, height);

        }
        private void OnEnable()
        {
            myRectTransform = GetComponent<RectTransform>();
            AdjustSize();
        }
    }
}