using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainTextManager : MonoBehaviour
{
    TextMeshProUGUI mainText;
    private void Start()
    {
        mainText = GetComponent<TextMeshProUGUI>();
    }
    public void TmpPrintMainText(string str)
    {
        mainText.gameObject.SetActive(true);
        mainText.text = str;
        StartCoroutine(HiddenMainText());
    }

    public IEnumerator TmpPrintMainText(string str, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        mainText.gameObject.SetActive(true);
        mainText.text = str;
        StartCoroutine(HiddenMainText());
        yield return null;
    }

    IEnumerator HiddenMainText()
    {
        yield return new WaitForSeconds(1.2f);
        mainText.gameObject.SetActive(false);
        yield return null;
    }
}
