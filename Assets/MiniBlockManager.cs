using Common;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ブロック生成ボタンに表示する、ボタンが生成できるブロックを可視化するためのクラス
/// </summary>
public class MiniBlockManager : MonoBehaviour
{
    static readonly float defaultMiniBlockSize = 200f;

    int generatedPrimeNumber;
    GameObject generatedBlock;
    Image miniBlockImage;
    SpriteRenderer generateBlockRenderer;
    BlockGenerator blockGenerator;

    void Start()
    {
        blockGenerator = transform.parent.GetComponent<BlockGenerator>();
        generatedPrimeNumber = blockGenerator.PrimeNumber;
        generatedBlock = blockGenerator.GetPrimeNumberBlock(generatedPrimeNumber);
        generateBlockRenderer = generatedBlock.GetComponent<SpriteRenderer>();
        miniBlockImage = GetComponent<Image>();
        miniBlockImage.material = new Material(miniBlockImage.material); //元のマテリアルを参照しないように
        miniBlockImage.material.SetTexture("_MainTex", generateBlockRenderer.sprite.texture);
        miniBlockImage.material.SetColor("_OutLineColor", GameInfo.MiniBlockColor);

        ResizeMiniBlock();
    }

    void ResizeMiniBlock()
    {
        Vector3 blockScale = generatedBlock.transform.localScale;
        GetComponent<RectTransform>().sizeDelta = blockScale * defaultMiniBlockSize;
        //超長細いブロックのみ輪郭線の表示が難しいため、ブロックの見た目のまま表示する
        if (generatedPrimeNumber == 11) 
        {
            miniBlockImage.sprite = generateBlockRenderer.sprite;
            miniBlockImage.material = null;
            miniBlockImage.color = GameInfo.MiniBlockColor;
        }
    }

}
