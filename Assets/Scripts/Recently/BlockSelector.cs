using Common;
using MaterialLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VolumeComponent;

//今選択中の単一のゲームオブジェクトを管理するクラス
public class BlockSelector : MonoBehaviour
{
    int nowBlockIndex = 0;
    int[] allprimenumber;
    Transform materialButtonsParent;
    GameObject singleBlockParent;
    GameObject singleBlock;
    BlockNumberSetter blockNumberSetter;
    MaterialDatabaseManager materialDatabaseManager;

    public int NowBlockNum => allprimenumber[nowBlockIndex];
    public GameObject SingleBlock => singleBlockParent.transform.GetChild(0).gameObject;

    private void Start()
    {
        allprimenumber = GameModeManager.Ins.PrimeNumberPool;
        materialButtonsParent = GameObject.Find("MaterialButtonsPanel").transform;
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
        InvokeNowBlockMaterialButton();
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
        materialDatabaseManager.LoadMaterialDatabase(); //切り替わる前に設定していた情報を消去
        nowBlockIndex++;
        if(nowBlockIndex > allprimenumber.Length-1) nowBlockIndex = 0;
        SetSingleBlock();
    }

    public void SetPreviousBlock()
    {
        materialDatabaseManager.LoadMaterialDatabase();　//切り替わる前に設定していた情報を消去
        nowBlockIndex--;
        if(nowBlockIndex < 0) nowBlockIndex = allprimenumber.Length-1;
        SetSingleBlock() ;
    }

    //特定のマテリアルで単一のブロックを初期化する。
    public void SetBlockMaterialDataToSingleBlock<TEnum>() where TEnum : Enum
    {
        //保存されているデータベースから直接データを持ってくる。なければ中間のマテリアルを取得
        MaterialDatabase materialDatabase = PlayerInfoManager.Ins.MaterialDatabase;
        if(materialDatabase == null) materialDatabase = materialDatabaseManager.MiddleMaterialDatabase;

        //取得したデータベースから現在表示されているブロックのものを取得
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
    void InvokeNowBlockMaterialButton()
    {
        int nowMaterialIndex = 0;
        //今表示されているブロックに割り当てられている方のボタンを一度クリックする
        foreach (var ibinder in EnumParameterBinderManager.Binders)
        {
            if (EnumParameterBinderManager.GetBindersIndex(ibinder) == materialDatabaseManager.MiddleMaterialDatabase.GetBlockMaterialData(NowBlockNum).binderIndex)
            {
                materialButtonsParent.GetChild(nowMaterialIndex).GetComponent<Button>().onClick.Invoke();
            }
            nowMaterialIndex++;
        }
    }

}
