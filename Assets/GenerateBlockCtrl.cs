using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBlockCtrl : MonoBehaviour
{
    [SerializeField] PrimeNumberData primeNumberData;
    [SerializeField] GameObject primeNumberGeneratingPoint;
    public void GenerateBlock()
    {
        HundleGenerateBlock(primeNumberData.primeNumber);
    }
    void HundleGenerateBlock(int primeNumber)
    {
        Instantiate(GetPrimeNumberBlock(primeNumber), primeNumberGeneratingPoint.transform.position, Quaternion.identity);
    }
    GameObject GetPrimeNumberBlock(int primeNumber)
    {
        Debug.Log("Block" + primeNumber.ToString());
        return (GameObject)Resources.Load("Block" + primeNumber.ToString());
    }
}
