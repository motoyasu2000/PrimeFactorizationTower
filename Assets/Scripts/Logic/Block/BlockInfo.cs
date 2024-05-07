using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

//素数ブロックに関わる情報や、操作を行うクラス。素数ブロックにアタッチされている。
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
    BlocksGraph blocksGraph;

    private void Awake()
    {
        primeNumberText = transform.Find("PrimeNumberText").GetComponent<TextMeshPro>();
        rb2D = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        if(SceneManager.GetActiveScene().name =="PlayScene") blocksGraph = GameObject.Find("BlocksGraph").GetComponent<BlocksGraph>();
        SetText();
        //SetShader();
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

    //シェーダーのMainColorをこのスプライトの色に変更する
    public void SetShader()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //if(もし初期設定であれば)
        renderer.material.SetColor("_Color", renderer.color);
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

    //サブグラフに使うメソッド、patternにマッチしないエッジを消去する。
    public void DeleteMissNeighberBlock(HashSet<int> subNetPattern)
    {
        neighborEdge.RemoveAll(item => item!=null && !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetPrimeNumber()));
    }

    //他のブロックと衝突した際、自分のブロックが相手のブロックの上にあるならtrueを返す関数。
    //2つのブロックが衝突した際、エッジの情報をネットワークに追加するが、それが2回呼ばれないように、trueになったゲームオブジェクトのみがネットワークにノードを追加する処理を呼び出すようにする。
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
        if ((collision.gameObject.CompareTag("PrimeNumberBlock")) && (collision.gameObject.GetComponent<BlockInfo>() != null) && (IsUpOrRight(gameObject, collision.gameObject)))
        {
            blocksGraph.AttachNode(gameObject, collision.gameObject);
            blocksGraph.AddStartExpandNetworks(new HashSet<GameObject> {gameObject, collision.gameObject});
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
            blocksGraph.DetachNode(gameObject, collision.gameObject);
            //Debug.Log($"DetachNode: {gameObject.name} ------ {collision.gameObject.name}");
        }
    }
}
