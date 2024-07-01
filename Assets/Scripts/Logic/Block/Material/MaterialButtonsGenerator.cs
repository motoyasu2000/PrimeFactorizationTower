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
            materialDatabaseManager.LoadMaterialDatabase(); //中間のMaterialDatabeseにjson上のマテリアルをロードする
            sliderGenerater.SetActiveBinder(ibinder);
            materialDatabaseManager.InitializeBlockMaterial<TEnum>(ibinder, blockSelector.NowBlockNum); //TmpMaterialDatabeseの現在のブロック部分を選択したマテリアルのものに変更する;
            sliderGenerater.GenerateParameterSliders<TEnum>();
            blockSelector.SetBlockMaterialDataToSingleBlock<TEnum>();
            saveUIManager.ChangeColor(true);
        });
    }
}
