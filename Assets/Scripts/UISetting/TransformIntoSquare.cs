using UnityEngine;
using UnityEngine.UI;

public class TransformIntoSquare : MonoBehaviour
{
    public RectTransform myRectTransform;

    //Startで画像を挿入するため、ここはAwake
    private void Awake()
    {

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

    private void OnEnable()
    {
        myRectTransform = GetComponent<RectTransform>();
        AdjustSize();
    }
}


