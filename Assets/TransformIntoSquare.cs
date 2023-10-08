using UnityEngine;
using UnityEngine.UI;

public class TransformIntoSquare : MonoBehaviour
{
    public RectTransform myRectTransform;

    //Start�ŉ摜��}�����邽�߁A������Awake
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


