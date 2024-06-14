//このネットワークを拡張していきpatternにマッチしたgraphを探索する。拡張するとは拡張されたインスタンスを生成することで、後退するとはbeforeNetworkに戻り、戻る前に追加していたリストをクローズドリストに追加する。
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//ネットワーク内に特定のパターンがあるかどうか見つけるための、拡張されていく特殊なデータ構造
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

    //コンストラクタ。呼び出す側から見て、現在のネットワークと追加したいノードを引数で指定する。
    public ExpandNetwork(ExpandNetwork beforeNetwork, GameObject nowNode, Dictionary<int, int> freezeCondition)
    {
        //beforeNetworkがあるということはnowNodeは１番目以降のノード => closedListとmyNetworkをbeforeNetworkで初期化
        if (beforeNetwork != null)
        {
            closedList = new List<GameObject>(beforeNetwork.closedList);
            myNetwork = new List<GameObject>(beforeNetwork.myNetwork);
            this.beforeNetwork = beforeNetwork;
        }
        if (beforeNetwork != null) beforeNetwork.closedList.Add(nowNode); //一個前のネットワークに今追加したノードをクローズドリストに追加する。

        int nodeValue = nowNode.GetComponent<BlockInfo>().GetPrimeNumber();
        //ネットワークに追加される素数が、条件を満たすのかのチェック
        if (freezeCondition.ContainsKey(nodeValue))
        {
            int requiredCount = freezeCondition[nodeValue];
            int currentCount = myNetwork.Count(node => node.GetComponent<BlockInfo>().GetPrimeNumber() == nodeValue);

            //追加したいノードが、条件を満たすための数が足りていなければ追加
            if (currentCount < requiredCount)
            {
                myNetwork.Add(nowNode);
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
        closedList.Add(nowNode);
        //Debug.Log(string.Join(", ", closedList));
        //Debug.Log(string.Join(", ", myNetwork));
        if(nowNode == null) Debug.LogError("nowNodeNull");
    }

    // ネットワークに隣接ノードを追加するメソッド（重複を防ぐ）
    public void AddAdjacentNodes(List<GameObject> adjacentNodes)
    {
        foreach (var node in adjacentNodes)
        {
            if (!myNetwork.Contains(node) && !closedList.Contains(node))
            {
                myNetwork.Add(node);
                closedList.Add(node);
            }
        }
    }
}