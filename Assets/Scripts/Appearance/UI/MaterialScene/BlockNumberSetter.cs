using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockNumberSetter : MonoBehaviour
{
    TextMeshProUGUI blockNumText;
    void Awake()
    {
        blockNumText = GetComponent<TextMeshProUGUI>();
    }

    public void SetBlockNumber(int blockNum)
    {
        blockNumText.text = $"{blockNum}";
    }
}
