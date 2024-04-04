using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMaterialSelector : MonoBehaviour
{
    int nowBlockIndex = 0;
    int[] allprimenumber;
    GameObject singleBlockParent;
    BlockNumberSetter blockNumberSetter;

    public int NowBlockNum => allprimenumber[nowBlockIndex];

    private void Start()
    {
        allprimenumber = GameModeManager.Ins.PrimeNumberPool;
        singleBlockParent = GameObject.Find("SingleBlockParent");
        blockNumberSetter = GameObject.Find("BlockNumberText").GetComponent<BlockNumberSetter>();
        SetSingleBlock();
    }

    void SetSingleBlock()
    {
        InitializeSingleBlockParent();
        GenerateBlock();
        blockNumberSetter.SetBlockNumber(NowBlockNum);
    }

    void GenerateBlock()
    {
        GameObject resourcesBlock = Resources.Load($"Block{NowBlockNum}") as GameObject;
        if(resourcesBlock == null)
        {
            Debug.LogError($"指定されたブロックがResourcesから見つかりませんでした。: {NowBlockNum}");
            return;
        }
        GameObject nowBlock = Instantiate(resourcesBlock);

        //不必要なコンポーネントを無効に
        nowBlock.GetComponent<LineViewer>().enabled = false;
        nowBlock.GetComponent<LineRenderer>().enabled = false;
        nowBlock.GetComponent<TouchBlock>().enabled = false;

        //ブロックに表示する文字の設定
        BlockInfo blockInfo = nowBlock.GetComponent<BlockInfo>();
        blockInfo.SetPrimeNumber(NowBlockNum);
        blockInfo.SetText();

        nowBlock.transform.SetParent(singleBlockParent.transform);
        nowBlock.transform.localPosition = Vector3.zero;
    }

    void InitializeSingleBlockParent()
    {
        foreach (Transform block in singleBlockParent.transform) 
        {
            Destroy(block.gameObject);
        }
    }

    public void SetNextBlock()
    {
        nowBlockIndex++;
        if(nowBlockIndex > allprimenumber.Length-1) nowBlockIndex = 0;
        SetSingleBlock() ;
    }

    public void SetPreviousBlock()
    {
        nowBlockIndex--;
        if(nowBlockIndex < 0) nowBlockIndex = allprimenumber.Length-1;
        SetSingleBlock() ;
    }
}
