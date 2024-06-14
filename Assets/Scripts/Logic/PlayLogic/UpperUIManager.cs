using TMPro;
using UnityEngine;

/// <summary>
/// conditionやoriginを表示するテキストを管理するためのクラス
/// </summary>
public class UpperUIManager : MonoBehaviour
{
    /// <summary>
    /// Condition,Origin,NextOrigin
    /// </summary>
    public enum KindOfUI{
        Condition,
        Origin,
        NextOrigin,
    }

    TextMeshProUGUI conditionNumberText; //左上の条件テキスト
    TextMeshProUGUI originNumberText;
    TextMeshProUGUI nextOriginNumberText;

    public TextMeshProUGUI OriginNumberText => originNumberText;
    private void Awake()
    {
        conditionNumberText = GameObject.Find("ConditonNumberText").GetComponent<TextMeshProUGUI>();
        originNumberText = GameObject.Find("OriginNumberText").GetComponent<TextMeshProUGUI>();
        nextOriginNumberText = GameObject.Find("NextOriginNumberText").GetComponent<TextMeshProUGUI>();
    }

    public void ChangeDisplayText(KindOfUI kindOfUI,string str)
    {
        switch (kindOfUI)
        {
            case KindOfUI.Condition:
                conditionNumberText.text = str; 
                break;
            case KindOfUI.Origin:
                originNumberText.text = str; 
                break;
            case KindOfUI.NextOrigin:
                nextOriginNumberText.text = str;
                break;
            default:
                Debug.LogError("予期せぬKindOfUIが呼ばれました。");
                break;
        }
    }

    public void ChangeDisplayColor(KindOfUI kindOfUI, Color color)
    {
        switch (kindOfUI)
        {
            case KindOfUI.Condition:
                conditionNumberText.color = color;
                break;
            case KindOfUI.Origin:
                originNumberText.color = color;
                break;
            case KindOfUI.NextOrigin:
                nextOriginNumberText.color = color;
                break;
            default:
                Debug.LogError("予期せぬKindOfUIが呼ばれました。");
                break;
        }
    }

}
