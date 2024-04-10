using TMPro;
using UnityEngine;

//今マテリアルの設定を行っているブロックの数字を表示するクラス
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
