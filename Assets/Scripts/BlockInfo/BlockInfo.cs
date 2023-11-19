using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class BlockInfo : MonoBehaviour
{
    protected int ID = -1;
    protected int myNumber; //�����̎������B�������Ƃ��̌v�Z�͂���𗘗p����
    [SerializeField]protected List<GameObject> neighborEdge = new List<GameObject>(); //�אڂ���Q�[���I�u�W�F�N�g���i�[

    protected GameObject selfPrefab; //�������g�̃v���t�@�u���i�[����ϐ�(�p����N���X���猩���������g)
    public GameObject SelfPrefab => selfPrefab;
    protected TextMeshPro primeNumberText;
    [SerializeField]bool isGround = false;
    protected Rigidbody2D rb2D;
    protected Collider2D myCollider;

    NetWork netWork;
    
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        SetMyNumber();
        SetSelfPrefab();
        SetText();
        myCollider = GetComponent<Collider2D>();
        netWork = GameObject.Find("NetWork").GetComponent<NetWork>();
    }

    public abstract void SetSelfPrefab(); //�������g�̃v���t�@�u�����ł��邩�͌p����̃X�N���v�g�Ō��肷�ׂ�

    //�N���b�N�����kinematic����dynamic�ɕω�����悤�ɂ���B
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
    }

    //�\������鐔�l�̐ݒ�
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

    public List<GameObject> GetNeighborEdge()
    {
        return neighborEdge;
    }

    public void EnableCollider()
    {
        myCollider.enabled = true;
    }

    public void SetID(int newID)
    {
        ID = newID;
    }

    public void RemoveNeighborBlock(GameObject block)
    {
        if (!neighborEdge.Contains(block))
        {
            //Debug.Log("���݂��Ȃ��G�b�W���������悤�Ƃ��Ă��܂��B");
        }
        else
        {
            neighborEdge.Remove(block);
        }
    
    }

    public void AddNeighborBlock(GameObject block)
    {
        if (neighborEdge.Contains(block))
        {
            //Debug.Log("���݂���G�b�W��ǉ����悤�Ƃ��Ă��܂��B");
        }
        else 
        {
            neighborEdge.Add(block);
        }
    }

    //�T�u�O���t�Ɏg�����\�b�h�Apattern�Ƀ}�b�`���Ȃ��G�b�W����������B
    public void DeleteMissNeighberBlock(HashSet<int> subNetPattern)
    {
        neighborEdge.RemoveAll(item => !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetNumber()));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PrimeNumberBlock")){
            isGround = true;
        }
        //������̃u���b�N(�m�[�h)���ڐG�����Ȃ�A���̓�̃m�[�h�ԂɃG�b�W��ݒ�A�����ăT�u�O���t�̒��o�A�����ĒT��
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            netWork.AttachNode(gameObject, collision.gameObject);
            netWork.CreateSubNetwork(new HashSet<int> { 2, 3, 5 , 7});�@//�����������͏W���Ŏw�肵�Ă邯��
            netWork.SearchMatchingPattern(new Dictionary<int, int>() { { 2,1},{ 3,1},{ 5,1}, {7,1 } },new HashSet<GameObject> {gameObject, collision.gameObject}); //�����������͎����Ŏw�肵�Ă�̂��C���������̂Ō�Ŏ����I
            //Debug.Log($"AttachNode: {gameObject.name} ------ {collision.gameObject.name}");a
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //������̃u���b�N(�m�[�h)�����ꂽ�Ȃ�A���̓�̃m�[�h�ԂɃG�b�W������
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            netWork.DetachNode(gameObject, collision.gameObject);
            //Debug.Log($"DetachNode: {gameObject.name} ------ {collision.gameObject.name}");
        }
    }

    private void Update()
    {
    }

}
