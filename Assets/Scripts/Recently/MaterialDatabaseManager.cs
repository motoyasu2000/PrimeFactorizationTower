using Common;
using MaterialLibrary;
using System;
using System.Linq;
using UnityEngine;

public class MaterialDatabaseManager : MonoBehaviour
{
    static readonly float initColorValue = 0.8f;
    static readonly float initFloatValue = 10f;

    //MaterialScene上で設定中の情報を保存するマテリアルデータベース。
    MaterialDatabase middleMaterialDatabase;
    public MaterialDatabase MiddleMaterialDatabase => middleMaterialDatabase;

    private void Start()
    {
        LoadMaterialDatabase();
    }

    //引数で受け取った情報に合わせて、materialDatabaseの更新処理
    public void SetShaderParameter(int blockNum, string materialPath, IEnumParametersBinder ibinder ,ParameterData parameter)
    {
        //パラメーターを保存するためため、ブロックデータの取得処理
        BlockMaterialData blockData;
        //データベース上に既にブロックの情報があれば、そこから取得
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
        blockData.materialPath = materialPath;
        blockData.binderIndex = EnumParameterBinderManager.GetBindersIndex(ibinder);


        blockData.AddParameter(parameter);
        middleMaterialDatabase.AddBlockMaterial(blockData);
    }

    //パラメーターの文字列に対応するParameterDataを返す
    ParameterData GenerateParameterData<TEnum>(string parameterName) where TEnum:Enum
    {
        ParameterData parameterData = new ParameterData();
        parameterData.parameterEnumIndex = EnumManager.GetEnumIndexFromString<TEnum>(parameterName);
        if (parameterName.Contains("Color") || parameterName.Contains("color"))
        {
            parameterData.type = 1;
            parameterData.redValue = initColorValue;
            parameterData.greenValue = initColorValue;
            parameterData.blueValue = initColorValue;
        }
        //color以外(今のところfloatのみ)は全て1つだけ生成
        else
        {
            parameterData.type = 0;
            parameterData.floatValue = initFloatValue;
        }
        return parameterData;
    }

    //materialDatabaseの初期化 初回のみ実行されることを想定　
    public void InitializeMaterialDatabase<TEnum>(IEnumParametersBinder ibinder) where TEnum : Enum
    {
        if (middleMaterialDatabase == null) middleMaterialDatabase = PlayerInfoManager.Ins.MaterialDatabase;
        if(middleMaterialDatabase == null) new MaterialDatabase();
        foreach (var prime in GameModeManager.Ins.PrimeNumberPool)
        {
            if (middleMaterialDatabase.GetBlockMaterialData(prime) == null) InitializeBlockMaterial<TEnum>(ibinder, prime);
        }
    }

    //現在のマテリアルと押したボタンのマテリアルが一致していなければ単一のBlockMaterialを初期化し、
    //tmpMaterialDatabaseに追加 MaterialButtonをタップ時に呼ばれる
    public void InitializeBlockMaterial<TEnum>(IEnumParametersBinder ibinder, int prime) where TEnum : Enum
    {
        //もし、押されたボタンのbinderが、現在の数字のブロックのbinderと等しくなければ、もしくはそもそも現在の数字がMiddleMaterialDatabase上に存在しなければ 初期化
        if ((MiddleMaterialDatabase.GetBlockMaterialData(prime) == null) || (EnumParameterBinderManager.GetBindersIndex(ibinder) != MiddleMaterialDatabase.GetBlockMaterialData(prime).binderIndex))
        {
            BlockMaterialData materialData = new BlockMaterialData() { blockNumber = prime, binderIndex = EnumParameterBinderManager.GetBindersIndex(ibinder) };
            for (int i = 0; i < Enum.GetValues(ibinder.EnumType).Length; i++)
            {
                string parameterName = EnumManager.GetStringFromIndex<TEnum>(i);
                materialData.AddParameter(GenerateParameterData<TEnum>(parameterName));
                //Debug.Log($"MDB上の 素数: {prime}, パラメーター{parameterName}");
            }
            middleMaterialDatabase.AddBlockMaterial(materialData);
        }
    }

    public void SaveMaterialDatabase()
    {
        PlayerInfoManager.Ins.SaveMaterialDatabase(middleMaterialDatabase);
    }

    public void LoadMaterialDatabase()
    {
        middleMaterialDatabase = new MaterialDatabase(PlayerInfoManager.Ins.MaterialDatabase);
        if (middleMaterialDatabase == null || middleMaterialDatabase.blockMaterials == null || middleMaterialDatabase.blockMaterials.Count == 0) InitializeMaterialDatabase<DefaultBlocksMaterialProperty>(new DefaultMaterialEnumBinder());
    }
}
