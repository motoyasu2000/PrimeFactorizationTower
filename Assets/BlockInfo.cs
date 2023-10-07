using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class BlockInfo : MonoBehaviour
{
    protected int myNumber; //自分の持つ数字。合成数とかの計算はこれを利用する
    protected GameObject selfPrefab; //自分自身のプレファブを格納する変数(継承先クラスから見た自分自身)
    public GameObject SelfPrefab => selfPrefab;
    protected TextMeshPro primeNumberText;
    
    private void Start()
    {
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        SetMyNumber();
        SetSelfPrefab();
        SetText();
    }

    public abstract void SetSelfPrefab(); //自分自身のプレファブが何であるかは継承先のスクリプトで決定すべき
    public abstract void AddRigidbody2D(); //ブロックごとに重力のかけ方が違うかもしれないので、継承先のクラスで記述
    public void SetText()
    {
        primeNumberText.text = myNumber.ToString();
    }

    //自分自身の番号を設定するクラス。継承先に記述。
    public abstract void SetMyNumber();
}
