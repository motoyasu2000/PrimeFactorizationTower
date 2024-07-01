using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シーン上にあるすべてのボタンに影をつけるクラス
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
            AddShadowToButton(button);
        }
    }

    void AddShadowToButton(Button button)
    {
        GameObject shadow = Instantiate(shadowPrefab);
        shadow.transform.SetParent(button.transform);
        RectTransform rectTransform = shadow.GetComponent<RectTransform>();
        rectTransform.offsetMin = Vector3.zero;
        rectTransform.offsetMax = Vector3.zero;
    }
}
