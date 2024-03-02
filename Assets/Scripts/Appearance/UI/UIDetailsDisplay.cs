using UnityEngine;
using UnityEngine.UI;
using TMPro;

////////////////////////////////////////////
//※実験用のクラス　今回のゲームには利用しない。///
////////////////////////////////////////////

//RectTransformの仕様について調査するクラス(勉強用)
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

