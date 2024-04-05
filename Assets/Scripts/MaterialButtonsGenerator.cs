using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialLibrary;
using System;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

public class MaterialButtonsGenerator : MonoBehaviour
{
    int generateButtonCounter = 0;
    float[] splitAnchorPoints_x = Helper.CalculateSplitAnchorPoints(EnumParameterBinderManager.bindersCount);
    GameObject materialButtonPrefab;
    ParameterSliderGenerater sliderGenerater;
    void Start()
    {
        materialButtonPrefab = Resources.Load("MaterialButton") as GameObject;
        sliderGenerater = GameObject.Find("ParameterSlidersPanel").GetComponent<ParameterSliderGenerater>();
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
                genericMethod.Invoke(this, new object[] { binder.Material });
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
    void GenerateMaterialButton<TEnum>(Material material) where TEnum : Enum
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

        //
        buttonRectTransform.localScale = Vector2.one;
        buttonRectTransform.anchoredPosition3D = Vector3.zero;

        materialButton.GetComponent<Image>().material = material;

        materialButton.GetComponent<Button>().onClick.AddListener(() => { sliderGenerater.GenerateParameterSliders<TEnum>(); });
    }
}
