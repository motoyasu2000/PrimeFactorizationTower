using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NetWork : MonoBehaviour
{
    static int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23 }; //素数配列
    [SerializeField] List<GameObject> allNodes = new List<GameObject>(); //全ノードのリストト
    [SerializeField] Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>();
    [SerializeField] GameModeManager gameModeManager;
    [SerializeField] SoundManager soundManager;
    [SerializeField] MainTextManager mainTextManager;
    GameObject freezeEffect;

    ConditionGenerator conditionGenerator;
    public ConditionGenerator _conditionGenerator => conditionGenerator;
    [SerializeField]Dictionary<int, int> freezeCondition;
    HashSet<int> freezeSet = new HashSet<int>();
    public HashSet<int> FreezeSet => freezeSet;

    public Dictionary<int, int> FreezeCondition => freezeCondition;
    Queue<ExpandNetwork> startExpandNetworks = new Queue<ExpandNetwork>(); //ネットワークの拡張を開始する最初のサブネットワークをリストとして保存しておく。非同期の処理を一つずつ実行するため、タプルの２つ目の要素は条件の辞書
    bool wasCriteriaMet = false;
    bool isRespawnKillChecking = false; //条件を変更した瞬間にすでに条件を達成していることをリスポーンキルと呼ぶことにする。
    int CheckNumParFrame = 3; //位置フレーム当たりにキューから取り出す数


    private void Start()
    {
        gameModeManager = GameObject.Find("GameModeManager").GetComponent<GameModeManager>();
        soundManager = SoundManager.SoundManagerInstance;
        conditionGenerator = transform.Find("ConditionGenerator").GetComponent<ConditionGenerator>();

        freezeEffect = (GameObject)Resources.Load("FreezeEffect");

        freezeCondition = _conditionGenerator.GenerateCondition();
        foreach(var key in freezeCondition.Keys)
        {
            freezeSet.Add(key);
        }
        foreach (var value in primeNumbers)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

    private void Update()
    {
        //checkNumParFrame分だけキューに入っていて条件を満たすものがないかネットワーク内でチェックを行う。
        for (int i = 0; i < CheckNumParFrame; i++)
        {
            if (startExpandNetworks.Count == 0)
            {
                isRespawnKillChecking = false;
                return;
            }
            var item = startExpandNetworks.Dequeue();
            foreach (var block in item.myNetwork)
            {
                if (block.GetComponent<BlockInfo>().enabled == false) return; //もうネットワークから切り離されて処理を行う予定でないブロックもこの中に含まれてしまっており、そのようなものは条件を満たさない。
            }
            wasCriteriaMet = false;
            ExpandAndSearch(item);
        }

    }

    //ノードの追加、allNodesとnodesDictの更新を行う。
    public void AddNode(GameObject node)
    {
        allNodes.Add(node);
        BlockInfo info = node.GetComponent<BlockInfo>();
        if (System.Array.Exists(primeNumbers, element => element == info.GetNumber()))
        {
            if (!nodesDict.ContainsKey(info.GetNumber()))
            {
                nodesDict.Add(info.GetNumber(), new List<GameObject>());
            }
            nodesDict[info.GetNumber()].Add(node);
            //foreach(var value in nodesDict.Values)
            //{
            //    Debug.Log(string.Join(",", value));
            //}
        }
        else
        {
            Debug.LogError("素数定義外のノードが定義されようとしています。");
        }
    }

    //エッジの更新を行う(削除)
    public void DetachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.RemoveNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.RemoveNeighborBlock(node1);
    }

    //エッジの更新を行う(追加)
    public void AttachNode(GameObject node1, GameObject node2)
    {
        BlockInfo info1 = node1.GetComponent<BlockInfo>();
        info1.AddNeighborBlock(node2);
        BlockInfo info2 = node2.GetComponent<BlockInfo>();
        info2.AddNeighborBlock(node1);
    }

    //ネットワークから特定のノードをDestroyするメソッド
    private void SafeDestroyNode(GameObject originNode)
    {
        SafeCutNode(originNode);
        Destroy(originNode);
    }

    //ネットワークから特定のサブネットワークをDestroyするメソッド
    private void SafeDestroyNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeDestroyNode(node);
        }
    }

    //ネットワークから特定のノードを切り離すメソッド(Destroyはしない)
    private void SafeCutNode(GameObject originNode)
    {
        //切り離し元のノードと隣接するエッジを一時的な変数に格納(コレクションがイテレーション中に変更してはならないため)
        List<GameObject> tmpNeighborNode = new List<GameObject>();
        foreach (var neighborNode in originNode.GetComponent<BlockInfo>().GetNeighborEdge())
        {
            tmpNeighborNode.Add(neighborNode);
        }
        //切り離し元のノードと隣接するエッジを実際に削除する
        foreach (var neighborNode in tmpNeighborNode)
        {
            DetachNode(neighborNode, originNode);
        }
        //ネットワークからそのノードを削除
        allNodes.Remove(originNode);
        //ネットワーク辞書からもそのノードを削除
        nodesDict[originNode.GetComponent<BlockInfo>().GetNumber()].Remove(originNode);
        //ブロックの情報も失わせる。
        originNode.GetComponent<BlockInfo>().enabled = false;
    }

    //ネットワークから特定のサブネットワークを切り離すメソッド
    private void SafeCutNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeCutNode(node);
        }
    }

    //サブネットワークの色を変更させるメソッド
    private void ChangeColorNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            node.GetComponent<SpriteRenderer>().color = Color.white;
            node.GetComponent<SpriteRenderer>().material.color = Color.white;
        }
    }

    //サブネットワークを物理的に結合し色を変更し、仮想的なネットワークから切り離すメソッド
    private void FreezeNodes(List<GameObject> nodes)
    {
        for(int i=1; i<nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
        SafeCutNodes(nodes);
        ChangeColorNodes(nodes);
    }

    IEnumerator StopRigidbodys(List<GameObject> nodes, float second)
    {
        yield return new WaitForSeconds(second);
        foreach (var node in nodes)
        {
            Rigidbody2D rb2d = node.GetComponent<Rigidbody2D>();
            rb2d.velocity = Vector3.zero;
            rb2d.angularVelocity = 0;
            rb2d.isKinematic = true;
            node.GetComponent<SpriteRenderer>().color = new Color(23f / 255f, 1f, 1f);
        }
        yield break;
    }

    //条件を満たしたときの処理
    private void CompleteConditions(List<GameObject> nodes)
    {
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                Vector3 nodesCenter = CaluculateCenter(nodes);
                FreezeNodes(nodes);
                mainTextManager.TmpPrintMainText("Criteria Met");
                soundManager.PlayAudio(soundManager.VOICE_CRITERIAMAT);

                StartCoroutine(StopRigidbodys(nodes, 1.5f));
                StartCoroutine(soundManager.PlayAudio(soundManager.VOICE_FREEZE,1.5f));
                StartCoroutine(soundManager.PlayAudio(soundManager.SE_FREEZE, 1.5f));
                StartCoroutine(mainTextManager.TmpPrintMainText("Freeze",1.5f));
                StartCoroutine(InstantiateEffect(freezeEffect, nodesCenter, 1.5f));
                freezeCondition = _conditionGenerator.GenerateCondition();
                break;
        }
    }

    private Vector3 CaluculateCenter(List<GameObject> gameObjects)
    {
        Vector3 center = Vector3.zero;
        foreach (var gameObject in gameObjects)
        {
            center += gameObject.transform.position;
        }
        center /= gameObjects.Count;
        return center;
    }

    private IEnumerator InstantiateEffect(GameObject effect, Vector3 position, float second)
    {
        yield return new WaitForSeconds (second);
        Instantiate(effect, position, Quaternion.identity);
    }

    //ネットワーク全体に条件にマッチするものがないかを探索するためのメソッド
    void CheckConditionAllNetwork()
    {
        int minNodeNum = int.MaxValue; //条件に存在するノードの数字の内、ネットワーク内に最小個数である数字の個数を格納する変数
        int minNode = -1; //最小個数の素数
        //条件に存在する素数を全探索し、最小個数のものを探す
        foreach(int valueInFreezeCondition in freezeCondition.Keys)
        {
            //もし、現在の最小個数よりも、今見ている条件の個数のほうがすくなかったら、最小個数の更新と最小個数の素数が何であるのかの更新をする。
            if(minNodeNum > nodesDict[valueInFreezeCondition].Count)
            {
                minNodeNum = nodesDict[valueInFreezeCondition].Count;
                minNode = valueInFreezeCondition;
            }
        }

        //最小個数の素数はすでに求まっているので、それに対してfor分を回してstartExpandNetworks
        foreach(var node in nodesDict[minNode])
        {
            startExpandNetworks.Enqueue(new ExpandNetwork(null, node, freezeCondition));
        }
    }

    //パターンマッチングのロジック
    bool ContainsAllRequiredNodes(List<GameObject> myNetwork, Dictionary<int, int> requiredNodesDict)
    {
        Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);
        //Debug.Log(string.Join(", ", requiredCounts));

        //現在のネットワーク内のノードの出現回数をカウント
        foreach (var node in myNetwork)
        {
            int nodeValue = node.GetComponent<BlockInfo>().GetNumber();
            if (requiredCounts.ContainsKey(nodeValue))
            {
                requiredCounts[nodeValue]--;
                if (requiredCounts[nodeValue] == 0)
                    requiredCounts.Remove(nodeValue);
            }
            //もしサブネットワーク内に関係のない数値があればfalseを返す
            else
            {
                return false;
            }
        }

        return requiredCounts.Count == 0; //必要なノードがすべて含まれていればtrue
    }
    public void AddStartExpandNetworks(HashSet<GameObject> neiborSet)
    {
        ExpandNetwork currentNetwork = null;
        foreach (var startNode in neiborSet)
        {
            if(currentNetwork == null)
            {
                currentNetwork = new ExpandNetwork(null, startNode, freezeCondition);
            }
            else
            {
                currentNetwork = new ExpandNetwork(currentNetwork, startNode, freezeCondition);
            }
        }
        //Debug.Log(string.Join(", ",currentNetwork.myNetwork));
        //ネットワークを拡張していく処理
        startExpandNetworks.Enqueue(currentNetwork);
    }

    //ネットワークを拡張しながらサブグラフを探索する再帰的メソッド
    private void ExpandAndSearch(ExpandNetwork currentNetwork)
    {
        //if (wasCriteriaMet) return; //もし現在のフレームで条件を達成済みならreturnする 1フレームあたりに一つの衝突パターンからの検知しか行わないので1フレーム内で発見済みならそれ以上探さなくてよい 単一の衝突から探索する条件を満たすサブネットワークが複数存在する場合があるが、見つけるのは一つでいいということ。
        //Debug.Log(string.Join(", ", currentNetwork.myNetwork));
        //Debug.Log(string.Join(", ", currentNetwork));

        if (ContainsAllRequiredNodes(currentNetwork.myNetwork, freezeCondition))
        {
            if (isRespawnKillChecking) //リスキルチェック中に条件を達成→リスキルされることが確定→条件を再設定して再び調査。
            {
                freezeCondition = _conditionGenerator.GenerateCondition();
                isRespawnKillChecking = true;
                CheckConditionAllNetwork();
                //Debug.Log("リスキル");
                return;
            }
            //wasCriteriaMet = true;
            Debug.Log(string.Join(", ", currentNetwork.myNetwork));

            CompleteConditions(currentNetwork.myNetwork);
            startExpandNetworks = new Queue<ExpandNetwork>(); //探索が完了したらもうネットワーク内に条件を満たすものが存在しないと考えられるので、キューをリセットしておく。
            isRespawnKillChecking = true;
            CheckConditionAllNetwork();

            return;
        }

        //各ノードの隣接ノードを探索
        foreach (var node in currentNetwork.myNetwork)
        {
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            //現在のネットワークとclosedListに含まれていないノードのみを選択
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.closedList.Contains(n) && !currentNetwork.myNetwork.Contains(n)).ToList();

            if (adjacentNodes.Count == 0)
            {
                continue; //隣接する新しいノードがなければスキップ
            }

            foreach (var adjacentNode in adjacentNodes)
            {
                if (adjacentNode.gameObject.GetComponent<BlockInfo>().enabled == false) continue; //ネットワークから切り離されてblockinfoがなくなったブロックに対してもadjacentNodesに含まれる可能性があるのではじいておく。
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, freezeCondition);
                if (newNetwork.BackFlag)
                {
                    newNetwork = newNetwork.Beforenetwork;
                }
                
                ExpandAndSearch(newNetwork);
            }
        }
    }
}