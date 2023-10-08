using UnityEngine;
using UnityEngine.UI;

public class TransformIntoSquare : MonoBehaviour
{
    public RectTransform myRectTransform;

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


