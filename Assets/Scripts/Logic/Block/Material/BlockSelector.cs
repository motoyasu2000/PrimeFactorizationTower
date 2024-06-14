using Common;
using MaterialLibrary;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MaterialScene内で、どのブロックを選択するのかを指定したり、そのブロックに対して色を割り当てたりするクラス。
/// </summary>
public class BlockSelector : MonoBehaviour
{
    int nowBlockIndex = 0;
    int[] allprimenumber;
    Transform materialButtonsParent;
    GameObject singleBlockParent;
    BlockNumberSetter blockNumberSetter;
    MaterialDatabaseManager materialDatabaseManager;

    //現在選ばれているブロックの数値
    public int NowBlockNum => allprimenumber[nowBlockIndex];

    private void Start()
    {
        allprimenumber = GameModeManager.Ins.PrimeNumberPool;
        materialButtonsParent = GameObject.Find("MaterialButtonsPanel").transform;
        singleBlockParent = GameObject.Find("SingleBlockParent");
        blockNumberSetter = GameObject.Find("BlockNumberText").GetComponent<BlockNumberSetter>();
        materialDatabaseManager = GameObject.Find("MaterialDatabaseManager").GetComponent<MaterialDatabaseManager>();
        SetSingleBlock(); //実行時にブロックを配置
    }

    /// <summary>
    /// 今何のブロックが選ばれているのかを指定するメソッド。
    /// </summary>
    void SetSingleBlock()
    {
        InitializeSingleBlockParent();
        GenerateBlock();
        blockNumberSetter.SetBlockNumber(NowBlockNum);
        InvokeNowBlockMaterialButton();
    }

    /// <summary>
    /// 単一のゲームオブジェクトが入る親オブジェクトの子要素を全部消去する。
    /// 子オブジェクトの追加の際に初期化のために実行される。
    /// </summary>
    void InitializeSingleBlockParent()
    {
        foreach (Transform block in singleBlockParent.transform)
        {
            Destroy(block.gameObject);
        }
    }

    /// <summary>
    /// NowBlockNumに対応するブロックを生成する。
    /// </summary>
    void GenerateBlock()
    {
        GameObject resourcesBlock = Resources.Load($"Block{NowBlockNum}") as GameObject;
        if (resourcesBlock == null)
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

        //親の設定と位置の調整
        nowBlock.transform.SetParent(singleBlockParent.transform);
        nowBlock.transform.localPosition = Vector3.zero;
    }

    //現在選ばれているブロックを戻す
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

    //次の素数のブロックを生成
    public void SetNextBlock()
    {
        materialDatabaseManager.LoadMaterialDatabase(); //切り替わる前に設定していた情報を消去
        nowBlockIndex++;
        if(nowBlockIndex > allprimenumber.Length-1) nowBlockIndex = 0;
        SetSingleBlock();
    }

    //前の素数のブロックを生成
    public void SetPreviousBlock()
    {
        materialDatabaseManager.LoadMaterialDatabase();　//切り替わる前に設定していた情報を消去
        nowBlockIndex--;
        if(nowBlockIndex < 0) nowBlockIndex = allprimenumber.Length-1;
        SetSingleBlock() ;
    }

    /// <summary>
    /// 現在選択中のブロックのマテリアルを設定する
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型からプロパティを設定するか</typeparam>
    public void SetBlockMaterialDataToSingleBlock<TEnum>() where TEnum : Enum
    {
        //中間のマテリアルを取得
        MaterialDatabase materialDatabase = materialDatabaseManager.MiddleMaterialDatabase;

        //取得したデータベースから現在表示されているブロックのものを取得
        BlockMaterialData blockMaterialData = materialDatabase.GetBlockMaterialData(NowBlockNum);

        if (blockMaterialData != null)
        {
            IBinder binder = BinderManager.Binders[blockMaterialData.binderIndex];//現在のマテリアル

            foreach (ParameterData parameter in blockMaterialData.parameters)
            {
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
            }

            //現在選択されているブロックを取得して、実際にマテリアルを適用する
            GameObject singleBlock = GetSingleBlock();
            SpriteRenderer spriteRenderer = singleBlock.GetComponent<SpriteRenderer>();
            spriteRenderer.material = new Material(binder.Material);
        }
        else
        {
            Debug.LogError("blockMaterialDataが取得できませんでした。");
        }
    }
    //初期化のために現在選択中のブロックに割り当てられているマテリアルに対応したMaterialButtonをクリックする。
    void InvokeNowBlockMaterialButton()
    {
        int nowMaterialIndex = 0;

        //全てのマテリアルを検索して
        foreach (var ibinder in BinderManager.Binders)
        {
            //現在の選択中のブロックのマテリアルのMaterialButtonがあれば
            if (BinderManager.GetBindersIndex(ibinder) == materialDatabaseManager.MiddleMaterialDatabase.GetBlockMaterialData(NowBlockNum).binderIndex)
            {
                //そのMaterialButtonをクリックする。
                Button materialButton = materialButtonsParent.GetChild(nowMaterialIndex).GetComponent<Button>();
                materialButton.onClick.Invoke();
            }
            nowMaterialIndex++;
        }
    }

}
