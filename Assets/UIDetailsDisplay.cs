using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDetailsDisplay : MonoBehaviour
{
    public RectTransform targetUIElement;
    public TextMeshProUGUI detailsText;

    void Update()
    {
        Vector2 size = targetUIElement.rect.size;
        Vector2 anchorSize = targetUIElement.sizeDelta;
        detailsText.text = "UI Size: " + size.ToString() + "\nAnchor Size: " + anchorSize.ToString();
    }
}
