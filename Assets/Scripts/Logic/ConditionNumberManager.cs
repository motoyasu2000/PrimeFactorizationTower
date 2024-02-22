using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConditionNumberManager : MonoBehaviour
{
    TextMeshProUGUI conditionText;
    private void Start()
    {
        conditionText = GetComponent<TextMeshProUGUI>();
    }
    public void PrintConditionNumber(string str)
    {
        conditionText.gameObject.SetActive(true);
        conditionText.text = str;
    }

    public IEnumerator PrintConditionNumber(string str, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        conditionText.gameObject.SetActive(true);
        conditionText.text = str;
        yield return null;
    }
}
