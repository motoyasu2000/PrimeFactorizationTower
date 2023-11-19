using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public abstract void SetSelfPrefab(); //自分自身のプレファブが何であるかは継承先のスクリプトで決定すべき

    //クリックするとkinematicからdynamicに変化するようにする。
    public void ChangeDynamic()
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
    }

    //表示される数値の設定
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
        neighborEdge.RemoveAll(item => !subNetPattern.Contains(item.GetComponent<BlockInfo>().GetNumber()));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PrimeNumberBlock")){
            isGround = true;
        }
        //もし二つのブロック(ノード)が接触したなら、その二つのノード間にエッジを設定、そしてサブグラフの抽出、そして探索
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            netWork.AttachNode(gameObject, collision.gameObject);
            netWork.CreateSubNetwork(new HashSet<int> { 2, 3, 5 , 7});　//※※こっちは集合で指定してるけど
            netWork.SearchMatchingPattern(new Dictionary<int, int>() { { 2,1},{ 3,1},{ 5,1}, {7,1 } },new HashSet<GameObject> {gameObject, collision.gameObject}); //※※こっちは辞書で指定してるのが気持ち悪いので後で治す！
            //Debug.Log($"AttachNode: {gameObject.name} ------ {collision.gameObject.name}");a
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
