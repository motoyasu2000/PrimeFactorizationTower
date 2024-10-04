using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// シーン内に存在するテキストを画面の対角線長にあわせてスケーリングするクラス
/// </summary>
public class FontSizeScaler : MonoBehaviour
{
    //UIをデザインしているときもっとも基準としていた画面の対角線
    static readonly float referenceDiagonal = 2789.25f;

    //フォントサイズを加算する値にかける値(大きいほどフォントサイズの値の増減が激しくなる。1だと対角線の大きさに完全に依存する)
    static readonly float fontSizeAddScaler = 0.5f;

    void Start()
    {
        AdjustFontSizes();
    }

    void AdjustFontSizes()
    {
        //現在の画面の対角線長から、どのくらいリサイズすべきかを計算
        float currentDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        float scalingFactor = currentDiagonal / referenceDiagonal;
        float rateOfChange = scalingFactor - 1; //増減率

        //シーン内のすべてのTextコンポーネントを取得し、スケーリング
        TextMeshProUGUI[] tmpTextComponents = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI tmpText in tmpTextComponents)
        {
            tmpText.fontSize += tmpText.fontSize * rateOfChange * fontSizeAddScaler;
        }
    }
}
