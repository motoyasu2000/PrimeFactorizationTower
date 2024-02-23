using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockInfo : MonoBehaviour
{
    //ブロックの基本情報
    int ID = -1;
    int myPrimeNumber; //自分の持つ数字。合成数とかの計算はこれを利用する
    TextMeshPro primeNumberText;
    Rigidbody2D rb2D;
    Collider2D myCollider;

    //ゲーム内で動的に変化する情報
    bool isGround = false;

    //ネットワーク情報
    List<GameObject> neighborEdge = new List<GameObject>(); //隣接するゲームオブジェクトを格納
    Network network;

    //初期化コード
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

    //クリックするとkinematicからdynamicに変化するようにする。
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.drag = 2;
    }

    //表示される数値の設定
    public void SetText()
    {
        primeNumberText.text = myPrimeNumber.ToString();
    }

    //自分自身の番号を設定するクラス。ブロックを生成するボタンによって指定される。
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
            //Debug.Log("存在しないエッジを消去しようとしています。");
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
            //Debug.Log("存在するエッジを追加しようとしています。");
        }
        else 
        {
            neighborEdge.Add(block);
        }
    }

    //サブグラフに使うメソッド、patternにマッチしないエッジを消去する。
    public void DeleteMissNeighberBlock(HashSet<int> subNetPattern)
    {
        neighborEdge.RemoveAll(item => item!=null && !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetPrimeNumber()));
    }

    private bool IsUpOrRight(GameObject myself, GameObject other)
    {
        if(myself.transform.position == other.transform.position)
        {
            Debug.LogError("衝突した二つのゲームオブジェクトは同じ座標にあります。");
        }

        //第一引数のgameobjectが上側にあるならtrueを返す。もし同じであれば、右側の場合にtrueを返す。
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
        //もし二つのブロック(ノード)が接触したなら、その二つのノード間にエッジを設定、サブグラフの探索
        if (collision.gameObject.CompareTag("PrimeNumberBlock") && collision.gameObject.GetComponent<BlockInfo>() != null && IsUpOrRight(gameObject, collision.gameObject))
        {
            network.AttachNode(gameObject, collision.gameObject);
            network.AddStartExpandNetworks(new HashSet<GameObject> {gameObject, collision.gameObject});
            //Debug.Log($"myself:{gameObject.name} ------ other:{collision.gameObject.name}");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //もし二つのブロック(ノード)が離れたなら、その二つのノード間のエッジを消去
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            network.DetachNode(gameObject, collision.gameObject);
            //Debug.Log($"DetachNode: {gameObject.name} ------ {collision.gameObject.name}");
        }
    }

}
