using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UI;
using Common;
using static BlocksGraphData;

//積み上げれたブロックはブロックがノード、隣接関係がエッジとなるグラフ構造をしており。それを管理するためのクラス。
public class BlocksGraphManager : MonoBehaviour
{
   //サブグラフの探索
    int CheckNumParFrame = 20; //1フレーム当たりにキューから取り出す数

    //条件のチェックに使用
    ConditionManager conditionGenerator;
    CriteriaMetChecker criteriaMetChecker;

    private void Awake()
    {
        //初期化処理
        InitializeBlocksGraph();
        conditionGenerator = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        criteriaMetChecker = GameObject.Find("CriteriaMetChecker").GetComponent<CriteriaMetChecker>();
    }

    private void Update()
    {
        //checkNumParFrameの整数値回だけキューに入っていて条件を満たすものがないかネットワーク内でチェックを行う。
        for (int i = 0; i < CheckNumParFrame; i++)
        {
            if (StartExpandNetworks.Count == 0)
            {
                SetConditionChecking(false);
                return;
            }
            var item = DequeueStartExpandNetworks();
            criteriaMetChecker.ExpandAndSearch(item);
        }

    }

    //ネットワークからサブグラフを探索する拡張前のExpandNetworkを、拡張するネットワークを格納するキューに追加する。Update内でこのキューから要素が取り出され、自動で探索が始まる。
    public void AddStartExpandNetworks(HashSet<GameObject> neiborSet)
    {
        if (NewConditionGenerating) return; //条件をチェックしている最中には、ブロックの接触による条件をチェックするキューへの追加は行わない
        ExpandNetwork currentNetwork = null;
        foreach (var startNode in neiborSet)
        {
            if (currentNetwork == null)
            {
                currentNetwork = new ExpandNetwork(null, startNode, conditionGenerator.ConditionNumberDict);
            }
            else
            {
                currentNetwork = new ExpandNetwork(currentNetwork, startNode, conditionGenerator.ConditionNumberDict);
            }
        }
        EnqueueStartExpandNetworks(currentNetwork);
    }

}