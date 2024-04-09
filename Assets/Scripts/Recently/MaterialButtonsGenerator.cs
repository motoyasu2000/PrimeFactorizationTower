using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialLibrary;
using System;
using System.Reflection;
using TMPro;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

//MaterialSceneでマテリアルを選択するためのボタンを生成するクラス
public class MaterialButtonsGenerator : MonoBehaviour
{
    int generateButtonCounter = 0;
    float[] splitAnchorPoints_x = Helper.CalculateSplitAnchorPoints(EnumParameterBinderManager.BindersCount);
    GameObject materialButtonPrefab;
    ParameterSliderGenerator sliderGenerater;
    MaterialDatabaseManager materialDatabaseManager;
    BlockSelector blockSelector;
    void Start()
    {
        materialButtonPrefab = Resources.Load("MaterialButton") as GameObject;
        sliderGenerater = GameObject.Find("ParameterSlidersPanel").GetComponent<ParameterSliderGenerator>();
        materialDatabaseManager = GameObject.Find("MaterialDatabaseManager").GetComponent<MaterialDatabaseManager>();
        blockSelector = GameObject.Find("BlockMaterialSelector").GetComponent<BlockSelector>();
        GenerateMaterialButtons();
    }


    //EnumParameterBinderManager.Bindersに定義されている全てのマテリアルを操作するためのボタンを生成する。リフレクションを使ってGenerateMaterialBlockのジェネリックのenumを指定できるようにしている。
    void GenerateMaterialButtons()
    {
        foreach (var binder in EnumParameterBinderManager.Binders)
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
    void GenerateMaterialButton<TEnum>(IEnumParametersBinder ibinder) where TEnum : Enum
    {
        GameObject materialButton = Instantiate(materialButtonPrefab);
        materialButton.transform.SetParent(gameObject.transform);

        RectTransform buttonRectTransform = materialButton.GetComponent<RectTransform>();

        //Anchorの設定
        buttonRectTransform.anchorMin = new Vector2(splitAnchorPoints_x[generateButtonCounter], 0);
        buttonRectTransform.anchorMax = new Vector2(splitAnchorPoints_x[generateButtonCounter+1], 1);

        //Anchorとの差を0に
        buttonRectTransform.offsetMin = Vector2.zero;
        buttonRectTransform.offsetMax = Vector2.zero;

        //スケールやz軸方向のanchorも0に初期化しておく
        buttonRectTransform.localScale = Vector2.one;
        buttonRectTransform.anchoredPosition3D = Vector3.zero;

        materialButton.GetComponent<Image>().material = new Material(ibinder.Material);

        materialButton.GetComponent<Button>().onClick.AddListener(() => {
            materialDatabaseManager.LoadMaterialDatabase(); //TmpMaterialDatabeseの初期化
            sliderGenerater.SetActiveBinder(ibinder);
            materialDatabaseManager.InitializeBlockMaterial<TEnum>(ibinder, blockSelector.NowBlockNum); //TmpMaterialDatabeseの現在のブロック部分を選択したマテリアルのものに変更する;
            sliderGenerater.GenerateParameterSliders<TEnum>();
            blockSelector.SetBlockMaterialDataToSingleBlock<TEnum>();
        });
    }
}
