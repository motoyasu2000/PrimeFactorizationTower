using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UIElements;
using TMPro;
using System.Linq;
using MaterialLibrary;

//マテリアルのパラメーターを調整するスライダーを生成するクラス
public class ParameterSliderGenerater : MonoBehaviour
{
    int generateSliderCounter = 0;
    float[] splitAnchorPoints_y = Helper.CalculateSplitAnchorPoints(10); //Start時に反転する
    GameObject parameterSliderCellPrefab;
   
    void Start()
    {
        Array.Reverse(splitAnchorPoints_y);
        parameterSliderCellPrefab = Resources.Load("ParameterSliderCell") as GameObject;
    }

    //与えられた列挙型に応じて必要な数だけスライダーを生成する
    public void GenerateParameterSliders<TEnum>() where TEnum : Enum
    {
        InitializeSlider();

        string[] parameterNames = EnumManager.GetEnumNames<TEnum>();
        foreach (string parameterName in parameterNames)
        {
            if(generateSliderCounter >= 10)
            {
                Debug.LogError("生成したいスライダーが10個以上生成できません。");
            }
            //colorのパラメーターだった場合はr,g,b文3つのスライダーを生成する
            if(parameterName.Contains("Color") || parameterName.Contains("color"))
            {
                for(int i=0; i<3; i++)
                {
                    GenerateParameterSliderCell(parameterName);
                }
            }
            //color以外(今のところfloatのみ)は全て1つだけ生成
            else
            {
                GenerateParameterSliderCell(parameterName);
            }
        }
        generateSliderCounter = 0; //GenerateParameterSliderCell関数の呼び出しごとにgenerateSliderCounterが増えるのでここで初期化
    }

    //与えられた名前に応じたスライダーを生成する
    void GenerateParameterSliderCell(string parameterName)
    {
        GameObject parameterSliderCell = Instantiate(parameterSliderCellPrefab);
        parameterSliderCell.transform.SetParent(gameObject.transform);

        RectTransform sliderCellRectTransform = parameterSliderCell.GetComponent<RectTransform>();
        sliderCellRectTransform.anchorMin = new Vector2(0, splitAnchorPoints_y[generateSliderCounter + 1]);
        sliderCellRectTransform.anchorMax = new Vector2(1, splitAnchorPoints_y[generateSliderCounter]);

        sliderCellRectTransform.offsetMin = Vector2.zero;
        sliderCellRectTransform.offsetMax = Vector2.zero;

        sliderCellRectTransform.localScale = Vector2.one;
        sliderCellRectTransform.anchoredPosition3D = Vector3.zero;

        TextMeshProUGUI parameterSliderText = parameterSliderCell.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        parameterSliderText.text = parameterName;
        
        //ここでスライダーの動きを設定する・

        generateSliderCounter++;
    }

    //スライダーを生成する際、あらかじめスライダーがあったら消去する
    public void InitializeSlider()
    {
        foreach(Transform slider in gameObject.transform)
        {
            Destroy(slider.gameObject);
        }
    }
}
