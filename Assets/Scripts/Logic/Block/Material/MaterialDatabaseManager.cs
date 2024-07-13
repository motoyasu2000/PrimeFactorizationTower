using Common;
using MaterialLibrary;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// 実際にブロックに設定するためのMaterialDatabaseを設定する前に、
/// マテリアル設定のためのシーン内で使う用の中間MaterialDatabaseを管理するためのクラス
/// </summary>
public class MaterialDatabaseManager : MonoBehaviour
{
    static readonly float initColorValue = 0.8f; //R,G,Bの初期化色、明るめのグレー
    static readonly float initFloatValue = 10f; //floatの初期化値

    //MaterialScene上で設定中の情報を保存するMaterialDatabase。
    MaterialDatabase middleMaterialDatabase;
    public MaterialDatabase MiddleMaterialDatabase => middleMaterialDatabase;

    private void Start()
    {
        //まず実際にブロックに設定するためのMaterialDatabaseの値を中間のMaterialDatabaseに割り当てる。
        LoadMaterialDatabase();
    }

    /// <summary>
    /// middleMaterialDatabaseの更新処理　
    /// どのブロックにどのマテリアルのどの様なパラメーターを設定するかを指定できる。
    /// </summary>
    /// <param name="blockNum">どのブロックか</param>
    /// <param name="materialPath">どのマテリアルか</param>
    /// <param name="ibinder">どのバインダーか</param>
    /// <param name="parameter">どの様なパラメーターか</param>
    public void SetShaderParameter(int blockNum, IBinder ibinder ,ParameterData parameter)
    {
        //パラメーターを保存するため、現在のブロックに割り当てられているマテリアルの取得処理
        BlockMaterialData blockData;
        //中間のデータベース上に既にブロックの情報があれば、そこから取得
        if (middleMaterialDatabase.blockMaterials.Any(b =>  b.blockNumber == blockNum))
        {
            blockData = middleMaterialDatabase.GetBlockMaterialData(blockNum);
        }
        //無ければ生成
        else
        {
            Debug.LogError("指定されたインデックスが見つかりませんでした。");
            blockData = new BlockMaterialData() { blockNumber = blockNum };
        }
        blockData.binderIndex = BinderManager.GetBindersIndex(ibinder);

        blockData.AddParameter(parameter);
        middleMaterialDatabase.AddBlockMaterial(blockData);
    }

    /// <summary>
    /// 文字列で与えられたパラメーターを受け取り、その文字列に応じて、初期化されたParameterDataを返す
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型のパラメーターを初期化するか</typeparam>
    /// <param name="parameterName">どのパラメーターか</param>
    /// <returns>初期化されたParameterData</returns>
    ParameterData InitializeParameterData<TEnum>(string parameterName) where TEnum:Enum
    {
        ParameterData parameterData = new ParameterData();
        parameterData.parameterEnumIndex = EnumManager.GetEnumIndexFromString<TEnum>(parameterName);
        //色のパラメーターであれば、typeと各色を初期化して返す
        if (parameterName.Contains("Color") || parameterName.Contains("color"))
        {
            parameterData.type = ParameterData.PropertyType.Color;
            parameterData.redValue = initColorValue;
            parameterData.greenValue = initColorValue;
            parameterData.blueValue = initColorValue;
        }
        //color以外(今のところfloatのみ)はtypeとfloat値を初期化して返す
        else
        {
            parameterData.type = ParameterData.PropertyType.Float;
            parameterData.floatValue = initFloatValue;
        }
        return parameterData;
    }

    /// <summary>
    /// 現在のマテリアルと押したボタンのマテリアルが一致していなければ
    /// 中間のマテリアルデータベースの特定の数値のマテリアルを初期化する。
    /// MaterialButtonのタップ時に呼ばれる
    /// </summary>
    /// <typeparam name="TEnum">どの列挙型のパラメーターを初期化するか</typeparam>
    /// <param name="ibinder">どのバインダーか</param>
    /// <param name="prime">どのブロックか</param>

    public void InitializeBlockMaterial<TEnum>(IBinder ibinder, int prime) where TEnum : Enum
    {
        //もし、押されたボタンのbinderが、現在の数字のブロックのbinderと等しくなければ、
        //もしくはそもそも現在の数字がMiddleMaterialDatabase上に存在しなければ初期化して中間のMaterialDatabaseを更新
        BlockMaterialData middleMaterialData = MiddleMaterialDatabase.GetBlockMaterialData(prime);
        if ((middleMaterialData == null) ||
            (BinderManager.GetBindersIndex(ibinder) != middleMaterialData.binderIndex))
        {
            BlockMaterialData materialData = new BlockMaterialData() { blockNumber = prime, binderIndex = BinderManager.GetBindersIndex(ibinder) };
            for (int i = 0; i < Enum.GetValues(ibinder.EnumType).Length; i++)
            {
                string parameterName = EnumManager.GetStringFromIndex<TEnum>(i);
                materialData.AddParameter(InitializeParameterData<TEnum>(parameterName));
                //Debug.Log($"MDB上の 素数: {prime}, パラメーター{parameterName}");
            }
            middleMaterialDatabase.AddBlockMaterial(materialData);
        }
    }

    /// <summary>
    /// ゲーム中にブロックが読み込むMaterialDatabeseの情報を中間のMaterialDatabeseにロードする。無ければ初期化を行う。
    /// </summary>
    public void LoadMaterialDatabase()
    {
        if (PlayerInfoManager.Ins.MaterialDatabase != null)
            middleMaterialDatabase = new MaterialDatabase(PlayerInfoManager.Ins.MaterialDatabase);

        else
            InitializeMaterialDatabase();
    }

    /// <summary>
    /// middleMaterialDatabaseの初期化し、保存も行う 初回のみ実行されることを想定　
    /// </summary>
    public void InitializeMaterialDatabase()
    {
        middleMaterialDatabase = new MaterialDatabase();

        //すべてのブロックをデフォルトのマテリアルで初期化
        foreach (var prime in GameModeManager.Ins.PrimeNumberPool)
        {
            InitializeBlockMaterial<DefaultBlocksMaterialProperty>(BinderManager.Binders[0], prime);
        }

        SaveMaterialDatabase();
    }

    //中間のMaterialDatabaseの情報をjson形式で保存し、いつでもロードできるようにする。
    public void SaveMaterialDatabase()
    {
        PlayerInfoManager.Ins.SaveMaterialDatabase(middleMaterialDatabase);
    }
}
