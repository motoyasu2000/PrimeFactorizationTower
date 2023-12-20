using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenerateBlockCtrl : MonoBehaviour
{
    [SerializeField] PrimeNumberData primeNumberData;
    [SerializeField] GameObject primeNumberGeneratingPoint;
    GameObject blockField;
    GameObject beforeField;
    SingleGenerateManager singleGenerateManager;
    TextMeshProUGUI text;
    protected GameManager gameManager;
    static int IDCounter = 0;

    private void Start()
    {
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = primeNumberData.primeNumber.ToString();
        blockField = GameObject.Find("BlockField");
        beforeField = blockField.transform.Find("BeforeField").gameObject;
        if(GameObject.Find("GameManager") != null) gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        else gameManager = GameObject.Find("_GameManager").GetComponent<GameManager>();
    }
    //ブロックを生成する関数の引数を制御する関数。
    public void GenerateBlock()
    {
        if (gameManager.GetCompleteNumberFlag()) return; //素数が揃えられている状態であればリターン
        HundleGenerateBlock(primeNumberData.primeNumber);
    }

    //引数で与えられた数値に合わせてブロックを生成する関数。
    void HundleGenerateBlock(int primeNumber)
    {
        GameObject generateObject = Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, GetPrimeNumberBlock(primeNumber).transform.rotation, beforeField.transform);
        generateObject.GetComponent<BlockInfo>().SetID(IDCounter);
        generateObject.name = $"Block{primeNumber}_{IDCounter++}";
        singleGenerateManager.SetSingleGameObject(generateObject);
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        //Debug.Log("Block" + primeNumber.ToString());
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }
}
