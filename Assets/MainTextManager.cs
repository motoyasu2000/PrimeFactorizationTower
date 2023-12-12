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
        mainText.gameObject.SetActive(false);
    }
    public void TmpPrintMainText(string str)
    {
        mainText.gameObject.SetActive(true);
        mainText.text = str;
        StartCoroutine(HiddenMainText());
    }

    IEnumerator HiddenMainText()
    {
        yield return new WaitForSeconds(1.2f);
        mainText.gameObject.SetActive(false);
        yield return null;
    }
}
