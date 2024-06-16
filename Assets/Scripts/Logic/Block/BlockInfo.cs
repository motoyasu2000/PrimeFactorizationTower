using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 素数ブロックに関わる情報や、操作を行うクラス。
/// 各ブロックにアタッチされている。
/// </summary>
public class BlockInfo : MonoBehaviour
{
    //ブロックの情報
    int ID = -1;
    int myPrimeNumber; //自分の持つ数字。合成数とかの計算はこれを利用する
    bool onGround = false;
    bool onBlock = false;
    bool IsGround => onGround || onBlock;
    TextMeshPro primeNumberText;
    Rigidbody2D rb2D;
    Collider2D myCollider;
    
    //ネットワーク情報
    List<GameObject> neighborEdge = new List<GameObject>(); //隣接するゲームオブジェクトを格納
    BlocksGraphManager blocksGraphManager;

    private void Awake()
    {
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        rb2D = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        if(SceneManager.GetActiveScene().name =="PlayScene") blocksGraphManager = GameObject.Find("BlocksGraphManager").GetComponent<BlocksGraphManager>();
        SetText();
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// kinematicからdynamicに変化するようにする。drag(抵抗)を2に
    /// ブロック落下時に呼ばれる
    /// </summary>
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
        return IsGround;
    }

    public List<GameObject> GetNeighborEdge()
    {
        return neighborEdge;
    }

    public void RemoveEdges()
    {
        neighborEdge = null;
    }

    public IEnumerator EnableCollider()
    {
        yield return new WaitForEndOfFrame();
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
            //Debug.Log("すでに存在するエッジを追加しようとしています。");
        }
        else 
        {
            neighborEdge.Add(block);
        }
    }

    /// <summary>
    /// 他のブロックと衝突した際、自分のブロックが相手のブロックの上にあるならtrueを返す関数。
    /// 2つのブロックが衝突した際、エッジの情報をネットワークに追加するが、それが2回呼ばれないように、trueになったゲームオブジェクトのみがネットワークにノードを追加する処理を呼び出すようにする。
    /// <param name="other">衝突した相手のGameObject</param>
    /// <returns>上か否か</returns>
    private bool IsUpOrRight(GameObject other)
    {
        GameObject myself = gameObject;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        //地面との設置判定
        if(collision.gameObject.CompareTag("Ground")){
           onGround = true;
        }
        if (collision.gameObject.CompareTag("PrimeNumberBlock")){
            onBlock = true;
        }

        //もし二つのブロック(ノード)が接触したなら、その二つのノード間にエッジを設定、サブグラフの探索
        if ((collision.gameObject.CompareTag("PrimeNumberBlock")) && (collision.gameObject.GetComponent<BlockInfo>() != null) && (IsUpOrRight(collision.gameObject)))
        {
            BlocksGraphData.AttachNode(gameObject, collision.gameObject);
            blocksGraphManager.AddStartExpandNetworks(new HashSet<GameObject> {gameObject, collision.gameObject});
            //Debug.Log($"myself:{gameObject.name} ------ other:{collision.gameObject.name}");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //地面との設置判定
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
        if (collision.gameObject.CompareTag("PrimeNumberBlock")){
            onBlock = false;
        }

        //もし二つのブロック(ノード)が離れたなら、その二つのノード間のエッジを消去
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            BlocksGraphData.DetachNode(gameObject, collision.gameObject);
            //Debug.Log($"DetachNode: {gameObject.name} ------ {collision.gameObject.name}");
        }
    }
}
