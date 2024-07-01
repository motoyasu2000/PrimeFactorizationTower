using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シーン上にあるあらゆる種類のボタンに影をつけるクラス(現状はImageのspriteがUISpriteのもののみ)
/// </summary>
public class ButtonShadowAdder : MonoBehaviour
{
    Button[] allButton;
    GameObject shadowPrefab;
    void Start()
    {
        allButton = FindObjectsOfType<Button>();
        shadowPrefab = Resources.Load("Shadow") as GameObject;
        foreach(var button in allButton)
        {
            Sprite sourceImage = button.GetComponent<Image>().sprite;
            if(sourceImage.name == "UISprite") AddShadowToButton_UISprite(button);
        }
    }

    void AddShadowToButton_UISprite(Button button)
    {
        GameObject shadow = Instantiate(shadowPrefab);
        shadow.transform.SetParent(button.transform);
        RectTransform rectTransform = shadow.GetComponent<RectTransform>();
        rectTransform.offsetMin = Vector3.zero;
        rectTransform.offsetMax = Vector3.zero;
    }
}
