using UnityEngine;
using UnityEngine.UI;

public class TransformIntoSquare : MonoBehaviour
{
    public RectTransform myRectTransform;

    //Start‚Å‰æ‘œ‚ğ‘}“ü‚·‚é‚½‚ßA‚±‚±‚ÍAwake
    private void Awake()
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


