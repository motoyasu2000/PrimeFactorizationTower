using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class BlockInfo : MonoBehaviour
{
    protected int myNumber; //自分の持つ数字。合成数とかの計算はこれを利用する
    protected List<GameObject> neighborEdge = new List<GameObject>(); //隣接するゲームオブジェクトを格納

    protected GameObject selfPrefab; //自分自身のプレファブを格納する変数(継承先クラスから見た自分自身)
    public GameObject SelfPrefab => selfPrefab;
    protected TextMeshPro primeNumberText;
    bool isGround = false;
    protected Rigidbody2D rb2D;
    protected Collider2D myCollider;
    
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        SetMyNumber();
        SetSelfPrefab();
        SetText();
        myCollider = GetComponent<Collider2D>();
    }

    public abstract void SetSelfPrefab(); //自分自身のプレファブが何であるかは継承先のスクリプトで決定すべき

    //クリックするとkinematicからdynamicに変化するようにする。
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
    }
    public void SetText()
    {
        primeNumberText.text = myNumber.ToString();
    }

    //自分自身の番号を設定するクラス。継承先に記述。
    public abstract void SetMyNumber();

    public int GetNumber()
    {
        return myNumber;
    }

    public bool CheckIsGround()
    {
        return isGround;
    }

    public void EnableCollider()
    {
        myCollider.enabled = true;
    }

    public void RemoveNeighjborBlock(GameObject block)
    {
        neighborEdge.Remove(block);
    }

    public void AddNeighjborBlock(GameObject block)
    {
        neighborEdge.Add(block);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PrimeNumberBlock")){
            isGround = true;
        }
    }


}
