using UnityEngine;
using TMPro;

//ブロックを生成するクラス。ブロックを生成するボタンにアタッチされている。
public class BlockGenerator : MonoBehaviour
{
    static int IDCounter = 0;
    int primeNumber;
    GameObject primeNumberGeneratingPoint;
    GameObject blockField;
    GameObject beforeField;
    SingleGenerateManager singleGenerateManager;
    TextMeshProUGUI buttonText;
    GameManager gameManager;

    private void Start()
    {
        primeNumberGeneratingPoint = GameObject.Find("PrimeNumberGeneratingPoint");
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        buttonText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        buttonText.text = primeNumber.ToString();
        blockField = GameObject.Find("BlockField");
        beforeField = blockField.transform.Find("BeforeField").gameObject;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    //ブロックを生成する関数の引数を制御する関数。
    public void GenerateBlock()
    {
        if (gameManager.GetCompleteNumberFlag()) return; //素数が揃えられている状態であればリターン
        HundleGenerateBlock(primeNumber);
    }

    //引数で与えられた数値に合わせてブロックを生成する関数。そのブロックの情報(IDや素数など)も追加する。
    void HundleGenerateBlock(int primeNumber)
    {
        //ゲームオブジェクトの生成とその情報をもつインスタンスの取得
        GameObject generateObject = Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, GetPrimeNumberBlock(primeNumber).transform.rotation, beforeField.transform);
        BlockInfo blockInfo = generateObject.GetComponent<BlockInfo>();

        //ブロックの持つ素数の設定とテキストの切り替え
        blockInfo.SetPrimeNumber(primeNumber);
        blockInfo.SetText();

        //IDの設定
        blockInfo.SetID(IDCounter);
        generateObject.name = $"Block{primeNumber}_{IDCounter}";
        IDCounter++;

        singleGenerateManager.SetSingleGameObject(generateObject);//生成したゲームオブジェクトの情報を、生成できるゲームオブジェクトが常に単一であるように管理するメソッドに入れる。
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        //Debug.Log("Block" + primeNumber.ToString());
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }

    public void SetPrimeNumber(int newPrimeNumber)
    {
        primeNumber = newPrimeNumber;
    }
}
