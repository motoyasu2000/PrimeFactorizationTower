using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using MaterialLibrary;
using UnityEditor.Rendering;

//マテリアルのパラメーターを調整するスライダーを生成するクラス
public class ParameterSliderGenerater : MonoBehaviour
{
    static readonly float floatSliderScale = 50; //スライダーは0~1までの値をとるので、それをスケーリングするための定数
    static readonly float floatSliderEpsilon = 0.01f; //スライダーで定めた値はシェーダー内で割り算の式に使う可能性がある。0除算が発生しないようにするために加算する項
    static readonly float minusAlpha = 0.8f; //スライダーセルのalphaに減算するための定数 1-minusAlphaがスライダーセルのアルファ値として設定される。
    int generateSliderCounter = 0;
    float[] splitAnchorPoints_y = Helper.CalculateSplitAnchorPoints(10); //Start時に反転する
    GameObject parameterSliderCellPrefab;
    IEnumParametersBinder activeBinder;
    MaterialDatabaseManager materialDatabaseManager;
    BlockSelector blockMaterialSelector;

    //ParameterDataのvalue達と同じ並び順だと好ましい
    enum SliderType
    {
        floatValue,
        RedValue,
        GreenValue, 
        BlueValue,
    }

    void Awake()
    {
        Array.Reverse(splitAnchorPoints_y);
        parameterSliderCellPrefab = Resources.Load("ParameterSliderCell") as GameObject;
        materialDatabaseManager = GameObject.Find("MaterialDatabaseManager").GetComponent<MaterialDatabaseManager>();
        blockMaterialSelector = GameObject.Find("BlockMaterialSelector").GetComponent<BlockSelector>();
    }

    //与えられた列挙型に応じて必要な数だけスライダーを生成する
    public void GenerateParameterSliders<TEnum>() where TEnum : Enum
    {
        InitializeSlider();

        string[] parameterNames = EnumManager.GetEnumNames<TEnum>();
        foreach (string parameterName in parameterNames)
        {
            Debug.Log(parameterName);
            if(generateSliderCounter >= 10)
            {
                Debug.LogError("生成したいスライダーが10個以上生成できません。");
            }
            //colorのパラメーターだった場合はr,g,b文3つのスライダーを生成する
            if(parameterName.Contains("Color") || parameterName.Contains("color"))
            {
                GenerateParameterSliderCell<TEnum>(parameterName, SliderType.RedValue);
                GenerateParameterSliderCell<TEnum>(parameterName, SliderType.GreenValue);
                GenerateParameterSliderCell<TEnum>(parameterName, SliderType.BlueValue);
            }
            //color以外(今のところfloatのみ)は全て1つだけ生成
            else
            {
                GenerateParameterSliderCell<TEnum>(parameterName, SliderType.floatValue);
            }
        }
        generateSliderCounter = 0; //GenerateParameterSliderCell関数の呼び出しごとにgenerateSliderCounterが増えるのでここで初期化
    }

    //与えられた名前に応じたスライダーを生成する
    void GenerateParameterSliderCell<TEnum>(string parameterName, SliderType sliderType) where TEnum : Enum
    {
        //sliderCellの生成と親の設定
        GameObject parameterSliderCell = Instantiate(parameterSliderCellPrefab);
        parameterSliderCell.transform.SetParent(gameObject.transform);

        //Anchorの設定
        RectTransform sliderCellRectTransform = parameterSliderCell.GetComponent<RectTransform>();
        sliderCellRectTransform.anchorMin = new Vector2(0, splitAnchorPoints_y[generateSliderCounter + 1]);
        sliderCellRectTransform.anchorMax = new Vector2(1, splitAnchorPoints_y[generateSliderCounter]);

        //Anchorとの差を0に
        sliderCellRectTransform.offsetMin = Vector2.zero;
        sliderCellRectTransform.offsetMax = Vector2.zero;

        //スケールやz座標も0に初期化
        sliderCellRectTransform.localScale = Vector2.one;
        sliderCellRectTransform.anchoredPosition3D = Vector3.zero;

        //何を調整するスライダーなのかを説明するテキストの設定
        TextMeshProUGUI parameterSliderText = parameterSliderCell.GetComponentInChildren<TextMeshProUGUI>();
        parameterSliderText.text = parameterName;

        //スライダーの設定
        Slider parameterSlider = parameterSliderCell.GetComponentInChildren<Slider>();
        int blockNum = blockMaterialSelector.NowBlockNum;
        string materialPathAndName = activeBinder.MaterialPathAndName;

        //今のスライダーがFloatに対するものなのか、colorに対するものなのか
        int parameterType;
        if (sliderType == SliderType.floatValue) parameterType = 0;
        else if (sliderType == SliderType.RedValue || sliderType == SliderType.GreenValue || sliderType == SliderType.BlueValue) parameterType = 1;
        else { parameterType = -1; Debug.LogError($"SliderTypeが予期せぬ値になっています。: {sliderType}"); }

        //パラメーターデータの生成
        ParameterData parameterData = GetNowStateParameterData<TEnum>(parameterName); //初期化
        parameterData.type = parameterType; //floatかColorか、typeの設定

        InitializeSliderValue(sliderType, parameterData, parameterSlider);
        SetSliderCellColor(sliderType, parameterSliderCell);

        //スライダーを動かしたの時のイベントの追加
        parameterSlider.onValueChanged.AddListener((v) => { SetParameterData(v); });
        parameterSlider.onValueChanged.AddListener((v) => { materialDatabaseManager.SetShaderParameter(blockMaterialSelector.NowBlockNum, materialPathAndName,activeBinder ,parameterData); });
        parameterSlider.onValueChanged.AddListener((v) => { blockMaterialSelector.SetBlockMaterialDataToSingleBlock<TEnum>(); });


        generateSliderCounter++;

        //引数で単一のfloatの変数だけを持つように、GenerateParameterSliderCellメソッド内部に定義。
        void SetParameterData(float value)
        {
            //スライダーによる設定
            if (sliderType == SliderType.floatValue) parameterData.floatValue = floatSliderScale*value + floatSliderEpsilon;
            else if (sliderType == SliderType.RedValue) parameterData.redValue = value;
            else if (sliderType == SliderType.GreenValue) parameterData.greenValue = value;
            else if (sliderType == SliderType.BlueValue) parameterData.blueValue = value;
            else Debug.LogError("予期せぬSlidertypeが呼ばれました。");
        }
    }

    //スライダーのバーの値を現在のマテリアルデータベースの情報で初期化
    void InitializeSliderValue(SliderType sliderType, ParameterData parameterData, Slider slider)
    {
        //スライダーによる設定
        if (sliderType == SliderType.floatValue) slider.value = (parameterData.floatValue-floatSliderEpsilon)/floatSliderScale;
        else if (sliderType == SliderType.RedValue) slider.value = parameterData.redValue;
        else if (sliderType == SliderType.GreenValue) slider.value = parameterData.greenValue;
        else if (sliderType == SliderType.BlueValue) slider.value = parameterData.blueValue;
        else Debug.LogError("予期せぬSlidertypeが呼ばれました。");
    }

    void SetSliderCellColor(SliderType sliderType, GameObject parameterSliderCell)
    {
        Image cellImage = parameterSliderCell.GetComponent<Image>();
        //スライダーによる設定
        if (sliderType == SliderType.floatValue)
        {
            cellImage.color = Color.black;
            cellImage.color = cellImage.color - new Color(0, 0, 0, minusAlpha);
        }
        else if (sliderType == SliderType.RedValue)
        {
            cellImage.color= Color.red;
            cellImage.color = cellImage.color - new Color(0, 0, 0, minusAlpha);
        }
        else if (sliderType == SliderType.GreenValue)
        {
            cellImage.color= Color.green;
            cellImage.color = cellImage.color - new Color(0, 0, 0, minusAlpha);
        }
        else if (sliderType == SliderType.BlueValue)
        {
            cellImage.color = Color.blue;
            cellImage.color = cellImage.color - new Color(0, 0, 0, minusAlpha);
        }
        else Debug.LogError("予期せぬSlidertypeが呼ばれました。");
    }

    //スライダーを生成する際、あらかじめスライダーがあったら消去する
    public void InitializeSlider()
    {
        foreach(Transform slider in gameObject.transform)
        {
            Destroy(slider.gameObject);
        }
    }
    //現在どのマテリアルがアクティブか設定する
    public void SetActiveBinder(IEnumParametersBinder binder)
    {
        activeBinder = binder;
    }

    //現在のスライダー、現在のブロックに合ったparametaerDataをmaterialDatabaseから取得する
    ParameterData GetNowStateParameterData<TEnum>(string parameterName) where TEnum : Enum
    {
        MaterialDatabase materialDatabase = materialDatabaseManager.MiddleMaterialDatabase;
        int parameterEnumindex = EnumManager.GetEnumIndexFromString<TEnum>(parameterName);
        //Debug.Log(parameterEnumindex);
        //指定したインデックスが含まれていれば
        if(materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum).GetParameter(parameterEnumindex)!=null)
        {

            return materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum).GetParameter(parameterEnumindex);
        }
        else
        {
            if (materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum) == null) Debug.Log("blockdata null");
            if (materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum).GetParameter(parameterEnumindex) == null) Debug.Log("parameter null");
            Debug.LogError("指定したインデックスが存在しませんでした。");
            return new ParameterData();
        }
    }


}
