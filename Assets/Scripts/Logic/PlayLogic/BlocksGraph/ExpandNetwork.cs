using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// このネットワークを拡張していきpatternにマッチしたgraphを探索する。
/// 拡張するとは拡張されたインスタンスを生成することで、後退するとはbeforeNetworkに戻り、戻る前に追加していたリストをクローズドリストに追加する。
/// ネットワーク内に特定のパターンがあるかどうか見つけるための、拡張されていく特殊なデータ構造
/// 特定のサブグラフを探索するための独自のアルゴリズムに使用するデータ構造
/// </summary>

public class ExpandNetwork
{
    List<GameObject> myNetwork = new List<GameObject>(); //現在のネットワーク情報
    List<GameObject> closedList = new List<GameObject>(); //現在のネットワークに対するクローズドリスト。これより小さいクローズドリストの情報も引き継ぐ。

    public List<GameObject> MyNetwork => myNetwork;
    public List<GameObject> ClosedList => closedList;

    ExpandNetwork beforeNetwork; //ひとつ前のネットワークに戻ることがあるので必要
    public ExpandNetwork Beforenetwork => beforeNetwork;
    bool backFlag = false;
    public bool BackFlag => backFlag;

    /// <summary>
    /// コンストラクタ。呼び出す側から見て、現在のネットワークと追加したいノードを引数で指定する。
    /// </summary>
    /// <param name="beforeNetwork">現在のネットワーク</param>
    /// <param name="addNode">ネットワークに追加するノード</param>
    /// <param name="condition">現在のcondition</param>
    public ExpandNetwork(ExpandNetwork beforeNetwork, GameObject addNode, Dictionary<int, int> condition)
    {
        //beforeNetworkがあるということはnowNodeは１番目以降のノード => closedListとmyNetworkをbeforeNetworkで初期化
        if (beforeNetwork != null)
        {
            closedList = new List<GameObject>(beforeNetwork.closedList);
            myNetwork = new List<GameObject>(beforeNetwork.myNetwork);
            this.beforeNetwork = beforeNetwork;
        }
        if (beforeNetwork != null) beforeNetwork.closedList.Add(addNode); //一個前のネットワークに今追加したノードをクローズドリストに追加する。

        int nodeValue = addNode.GetComponent<BlockInfo>().GetPrimeNumber();
        //ネットワークに追加される素数が、条件を満たすのかのチェック
        if (condition.ContainsKey(nodeValue))
        {
            int requiredCount = condition[nodeValue];
            int currentCount = myNetwork.Count(node => node.GetComponent<BlockInfo>().GetPrimeNumber() == nodeValue);

            //追加したいノードが、条件を満たすための数が足りていなければ追加
            if (currentCount < requiredCount)
            {
                myNetwork.Add(addNode);
            }
            //もうすでに足りていたなら元のネットワークに戻る
            else
            {
                backFlag = true;
            }
        }
        else
        {
            backFlag= true;
        }
        closedList.Add(addNode);
        if(addNode == null) Debug.LogError("nowNodeNull");
    }

}