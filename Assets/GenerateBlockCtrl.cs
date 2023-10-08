using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBlockCtrl : MonoBehaviour
{
    [SerializeField] PrimeNumberData primeNumberData;
    [SerializeField] GameObject primeNumberGeneratingPoint;
    SingleGenerateManager singleGenerateManager;

    private void Start()
    {
        singleGenerateManager = primeNumberGeneratingPoint.GetComponent<SingleGenerateManager>();
    }
    public void GenerateBlock()
    {
        HundleGenerateBlock(primeNumberData.primeNumber);
    }
    void HundleGenerateBlock(int primeNumber)
    {
        GameObject generateObject = Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, Quaternion.identity);
        singleGenerateManager.SetSingleGameObject(generateObject);
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        Debug.Log("Block" + primeNumber.ToString());
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }
}
