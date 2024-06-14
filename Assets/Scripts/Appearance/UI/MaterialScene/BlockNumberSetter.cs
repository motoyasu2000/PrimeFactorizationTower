using TMPro;
using UnityEngine;

/// <summary>
/// MaterialScene内で選択されているブロックの数値を設定するクラス
/// </summary>
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
