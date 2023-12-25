using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;

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

    //�������R�[�h
    private void Awake()
    {
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        SetMyNumber();
        SetText();
        SetShader();
    }
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        myCollider = GetComponent<Collider2D>();
        netWork = GameObject.Find("NetWork").GetComponent<NetWork>();
    }

    //�N���b�N�����kinematic����dynamic�ɕω�����悤�ɂ���B
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.drag = 2;
    }

    //�\������鐔�l�̐ݒ�
    public virtual void SetText()
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

    public void RemoveEdges()
    {
        neighborEdge = null;
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
        neighborEdge.RemoveAll(item => item!=null && !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetNumber()));
    }

    private bool IsUpOrRight(GameObject myself, GameObject other)
    {
        if(myself.transform.position == other.transform.position)
        {
            Debug.LogError("�Փ˂�����̃Q�[���I�u�W�F�N�g�͓������W�ɂ���܂��B");
        }

        //��������gameobject���㑤�ɂ���Ȃ�true��Ԃ��B���������ł���΁A�E���̏ꍇ��true��Ԃ��B
        if(myself.transform.position.y > other.transform.position.y)
        {
            return true;
        }
        else if(myself.transform.position.y == other.transform.position.y)
        {
            if(myself.transform.position.x > other.transform.position.x)
            {
                return true;
            }
            else { return false; }
        }
        else
        {
            return false;
        }
    }

    private void SetShader()
    {
        Color myColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", myColor);
        GetComponent<SpriteRenderer>().material.SetColor("_Color", myColor);
        Debug.Log($"{GetComponent<SpriteRenderer>().material.GetColor("_GlowColor")}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PrimeNumberBlock")){
            isGround = true;
        }
        //������̃u���b�N(�m�[�h)���ڐG�����Ȃ�A���̓�̃m�[�h�ԂɃG�b�W��ݒ�A�����ăT�u�O���t�̒��o�A�����ĒT��
        if (collision.gameObject.CompareTag("PrimeNumberBlock") && collision.gameObject.GetComponent<BlockInfo>() != null && IsUpOrRight(gameObject, collision.gameObject))
        {
            netWork.AttachNode(gameObject, collision.gameObject);
            netWork.AddStartExpandNetworks(new HashSet<GameObject> {gameObject, collision.gameObject}); //�����������͎����Ŏw�肵�Ă�̂��C���������̂Ō�Ŏ����I
            //Debug.Log($"myself:{gameObject.name} ------ other:{collision.gameObject.name}");
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
