using System;
using UnityEngine;
using Common;
using UnityEngine.UI;
using TMPro;
using MaterialLibrary;

/// <summary>
/// MaterialScene内で、マテリアルのパラメーターを調整するスライダーを生成するクラス
/// </summary>
public class ParameterSliderGenerator : MonoBehaviour
{
    static readonly int maxSliders = 10; //生成できるスライダーの上限
    static readonly float floatSliderScale = 50; //スライダーは0~1までの値をとるので、それをスケーリングするための定数(float型のsliderに使用)
    static readonly float floatSliderEpsilon = 0.01f; //スライダーで定めた値はシェーダー内で割り算の式に使う可能性がある。0除算が発生しないようにするために加算する項
    static readonly float sliderCellAlphaOffset = 0.8f; //スライダーセルのalphaに減算するための定数 1-minusAlphaがスライダーセルのアルファ値として設定される。
    
    int generateSliderCounter = 0; //今上から何個目のスライダーを生成しているか
    float[] splitAnchorPoints_y; //表示するスライダーを分割するyのビューポート座標
    GameObject parameterSliderCellPrefab;
    IBinder activeBinder;
    MaterialDatabaseManager materialDatabaseManager;
    BlockSelector blockMaterialSelector;

    //ParameterDataのvalue達と同じ並び順であるべき
    enum SliderType
    {
        floatValue,
        RedValue,
        GreenValue, 
        BlueValue,
    }

    void Awake()
    {
        splitAnchorPoints_y = Helper.CalculateSplitAnchorPoints(maxSliders);
        Array.Reverse(splitAnchorPoints_y);
        parameterSliderCellPrefab = Resources.Load("ParameterSliderCell") as GameObject;
        materialDatabaseManager = GameObject.Find("MaterialDatabaseManager").GetComponent<MaterialDatabaseManager>();
        blockMaterialSelector = GameObject.Find("BlockMaterialSelector").GetComponent<BlockSelector>();
    }

    /// <summary>
    /// 与えられた列挙型(シェーダ)に応じて必要な数だけスライダーを生成する
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型(シェーダー)か</typeparam>
    public void GenerateParameterSliders<TEnum>() where TEnum : Enum
    {
        DestroyAllSlider();

        string[] parameterNames = EnumManager.GetEnumNames<TEnum>();
        foreach (string parameterName in parameterNames)
        {
            if(generateSliderCounter >= maxSliders)
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

    /// <summary>
    /// 与えられた名前、タイプに応じたスライダーを生成する
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型(シェーダー)か</typeparam>
    /// <param name="parameterName">パラメーターの名前</param>
    /// <param name="sliderType">スライダーのタイプ(r,g,b,float)</param>
    void GenerateParameterSliderCell<TEnum>(string parameterName, SliderType sliderType) where TEnum : Enum
    {
        GameObject parameterSliderCell = GenerateSliderCell(parameterName);
        SetupSlider<TEnum>(parameterName, sliderType, parameterSliderCell);
        generateSliderCounter++;
    }

    /// <summary>
    /// スライダーセルを生成
    /// </summary>
    /// <param name="parameterName">パラメーターの名前</param>
    /// <returns>生成したスライダーセル</returns>
    GameObject GenerateSliderCell(string parameterName)
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

        return parameterSliderCell;
    }

    /// <summary>
    /// スライダーセルの子要素にあるスライダーや、そのスライダーに対応するParameterDataの設定を行う。
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型(シェーダー)か</typeparam>
    /// <param name="parameterName">パラメーターの名前</param>
    /// <param name="sliderType">スライダーのタイプ(floatやr,g,b)</param>
    /// <param name="parameterSliderCell">どのスライダーセルのスライダーの設定を行うか</param>
    void SetupSlider<TEnum>(string parameterName, SliderType sliderType, GameObject parameterSliderCell) where TEnum : Enum
    {
        //スライダーの設定
        Slider parameterSlider = parameterSliderCell.GetComponentInChildren<Slider>();

        //パラメーターデータの設定(スライダーではこのparameterDataの設定を行う)
        ParameterData parameterData = GetNowStateParameterData<TEnum>(parameterName); //初期化
        if (sliderType == SliderType.floatValue) parameterData.type = ParameterData.PropertyType.Float;
        else if (sliderType == SliderType.RedValue || sliderType == SliderType.GreenValue || sliderType == SliderType.BlueValue) parameterData.type = ParameterData.PropertyType.Color;
        else { parameterData.type = ParameterData.PropertyType.Invalid; Debug.LogError($"SliderTypeが予期せぬ値になっています。: {sliderType}"); }

        //生成したスライダーの初期値や色を設定し、イベントを追加する
        InitializeSliderValue(sliderType, parameterData, parameterSlider);
        SetupSliderCellColor(sliderType, parameterSliderCell);
        SetupSliderEvent<TEnum>(sliderType, parameterData, parameterSlider);
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

    //sliderCellの色をsliderTypeに合わせて設定する
    void SetupSliderCellColor(SliderType sliderType, GameObject parameterSliderCell)
    {
        Image cellImage = parameterSliderCell.GetComponent<Image>();
        //スライダーによる設定
        if (sliderType == SliderType.floatValue) cellImage.color = Color.black;
        else if (sliderType == SliderType.RedValue) cellImage.color = Color.red;
        else if (sliderType == SliderType.GreenValue) cellImage.color = Color.green;
        else if (sliderType == SliderType.BlueValue) cellImage.color = Color.blue;
        else Debug.LogError("予期せぬSlidertypeが呼ばれました。");
        cellImage.color = cellImage.color - new Color(0, 0, 0, sliderCellAlphaOffset);
    }

    /// <summary>
    /// 中間のマテリアルデータのparameterDataを更新して、現在表示されているブロックに一時的に反映するイベントをスライダーに追加
    /// </summary>
    /// <typeparam name="TEnum">そのスライダーはどの列挙型(シェーダー)のものか</typeparam>
    /// <param name="sliderType"><param name="sliderType">スライダーのタイプ(floatやr,g,b)</param>
    /// <param name="parameterData">のちにデータとして保存するパラメーターのデータ</param>
    /// <param name="parameterSlider">スライダー</param>
    void SetupSliderEvent<TEnum>(SliderType sliderType, ParameterData parameterData, Slider parameterSlider) where TEnum : Enum
    {
        string materialPathAndName = activeBinder.MaterialPathAndName;

        //スライダーを動かしたの時のイベントの追加
        //中間のマテリアルデータのparameterDataを更新して、現在表示されているブロックに一時的に反映する
        parameterSlider.onValueChanged.AddListener((v) => { UpdateParameterData(v,sliderType,parameterData); });
        parameterSlider.onValueChanged.AddListener((v) => { materialDatabaseManager.SetShaderParameter(blockMaterialSelector.NowBlockNum, materialPathAndName, activeBinder, parameterData); });
        parameterSlider.onValueChanged.AddListener((v) => { blockMaterialSelector.SetBlockMaterialDataToSingleBlock<TEnum>(); });

    }

    //引数で受け取ったparameterDataをvalueに応じて更新する。(スライダーによって操作される。)
    void UpdateParameterData(float value,SliderType sliderType, ParameterData parameterData)
    {
        //スライダーによる設定
        if (sliderType == SliderType.floatValue) parameterData.floatValue = floatSliderScale * value + floatSliderEpsilon;
        else if (sliderType == SliderType.RedValue) parameterData.redValue = value;
        else if (sliderType == SliderType.GreenValue) parameterData.greenValue = value;
        else if (sliderType == SliderType.BlueValue) parameterData.blueValue = value;
        else Debug.LogError("予期せぬSlidertypeが呼ばれました。");
    }

    //選択中のブロックや、ブロックに割り当てるマテリアルを切り替えて、新たなスライダーを生成するたびによばれる。
    public void DestroyAllSlider()
    {
        foreach(Transform slider in gameObject.transform)
        {
            Destroy(slider.gameObject);
        }
    }

    /// <summary>
    /// activeBinderが何であるのかを設定する
    /// 現在選択中のブロックのbinderをactiveBinderにするために使用
    /// </summary>
    /// <param name="binder">アクティブなマテリアルに対応するBinder</param>
    public void SetActiveBinder(IBinder binder)
    {
        activeBinder = binder;
    }

    /// <summary>
    /// 現在のスライダー、現在のブロックに合ったparametaerDataを中間のmaterialDatabaseから取得する
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型(シェーダーか)</typeparam>
    /// <param name="parameterName">パラメーターの名前</param>
    /// <returns>指定したパラメーターのParameterData</returns>
    ParameterData GetNowStateParameterData<TEnum>(string parameterName) where TEnum : Enum
    {
        MaterialDatabase materialDatabase = materialDatabaseManager.MiddleMaterialDatabase;
        int parameterEnumindex = EnumManager.GetEnumIndexFromString<TEnum>(parameterName);

        //指定したインデックスが中間のmaterialDatabaseに含まれていれば
        if (materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum).GetParameter(parameterEnumindex)!=null)
        {
            return materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum).GetParameter(parameterEnumindex);
        }
        else
        {
            if (materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum) == null) Debug.Log("blockdataがnullです");
            if (materialDatabase.GetBlockMaterialData(blockMaterialSelector.NowBlockNum).GetParameter(parameterEnumindex) == null) Debug.Log("parameterがnullです");
            Debug.LogError("指定したインデックスが存在しませんでした。");
            return new ParameterData();
        }
    }
}
