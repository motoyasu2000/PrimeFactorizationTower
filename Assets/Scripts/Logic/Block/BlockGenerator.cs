using UnityEngine;
using TMPro;

/// <summary>
/// ブロックを生成するクラス。ブロックを生成するボタンにアタッチされている。
/// </summary>
public class BlockGenerator : MonoBehaviour
{
    static int IDCounter = 0;
    int primeNumber;
    GameObject primeNumberGeneratingPoint;
    GameObject blockField;
    GameObject beforeField;
    AllBlocksManager allBlocksManager;
    SingleGenerateManager singleGenerateManager;
    TextMeshProUGUI buttonText;
    GameManager gameManager;

    private void Start()
    {
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        blockField = GameObject.Find("BlockField");
        beforeField = blockField.transform.Find("BeforeField").gameObject;
        allBlocksManager = GameObject.Find("AllBlocksManager").GetComponent<AllBlocksManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (transform.Find("Text") != null)
        {
            buttonText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            buttonText.text = primeNumber.ToString();
        }
    }
    //ブロックを生成する関数の引数を制御する関数。
    public void GenerateBlock()
    {
        if (gameManager.IsDropBlockNowTurn) return; //既に現在のターンでブロックがドロップされていたらリターン
        HundleGenerateBlock(primeNumber);
    }

    /// <summary>
    /// 引数で与えられた数値に合わせてブロックを生成する関数。
    /// そのブロックの情報(IDや素数など)も追加する。
    /// </summary>
    /// <param name="primeNumber">どの素数を生成するか</param>
    void HundleGenerateBlock(int primeNumber)
    {
        //ゲームオブジェクトの生成とその情報をもつインスタンスの取得
        GameObject generateObject = Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, GetPrimeNumberBlock(primeNumber).transform.rotation, beforeField.transform);
        singleGenerateManager.SetSingleGameObject(generateObject);//生成したゲームオブジェクトの情報を、生成できるゲームオブジェクトが常に単一であるように管理するメソッドに入れる。
        BlockInfo blockInfo = generateObject.GetComponent<BlockInfo>();

        //ブロックの持つ素数の設定とテキストの切り替え
        blockInfo.SetPrimeNumber(primeNumber);
        blockInfo.SetText();

        //IDの設定
        blockInfo.SetID(IDCounter);
        generateObject.name = $"Block{primeNumber}_{IDCounter}";
        IDCounter++;
    }

    public void GenerateBlock_HundleAI(int primeNumber)
    {
        HundleGenerateBlock(primeNumber);
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        return allBlocksManager.BlocksDict[primeNumber];
    }

    public void SetPrimeNumber(int newPrimeNumber)
    {
        primeNumber = newPrimeNumber;
    }
}
