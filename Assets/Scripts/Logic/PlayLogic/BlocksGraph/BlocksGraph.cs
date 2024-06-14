using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UI;
using Common;

//積み上げれたブロックはブロックがノード、隣接関係がエッジとなるグラフ構造をしており。それを管理するためのクラス。
public class BlocksGraph : MonoBehaviour
{
    static readonly float freezeDelayTime = 1.5f;

    //ネットワークの構造や基本機能に使用するもの
    static int[] primeNumberPool; //ゲーム内で扱う全ての素数
    List<GameObject> wholeGraph = new List<GameObject>(); //全ノードのリスト、ネットワーク全体
    Dictionary<int, List<GameObject>> nodesDict = new Dictionary<int, List<GameObject>>();

    //サブグラフの探索
    int CheckNumParFrame = 20; //1フレーム当たりにキューから取り出す数
    Queue<ExpandNetwork> startExpandNetworks = new Queue<ExpandNetwork>(); //ネットワークの拡張を開始する最初のサブネットワークをリストとして保存しておく。非同期の処理を一つずつ実行するため、タプルの２つ目の要素は条件の辞書

    //条件の生成
    bool newConditionChecking = false; //条件を達成してから、新たに条件を生成しおえるまでの間はtrueになる
    ConditionManager conditionGenerator;

    //条件達成時の処理 ※ゲームモードごとに条件達成時の処理が変わる可能性がある。
    GameObject freezeEffect;
    [SerializeField]Material CriteriaMetMaterial;
    GameModeManager gameModeManager;
    SoundManager soundManager;
    EffectTextManager effectTextManager;


    private void Start()
    {
        //初期化処理
        primeNumberPool = GameModeManager.Ins.PrimeNumberPool;
        gameModeManager = GameModeManager.Ins;
        soundManager = SoundManager.Ins;
        conditionGenerator = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        effectTextManager = GameObject.Find("EffectText").GetComponent<EffectTextManager>();
        freezeEffect = (GameObject)Resources.Load("FreezeEffect");
        foreach (var value in primeNumberPool)
        {
            nodesDict.Add(value, new List<GameObject>());
        }
    }

    private void Update()
    {
        //checkNumParFrameの整数値回だけキューに入っていて条件を満たすものがないかネットワーク内でチェックを行う。
        for (int i = 0; i < CheckNumParFrame; i++)
        {
            if (startExpandNetworks.Count == 0)
            {
                newConditionChecking = false;
                return;
            }
            var item = startExpandNetworks.Dequeue();
            ExpandAndSearch(item);
        }

    }

    //ノードの追加、wholeGraphとnodesDictの更新を行う。
    public void AddNode(GameObject node)
    {
        wholeGraph.Add(node);
        BlockInfo info = node.GetComponent<BlockInfo>();
        if (System.Array.Exists(primeNumberPool, element => element == info.GetPrimeNumber()))
        {
            if (!nodesDict.ContainsKey(info.GetPrimeNumber()))
            {
                nodesDict.Add(info.GetPrimeNumber(), new List<GameObject>());
            }
            nodesDict[info.GetPrimeNumber()].Add(node);
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
        wholeGraph.Remove(originNode);
        nodesDict[originNode.GetComponent<BlockInfo>().GetPrimeNumber()].Remove(originNode);
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

    //サブネットワーク内のGameObjectを物理的に結合し色を変更し、仮想的なネットワークから切り離すメソッド
    private void FreezeNodes(List<GameObject> nodes)
    {
        for(int i=1; i<nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
        SafeCutNodes(nodes);
        ChangeColorNodes(nodes);
    }

    private void SetCriteriaMetLayerAndMaterial(List<GameObject> nodes)
    {
        foreach(var node in nodes)
        {
            node.layer = LayerMask.NameToLayer("CriteriaMet");
            node.GetComponent<SpriteRenderer>().material = CriteriaMetMaterial;
        }
    }

    //第二引数で指定した時間後、第一引数で指定したゲームオブジェクトのリストに、物理的な計算を行わなくするメソッド。空中に固定し、水色にする。
    IEnumerator FreezeBlocks(List<GameObject> nodes, float second)
    {
        yield return new WaitForSeconds(second);
        foreach (var node in nodes)
        {
            Rigidbody2D rb2d = node.GetComponent<Rigidbody2D>();
            rb2d.velocity = Vector3.zero;
            rb2d.angularVelocity = 0;
            rb2d.isKinematic = true;
            node.GetComponent<SpriteRenderer>().color = GameInfo.FleezeColor;
        }
        yield break;
    }

    //条件を満たしたときの処理
    private void CompleteConditionsProcess(List<GameObject> nodes)
    {
        switch (gameModeManager.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                ProcessFreeze(nodes);
                break;
            case GameModeManager.GameMode.Battle:
                ProcessFreeze(nodes);
                break;
        }
        //後処理
        startExpandNetworks = new Queue<ExpandNetwork>(); //探索が完了したらもうネットワーク内に条件を満たすものが存在しないと考えられるので、キューをリセットしておく。(あるとバグが発生する)
        newConditionChecking = true;
        CheckConditionBlocksGraph();
        conditionGenerator.GenerateCondition();
    }

    //条件を満たしたノードに対して、Freeze処理を行う
    private void ProcessFreeze(List<GameObject> nodes)
    {
        SetCriteriaMetLayerAndMaterial(nodes); //条件を満たしたブロックの色を変更
        FreezeNodes(nodes); //凍らせる
        effectTextManager.DisplayEffectText("Criteria Met", freezeDelayTime, GameInfo.FleezeColor); //条件達成時のUIを表示
        soundManager.PlayAudio(soundManager.VOICE_CRITERIAMAT); //条件達成時のSEを再生

        DelayProcessFreeze(nodes, freezeDelayTime); //Freezeの後処理。(こっちで空中に固定する)
    }

    //第二引数で指定した時間後、Freezeの文字、サウンド、エフェクトを出力し、第一引数で指定したGameObjectのリストを空中に固定する
    private void DelayProcessFreeze(List<GameObject> nodes, float freezeDelayTime)
    {
        Vector3 nodesCenter = CaluculateCenter(nodes);
        StartCoroutine(FreezeBlocks(nodes, freezeDelayTime));
        StartCoroutine(soundManager.PlayAudio(soundManager.VOICE_FREEZE, freezeDelayTime));
        StartCoroutine(soundManager.PlayAudio(soundManager.SE_FREEZE, freezeDelayTime));
        StartCoroutine(effectTextManager.DisplayEffectText("Freeze", freezeDelayTime, 0, GameInfo.FleezeColor));
        StartCoroutine(InstantiateEffect(freezeEffect, nodesCenter, freezeDelayTime));
    }

    //引数で与えられたゲームオブジェクトたちの重心を計算して返すメソッド
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

    //第三引数で指定した時間後に、第二引数で指定した位置に、第一引数で指定したeffect(GameObject)を生成するメソッド
    private IEnumerator InstantiateEffect(GameObject effect, Vector3 position, float second)
    {
        yield return new WaitForSeconds (second);
        Instantiate(effect, position, Quaternion.identity);
    }

    ////////////////////////////////////
    //以下グラフ全体内部の探索で使うメソッド//
    ////////////////////////////////////

    //グラフ全体に条件にマッチするものがないかを探索するためのメソッド 条件に存在する素数のうち、ネットワーク全体で最小個数の素数を探し、そのノードから探索を始める
    void CheckConditionBlocksGraph()
    {
        int minNode = -1; //最小個数の素数
        int minNodeNum = int.MaxValue; //最小個数の素数の数

        //条件に存在する素数を全探索し、最小個数のものを探す
        foreach (int valueInFreezeCondition in conditionGenerator.ConditionNumberDict.Keys)
        {
            if(minNodeNum > nodesDict[valueInFreezeCondition].Count)
            {
                minNodeNum = nodesDict[valueInFreezeCondition].Count;
                minNode = valueInFreezeCondition;
            }
        }
        //最小個数の素数はすでに求まっているので、それに対してfor分を回してstartExpandNetworks
        foreach(var node in nodesDict[minNode])
        {
            startExpandNetworks.Enqueue(new ExpandNetwork(null, node, conditionGenerator.ConditionNumberDict));
        }
    }

    //第二引数で指定した条件を満たすサブグラフの条件を、第一引数で指定したネットワークが満たしているかをチェックするメソッド
    bool ContainsAllRequiredNodes(List<GameObject> blocksGraph, Dictionary<int, int> requiredNodesDict)
    {
        Dictionary<int, int> requiredCounts = new Dictionary<int, int>(requiredNodesDict);
        //Debug.Log(string.Join(", ", requiredCounts));

        //現在のネットワーク内のノードの出現回数をカウント
        foreach (var node in blocksGraph)
        {
            int nodeValue = node.GetComponent<BlockInfo>().GetPrimeNumber();
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

    //ネットワークからサブグラフを探索する拡張前のExpandNetworkを、拡張するネットワークを格納するキューに追加する。Update内でこのキューから要素が取り出され、自動で探索が始まる。
    public void AddStartExpandNetworks(HashSet<GameObject> neiborSet)
    {
        ExpandNetwork currentNetwork = null;
        foreach (var startNode in neiborSet)
        {
            if(currentNetwork == null)
            {
                currentNetwork = new ExpandNetwork(null, startNode, conditionGenerator.ConditionNumberDict);
            }
            else
            {
                currentNetwork = new ExpandNetwork(currentNetwork, startNode, conditionGenerator.ConditionNumberDict);
            }
        }
        //Debug.Log(string.Join(", ",currentNetwork.myNetwork));
        startExpandNetworks.Enqueue(currentNetwork);
    }

    //ネットワークを拡張しながらサブグラフを探索する再帰的メソッド
    private void ExpandAndSearch(ExpandNetwork currentNetwork)
    {
        //拡張したネットワークが条件を満たしていたら
        if (ContainsAllRequiredNodes(currentNetwork.myNetwork, conditionGenerator.ConditionNumberDict))
        {
            //条件生成時に既に条件を達成していた場合→条件を再生成して再び調査。
            if (newConditionChecking) 
            {
                conditionGenerator.GenerateCondition(); //条件を生成し
                CheckConditionBlocksGraph(); //その条件がすでに達成していないかチェック。達成したら再度ここの条件分岐に入るので、もう一度生成される。
                Debug.Log("再生成");
                return;
            }
            Debug.Log(string.Join(", ", currentNetwork.myNetwork));
            CompleteConditionsProcess(currentNetwork.myNetwork);
            return;
        }

        //現在拡張中のネットワークに存在する各ノードの隣接ノードを探索
        foreach (var node in currentNetwork.myNetwork)
        {
            //今見ているノードに隣接するノードを全てリストに追加
            List<GameObject> adjacentNodes = node.GetComponent<BlockInfo>().GetNeighborEdge();

            //現在のネットワークとclosedListに含まれていないノードのみを選択
            adjacentNodes = adjacentNodes.Where(n => !currentNetwork.closedList.Contains(n) && !currentNetwork.myNetwork.Contains(n)).ToList();

            //隣接する新しいノードがなければスキップ
            if (adjacentNodes.Count == 0)
            {
                continue; 
            }

            //今見ているノードに隣接するノードを探索
            foreach (var adjacentNode in adjacentNodes)
            {
                if (adjacentNode.gameObject.GetComponent<BlockInfo>().enabled == false) continue; //ネットワークから切り離されてblockinfoがなくなったブロックがadjacentNodesに含まれれば、処理をスキップ
                ExpandNetwork newNetwork = new ExpandNetwork(currentNetwork, adjacentNode, conditionGenerator.ConditionNumberDict); //拡張

                //場合によっては拡張前に戻る
                if (newNetwork.BackFlag)
                {
                    newNetwork = newNetwork.Beforenetwork;
                }
                
                //再帰呼び出し
                ExpandAndSearch(newNetwork);
            }
        }
    }
}