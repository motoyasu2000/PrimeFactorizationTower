using Common;
using UnityEngine;
using MaterialLibrary;
using System;
using System.Reflection;
using UnityEngine.UI;

/// <summary>
/// MaterialSceneでブロックに割り当てるマテリアルを選択するためのボタンを生成するクラス
/// </summary>
public class MaterialButtonsGenerator : MonoBehaviour
{
    //ボタン間に設ける間隔
    static readonly float widthSpace = 20;
    static readonly float heightSpace = 20;

    int generateButtonCounter = 0;
    float[] splitAnchorPoints_x = Helper.CalculateSplitAnchorPoints(BinderManager.BindersCount);
    GameObject materialButtonPrefab;
    ParameterSliderGenerator sliderGenerater;
    MaterialDatabaseManager materialDatabaseManager;
    BlockSelector blockSelector;
    SaveUIManager saveUIManager;
    void Start()
    {
        materialButtonPrefab = Resources.Load("MaterialButton") as GameObject;
        sliderGenerater = GameObject.Find("ParameterSlidersPanel").GetComponent<ParameterSliderGenerator>();
        materialDatabaseManager = GameObject.Find("MaterialDatabaseManager").GetComponent<MaterialDatabaseManager>();
        blockSelector = GameObject.Find("BlockMaterialSelector").GetComponent<BlockSelector>();
        saveUIManager = FindObjectOfType<SaveUIManager>();
        GenerateMaterialButtons();
    }


    //BinderManager.Bindersに定義されている全てのマテリアルを操作するためのボタンを生成する。リフレクションを使ってGenerateMaterialBlockのジェネリックのenumを指定できるようにしている。
    void GenerateMaterialButtons()
    {
        foreach (var binder in BinderManager.Binders)
        {
            MethodInfo methodInfo = typeof(MaterialButtonsGenerator).GetMethod(nameof(GenerateMaterialButton), BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
            {
                MethodInfo genericMethod = methodInfo.MakeGenericMethod(binder.EnumType);
                genericMethod.Invoke(this, new object[] { binder });
            }
            else
            {
                Debug.LogError("指定されたメソッドが見つかりませんでした。");
            }
            generateButtonCounter++;
        }
        generateButtonCounter = 0;
    }

    //ジェネリックで指定されたenumに対応するパラメーターを調整できるようにするためのボタンを生成する
    void GenerateMaterialButton<TEnum>(IBinder ibinder) where TEnum : Enum
    {
        GameObject materialButton = Instantiate(materialButtonPrefab);
        materialButton.transform.SetParent(gameObject.transform);

        RectTransform buttonRectTransform = materialButton.GetComponent<RectTransform>();

        //Anchorの設定
        buttonRectTransform.anchorMin = new Vector2(splitAnchorPoints_x[generateButtonCounter], 0);
        buttonRectTransform.anchorMax = new Vector2(splitAnchorPoints_x[generateButtonCounter+1], 1);

        //ボタン間に間隔を設ける
        buttonRectTransform.offsetMin = new Vector2(widthSpace/2,heightSpace/2);
        buttonRectTransform.offsetMax = new Vector2(-widthSpace/2,-heightSpace/2);

        //z軸方向のanchorを0に初期化しておく
        buttonRectTransform.anchoredPosition3D = Vector3.zero;

        materialButton.GetComponent<Image>().material = new Material(ibinder.Material);

        materialButton.GetComponent<Button>().onClick.AddListener(() => {

            //中間のMaterialDatabeseにjson上のマテリアルをロードする
            materialDatabaseManager.LoadMaterialDatabase(); 

            //引数で指定したマテリアルと中間のMaterialDatabeseのマテリアルが異なっていれば、引数で指定したマテリアルで初期化
            materialDatabaseManager.InitializeBlockMaterial<TEnum>(ibinder, blockSelector.NowBlockNum); 

            //生成するスライダーを現在ブロックについているマテリアルものに設定する
            sliderGenerater.SetActiveBinder(ibinder);

            //スライダーを生成する
            sliderGenerater.GenerateParameterSliders<TEnum>();

            //現在表示されているブロックに中間のMaterialDatabaseのマテリアルを割り当てる
            blockSelector.SetBlockMaterialDataToSingleBlock<TEnum>();
        });
    }
}
