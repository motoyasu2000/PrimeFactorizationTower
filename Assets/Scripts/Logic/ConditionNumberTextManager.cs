using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//条件を表示するテキストを管理するためのクラス
public class ConditionNumberTextManager : MonoBehaviour
{
    TextMeshProUGUI conditionNumberText; //左上の条件テキスト
    private void Start()
    {
        conditionNumberText = GetComponent<TextMeshProUGUI>();
    }
    public void PrintConditionNumber(string str)
    {
        conditionNumberText.gameObject.SetActive(true);
        conditionNumberText.text = str;
    }

    public IEnumerator PrintConditionNumber(string str, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PrintConditionNumber(str);
        yield return null;
    }
}
