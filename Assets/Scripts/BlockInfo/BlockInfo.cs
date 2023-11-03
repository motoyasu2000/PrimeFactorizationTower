using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class BlockInfo : MonoBehaviour
{
    protected int myNumber; //�����̎������B�������Ƃ��̌v�Z�͂���𗘗p����
    protected List<GameObject> neighborEdge = new List<GameObject>(); //�אڂ���Q�[���I�u�W�F�N�g���i�[

    protected GameObject selfPrefab; //�������g�̃v���t�@�u���i�[����ϐ�(�p����N���X���猩���������g)
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

    public abstract void SetSelfPrefab(); //�������g�̃v���t�@�u�����ł��邩�͌p����̃X�N���v�g�Ō��肷�ׂ�

    //�N���b�N�����kinematic����dynamic�ɕω�����悤�ɂ���B
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
    }
    public void SetText()
    {
        primeNumberText.text = myNumber.ToString();
    }

    //�������g�̔ԍ���ݒ肷��N���X�B�p����ɋL�q�B
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
