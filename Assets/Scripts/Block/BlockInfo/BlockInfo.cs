using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;

public abstract class BlockInfo : MonoBehaviour
{
    protected int ID = -1;
    protected int myNumber; //自分の持つ数字。合成数とかの計算はこれを利用する
    [SerializeField]protected List<GameObject> neighborEdge = new List<GameObject>(); //隣接するゲームオブジェクトを格納

    protected GameObject selfPrefab; //自分自身のプレファブを格納する変数(継承先クラスから見た自分自身)
    public GameObject SelfPrefab => selfPrefab;
    protected TextMeshPro primeNumberText;
    [SerializeField]bool isGround = false;
    protected Rigidbody2D rb2D;
    protected Collider2D myCollider;

    NetWork netWork;

    //初期化コード
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

    //クリックするとkinematicからdynamicに変化するようにする。
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.drag = 2;
    }

    //表示される数値の設定
    public virtual void SetText()
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
        neighborEdge.RemoveAll(item => item!=null && !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetNumber()));
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
        Debug.Log($"{GetComponent<SpriteRenderer>().material.GetColor("_GlowColor")}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PrimeNumberBlock")){
            isGround = true;
        }
        //もし二つのブロック(ノード)が接触したなら、その二つのノード間にエッジを設定、そしてサブグラフの抽出、そして探索
        if (collision.gameObject.CompareTag("PrimeNumberBlock") && collision.gameObject.GetComponent<BlockInfo>() != null && IsUpOrRight(gameObject, collision.gameObject))
        {
            netWork.AttachNode(gameObject, collision.gameObject);
            netWork.AddStartExpandNetworks(new HashSet<GameObject> {gameObject, collision.gameObject}); //※※こっちは辞書で指定してるのが気持ち悪いので後で治す！
            //Debug.Log($"myself:{gameObject.name} ------ other:{collision.gameObject.name}");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //もし二つのブロック(ノード)が離れたなら、その二つのノード間にエッジを消去
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
