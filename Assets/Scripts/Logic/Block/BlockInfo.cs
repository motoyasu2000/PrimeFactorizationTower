using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockInfo : MonoBehaviour
{
    //�u���b�N�̊�{���
    int ID = -1;
    int myPrimeNumber; //�����̎������B�������Ƃ��̌v�Z�͂���𗘗p����
    TextMeshPro primeNumberText;
    Rigidbody2D rb2D;
    Collider2D myCollider;

    //�Q�[�����œ��I�ɕω�������
    bool isGround = false;

    //�l�b�g���[�N���
    List<GameObject> neighborEdge = new List<GameObject>(); //�אڂ���Q�[���I�u�W�F�N�g���i�[
    Network network;

    //�������R�[�h
    private void Awake()
    {
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        SetText();
        SetShader();
    }
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        myCollider = GetComponent<Collider2D>();
        network = GameObject.Find("Network").GetComponent<Network>();
    }

    //�N���b�N�����kinematic����dynamic�ɕω�����悤�ɂ���B
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.drag = 2;
    }

    //�\������鐔�l�̐ݒ�
    public void SetText()
    {
        primeNumberText.text = myPrimeNumber.ToString();
    }

    //�������g�̔ԍ���ݒ肷��N���X�B�u���b�N�𐶐�����{�^���ɂ���Ďw�肳���B
    public void SetPrimeNumber(int newMyPrimeNumber)
    {
        myPrimeNumber = newMyPrimeNumber;
    }

    public int GetPrimeNumber()
    {
        return myPrimeNumber;
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
        neighborEdge.RemoveAll(item => item!=null && !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetPrimeNumber()));
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
        //Debug.Log($"{GetComponent<SpriteRenderer>().material.GetColor("_GlowColor")}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PrimeNumberBlock")){
            isGround = true;
        }
        //������̃u���b�N(�m�[�h)���ڐG�����Ȃ�A���̓�̃m�[�h�ԂɃG�b�W��ݒ�A�T�u�O���t�̒T��
        if (collision.gameObject.CompareTag("PrimeNumberBlock") && collision.gameObject.GetComponent<BlockInfo>() != null && IsUpOrRight(gameObject, collision.gameObject))
        {
            network.AttachNode(gameObject, collision.gameObject);
            network.AddStartExpandNetworks(new HashSet<GameObject> {gameObject, collision.gameObject});
            //Debug.Log($"myself:{gameObject.name} ------ other:{collision.gameObject.name}");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //������̃u���b�N(�m�[�h)�����ꂽ�Ȃ�A���̓�̃m�[�h�Ԃ̃G�b�W������
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            network.DetachNode(gameObject, collision.gameObject);
            //Debug.Log($"DetachNode: {gameObject.name} ------ {collision.gameObject.name}");
        }
    }

}
