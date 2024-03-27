using System.Collections;
using TMPro;
using UnityEngine;

//条件を表示するテキストを管理するためのクラス
public class UIManager : MonoBehaviour
{
    TextMeshProUGUI conditionNumberText; //左上の条件テキスト
    private void Awake()
    {
        conditionNumberText = GameObject.Find("ConditonNumberText").GetComponent<TextMeshProUGUI>();
    }
    public void PrintConditionNumber(string str)
    {
        conditionNumberText.text = str;
    }

    public IEnumerator PrintConditionNumber(string str, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PrintConditionNumber(str);
        yield return null;
    }
}
