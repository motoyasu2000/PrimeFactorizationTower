using UnityEngine;
using UnityEngine.UI;

public class TransformIntoSquare : MonoBehaviour
{
    public RectTransform myRectTransform;

    //Startで画像を挿入するため、ここはAwake
    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        AdjustSize();
    }
    private void Update()
    {
        
    }

    private void AdjustSize()
    {

        float height = myRectTransform.rect.height;
        myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, height);

    }
}


