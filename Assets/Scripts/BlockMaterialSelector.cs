using Common;
using MaterialLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

//今選択中の単一のゲームオブジェクトを管理するクラス
public class BlockMaterialSelector : MonoBehaviour
{
    int nowBlockIndex = 0;
    int[] allprimenumber;
    GameObject singleBlockParent;
    BlockNumberSetter blockNumberSetter;
    MaterialDatabaseManager materialDatabaseManager;

    public int NowBlockNum => allprimenumber[nowBlockIndex];

    private void Start()
    {
        allprimenumber = GameModeManager.Ins.PrimeNumberPool;
        singleBlockParent = GameObject.Find("SingleBlockParent");
        blockNumberSetter = GameObject.Find("BlockNumberText").GetComponent<BlockNumberSetter>();
        materialDatabaseManager = GameObject.Find("MaterialDatabaseManager").GetComponent<MaterialDatabaseManager>();
        SetSingleBlock();
    }

    void SetSingleBlock()
    {
        InitializeSingleBlockParent();
        GenerateBlock();
        blockNumberSetter.SetBlockNumber(NowBlockNum);
    }

    GameObject GetSingleBlock()
    {
        if (singleBlockParent.transform.GetChild(0).gameObject)
        {
            return singleBlockParent.transform.GetChild(0).gameObject;
        }
        else
        {
            Debug.LogError("SingleBlockが取得できませんでした。");
            return null;
        }
        
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

    //特定のマテリアルで単一のブロックを初期化する。
    public void SetBlockMaterialDataToSingleBlock<TEnum>() where TEnum : Enum
    {
        MaterialDatabase materialDatabase = materialDatabaseManager.TmpMaterialDatabase;
        BlockMaterialData blockMaterialData = materialDatabase.GetBlockMaterialData(NowBlockNum);
        if (blockMaterialData != null)
        {
            IEnumParametersBinder binder = EnumParameterBinderManager.Binders[blockMaterialData.binderIndex];//現在のマテリアル

            foreach (ParameterData parameter in blockMaterialData.parameters)
            {
                //Debug.Log($"パラメータータイプ: {parameter.type}, パラメータープロパティインデックス: {parameter.parameterEnumIndex}");
                //float型のパラメーター
                if (parameter.type == 0)
                {
                    //binderのメソッドと、Enumのインデックス情報を使い、parameterからパラメーターを調整する
                    binder.SetPropertyFloat<TEnum>(EnumManager.GetEnumValueFromIndex<TEnum>(parameter.parameterEnumIndex), parameter.floatValue);
                }
                else if (parameter.type == 1)
                {
                    //parameterから色の生成
                    Color color = new Color(parameter.redValue, parameter.greenValue, parameter.blueValue);
                    //binderのメソッドと、Enumのインデックス情報を使い、上で生成した色を割り当てる。
                    binder.SetPropertyColor<TEnum>(EnumManager.GetEnumValueFromIndex<TEnum>(parameter.parameterEnumIndex), color);
                }
                else
                {
                    Debug.LogError($"想定外のtypeが指定されました。: {parameter.type}");
                }
                //Debug.Log($"パラメータータイプ: {parameter.type}, パラメータープロパティインデックス: {parameter.parameterEnumIndex}");
            }
            GameObject singleBlock = GetSingleBlock();
            SpriteRenderer spriteRenderer = singleBlock.GetComponent<SpriteRenderer>();
            spriteRenderer.material = new Material(binder.Material);

        }
        else
        {
            Debug.LogError("blockMaterialDataが取得できませんでした。");
        }
    }
}
