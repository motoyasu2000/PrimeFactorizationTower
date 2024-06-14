using UnityEngine;

namespace UI
{
    /// <summary>
    /// 自分自身の縦幅を取得し、横幅に適用することで、長方形のUIから正方形のUIに変更するクラス。
    /// 色々な画面幅のあるスマホに動的に対応するために必要。
    /// </summary>

    public class FromRectangleToSquare : MonoBehaviour
    {
        RectTransform myRectTransform;

        private void Start()
        {
            myRectTransform = GetComponent<RectTransform>();
        }
        private void Update()
        {
            AdjustSize();
        }
        private void AdjustSize()
        {
            float height = myRectTransform.rect.height;
            myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, height);
        }
    }
}