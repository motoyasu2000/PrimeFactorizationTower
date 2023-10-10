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

    private void Start()
    {
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = primeNumberData.primeNumber.ToString();
        blockField = GameObject.Find("BlockField");
        beforeField = blockField.transform.Find("BeforeField").gameObject;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void GenerateBlock()
    {
        if (gameManager.GetCompleteNumberFlag()) return; //ëfêîÇ™ëµÇ¶ÇÁÇÍÇƒÇ¢ÇÈèÛë‘Ç≈Ç†ÇÍÇŒÉäÉ^Å[Éì
        HundleGenerateBlock(primeNumberData.primeNumber);
    }
    void HundleGenerateBlock(int primeNumber)
    {
        GameObject generateObject = Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, GetPrimeNumberBlock(primeNumber).transform.rotation, beforeField.transform);
        singleGenerateManager.SetSingleGameObject(generateObject);
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        Debug.Log("Block" + primeNumber.ToString());
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }
}
