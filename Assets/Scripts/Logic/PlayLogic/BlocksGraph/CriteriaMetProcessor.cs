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

    /// <summary>
    /// 条件を満たした瞬間に呼ばれる処理。
    /// </summary>
    /// <param name="nodes">条件を満たしたノード</param>
    public void ProcessCriteriaMet(List<GameObject> nodes)
    {
        CombineNodes(nodes);
        SafeCutNodes(nodes);
        ChangeCriteriaMetAppearance(nodes);
        effectTextManager.DisplayEffectText("Criteria Met", freezeDelayTime, GameInfo.FleezeColor); //条件達成時のUIを表示
        SoundManager.Ins.PlayAudio(SoundManager.Ins.VOICE_CRITERIAMAT); //条件達成時のSEを再生

        RunPostProcess(nodes);
    }

    void RunPostProcess(List<GameObject> nodes)
    {
        //現在の条件に合わせて異なる後処理を行う。
        switch (GameModeManager.Ins.NowGameMode)
        {
            case GameModeManager.GameMode.PileUp:
                ProcessFreeze(nodes, freezeDelayTime);
                break;
            case GameModeManager.GameMode.PileUp_60s:
                ProcessFreeze(nodes, freezeDelayTime);
                break;
            case GameModeManager.GameMode.Battle:
                ProcessFreeze(nodes, freezeDelayTime);
                break;
        }
    }

    /// <summary>
    /// 左上のconditionを満たした数秒後に呼ばれる処理、複数のブロックを空中に固定させて動かなくする。
    /// pileUpや対戦モードでの条件達成時の後処理
    /// </summary>
    /// <param name="nodes">条件を満たしたノード</param>
    private void ProcessFreeze(List<GameObject> nodes, float freezeDelayTime)
    {
        Vector3 nodesCenter = Helper.CaluculateCenter(nodes);
        StartCoroutine(FreezeBlocks(nodes, freezeDelayTime));
        StartCoroutine(InstantiateEffect(freezeEffect, nodesCenter, freezeDelayTime));
        StartCoroutine(SoundManager.Ins.PlayAudio(SoundManager.Ins.VOICE_FREEZE, freezeDelayTime));
        StartCoroutine(SoundManager.Ins.PlayAudio(SoundManager.Ins.SE_FREEZE, freezeDelayTime));
        StartCoroutine(effectTextManager.DisplayEffectText("Freeze", freezeDelayTime, freezeDelayTime, GameInfo.FleezeColor));
    }

    /// <summary>
    /// 条件を満たしたブロック同士を結合させる。
    /// </summary>
    /// <param name="nodes">条件を満たしたノード</param>
    void CombineNodes(List<GameObject> nodes)
    {
        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i].AddComponent<FixedJoint2D>().connectedBody = nodes[i - 1].GetComponent<Rigidbody2D>();
        }
    }

    //ネットワークから特定のノードを切り離すメソッド(Destroyはしない)
    private void SafeCutNode(GameObject node)
    {
        //切り離し元のノードと隣接するエッジを一時的な変数に格納(コレクションがイテレーション中に変更してはならないため)
        List<GameObject> tmpNeighborNode = new List<GameObject>();
        foreach (var neighborNode in node.GetComponent<BlockInfo>().GetNeighborEdge())
        {
            tmpNeighborNode.Add(neighborNode);
        }
        //切り離し元のノードと隣接するエッジを実際に削除する
        foreach (var neighborNode in tmpNeighborNode)
        {
            DetachNode(neighborNode, node);
        }
        RemoveNode(node);
        node.GetComponent<BlockInfo>().enabled = false;
    }

    /// <summary>
    /// ネットワークから複数のノードを切りはなす
    /// </summary>
    /// <param name="nodes">条件を達成したノード</param>
    private void SafeCutNodes(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            SafeCutNode(node);
        }
    }

    /// <summary>
    /// 引数で指定したノード(ブロック)の見た目を条件達成時のものに変更する
    /// </summary>
    private void ChangeCriteriaMetAppearance(List<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            node.layer = LayerMask.NameToLayer("CriteriaMet"); //AIがこの状態を見れるようにする
            node.GetComponent<SpriteRenderer>().material = CriteriaMetMaterial;
            node.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    /// <summary>
    /// 一定時間経過した後、条件を満たしたブロックに対して空中に固定し、びくともしなくさせる。
    /// また、色も変更する
    /// </summary>
    /// <param name="nodes">条件を満たしたノード(ブロック)</param>
    /// <param name="second">この処理を行うのに、どのくらい待つか</param>
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
    }

    /// <summary>
    /// 一定時間経過後にエフェクトを生成する
    /// </summary>
    /// <param name="effect">どのエフェクトを生成するか</param>
    /// <param name="position">どの位置に生成するか</param>
    /// <param name="second">何秒後に生成するか</param>
    private IEnumerator InstantiateEffect(GameObject effect, Vector3 position, float second)
    {
        yield return new WaitForSeconds(second);
        Instantiate(effect, position, Quaternion.identity);
    }
}
