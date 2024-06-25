using Common;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using static BlocksGraphData;


//条件が満たされた場合に行う行動を定義したクラス
public class CriteriaMetProcessor : MonoBehaviour
{
    static readonly float freezeDelayTime = 1.5f;

    //条件達成時の処理 ※ゲームモードごとに条件達成時の処理が変わる可能性がある。
    [SerializeField] Material CriteriaMetMaterial;
    GameObject freezeEffect;
    EffectTextManager effectTextManager;
    void Start()
    {
        effectTextManager = GameObject.Find("EffectText").GetComponent<EffectTextManager>();
        freezeEffect = (GameObject)Resources.Load("FreezeEffect");
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
        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
        SafeCutNodes(nodes);
        ChangeColorNodes(nodes);
    }

    private void SetCriteriaMetLayerAndMaterial(List<GameObject> nodes)
    {
        foreach (var node in nodes)
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

    /// <summary>
    /// 条件を満たしたノードに対して、Freeze処理を行う
    /// </summary>
    /// <param name="nodes">条件を満たしたノード</param>
    public void ProcessFreeze(List<GameObject> nodes)
    {
        SetCriteriaMetLayerAndMaterial(nodes); //条件を満たしたブロックの色を変更
        FreezeNodes(nodes); //凍らせる
        effectTextManager.DisplayEffectText("Criteria Met", freezeDelayTime, GameInfo.FleezeColor); //条件達成時のUIを表示
        SoundManager.Ins.PlayAudio(SoundManager.Ins.VOICE_CRITERIAMAT); //条件達成時のSEを再生

        DelayProcessFreeze(nodes, freezeDelayTime); //Freezeの後処理。(こっちで空中に固定する)
    }

    //第二引数で指定した時間後、Freezeの文字、サウンド、エフェクトを出力し、第一引数で指定したGameObjectのリストを空中に固定する
    private void DelayProcessFreeze(List<GameObject> nodes, float freezeDelayTime)
    {
        Vector3 nodesCenter = Helper.CaluculateCenter(nodes);
        StartCoroutine(FreezeBlocks(nodes, freezeDelayTime));
        StartCoroutine(SoundManager.Ins.PlayAudio(SoundManager.Ins.VOICE_FREEZE, freezeDelayTime));
        StartCoroutine(SoundManager.Ins.PlayAudio(SoundManager.Ins.SE_FREEZE, freezeDelayTime));
        StartCoroutine(effectTextManager.DisplayEffectText("Freeze", freezeDelayTime, freezeDelayTime, GameInfo.FleezeColor));
        StartCoroutine(InstantiateEffect(freezeEffect, nodesCenter, freezeDelayTime));
    }

    //第三引数で指定した時間後に、第二引数で指定した位置に、第一引数で指定したeffect(GameObject)を生成するメソッド
    private IEnumerator InstantiateEffect(GameObject effect, Vector3 position, float second)
    {
        yield return new WaitForSeconds(second);
        Instantiate(effect, position, Quaternion.identity);
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
        WholeGraphRemove(originNode);
        BlocksDictRemoveSingleBlock(originNode);
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
}
