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
    float[] splitAnchorPoints_x = Helper.CalculateSplitAnchorPoints(10); //Start時に反転する
    GameObject parameterSliderCellPrefab;
    void Start()
    {
        Array.Reverse(splitAnchorPoints_x);
        parameterSliderCellPrefab = Resources.Load("ParameterSliderCell") as GameObject;
        GenerateStripesCell();
    }

    void GenerateStripesCell()
    {
        GenerateParameterSliders<StripesMaterialProperty>();
    }

    //与えられた列挙型に応じて必要な数だけスライダーを生成する
    void GenerateParameterSliders<TEnum>() where TEnum : Enum
    {
        string[] parameterNames = EnumManager.GetEnumNames<TEnum>();
        foreach (string parameterName in parameterNames)
        {
            if(generateSliderCounter >= 10)
            {
                Debug.LogError("生成したいスライダーが10個以上生成できません。");
            }
            if(parameterName.Contains("Color") || parameterName.Contains("color"))
            {
                for(int i=0; i<3; i++)
                {
                    GenerateParameterSliderCell(parameterName);
                }
            }
            else
            {
                GenerateParameterSliderCell(parameterName);
            }
        }
        generateSliderCounter = 0;
    }

    //与えられた名前に応じたスライダーを生成する
    void GenerateParameterSliderCell(string parameterName)
    {
        GameObject parameterSliderCell = Instantiate(parameterSliderCellPrefab);
        parameterSliderCell.transform.SetParent(gameObject.transform);

        RectTransform sliderCellRectTransform = parameterSliderCell.GetComponent<RectTransform>();
        sliderCellRectTransform.anchorMin = new Vector2(0, splitAnchorPoints_x[generateSliderCounter + 1]);
        sliderCellRectTransform.anchorMax = new Vector2(1, splitAnchorPoints_x[generateSliderCounter]);

        sliderCellRectTransform.offsetMin = Vector2.zero;
        sliderCellRectTransform.offsetMax = Vector2.zero;

        sliderCellRectTransform.localScale = Vector2.one;
        sliderCellRectTransform.anchoredPosition3D = Vector3.zero;

        TextMeshProUGUI parameterSliderText = parameterSliderCell.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        parameterSliderText.text = parameterName;
        
        //ここでスライダーの動きを設定する・

        generateSliderCounter++;
    }
}
