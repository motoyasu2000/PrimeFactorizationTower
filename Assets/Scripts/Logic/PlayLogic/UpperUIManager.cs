using System.Collections;
using TMPro;
using UnityEngine;

//条件を表示するテキストを管理するためのクラス
public class UpperUIManager : MonoBehaviour
{
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
    private void Update()
    {
    }


    public void RemoveUpCompositeNumber()
    {
        originNumberText.text = "";
    }

    public void DisplayNumber(KindOfUI kindOfUI,string str)
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

    public IEnumerator PrintConditionNumber(KindOfUI kindOfUI, string str, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DisplayNumber(kindOfUI, str);
        yield return null;
    }
}
