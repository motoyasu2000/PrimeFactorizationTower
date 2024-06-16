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

    //画面上部に表示される、condition,origin,nextOrigin
    TextMeshProUGUI conditionNumberText;
    TextMeshProUGUI originNumberText;
    TextMeshProUGUI nextOriginNumberText;

    private void Awake()
    {
        conditionNumberText = GameObject.Find("ConditonNumberText").GetComponent<TextMeshProUGUI>();
        originNumberText = GameObject.Find("OriginNumberText").GetComponent<TextMeshProUGUI>();
        nextOriginNumberText = GameObject.Find("NextOriginNumberText").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 画面上部のUIのテキストを変更する
    /// </summary>
    /// <param name="kindOfUI">どのUIを変更するか(Condition, Origin, NextOrigin)</param>
    /// <param name="string_number">表示するテキスト</param>
    public void ChangeDisplayText(KindOfUI kindOfUI,string string_number)
    {
        switch (kindOfUI)
        {
            case KindOfUI.Condition:
                conditionNumberText.text = string_number; 
                break;
            case KindOfUI.Origin:
                originNumberText.text = string_number; 
                break;
            case KindOfUI.NextOrigin:
                nextOriginNumberText.text = string_number;
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
