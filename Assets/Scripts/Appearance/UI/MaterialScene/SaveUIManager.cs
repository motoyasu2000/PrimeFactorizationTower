using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MaterialScene内のセーブボタンのUIを操作するクラス
/// </summary>
public class SaveUIManager : MonoBehaviour
{
    static readonly Color saveColor_complete = Color.green;
    static readonly Color saveColor_incomplete = Color.red;
    Image saveUIImage;
    BlockSelector blockSelector;
    MaterialDatabaseManager materialDatabaseManager;

    void Start()
    {
        saveUIImage = GetComponent<Image>();
        blockSelector = FindObjectOfType<BlockSelector>();
        materialDatabaseManager = FindObjectOfType<MaterialDatabaseManager>();
    }

    private void Update()
    {
        //現在表示しているマテリアルのデータと、現在保存されているマテリアルのデータを比較して一致しているかどうかをチェック
        int blockNum = blockSelector.NowBlockNum;
        if (materialDatabaseManager.MiddleMaterialDatabase == null || PlayerInfoManager.Ins.MaterialDatabase == null) {
            ChangeUnSavedColor();
            return;
        }
        BlockMaterialData middleBlockData = materialDatabaseManager.MiddleMaterialDatabase.GetBlockMaterialData(blockNum);
        BlockMaterialData loadBlockData = PlayerInfoManager.Ins.MaterialDatabase.GetBlockMaterialData(blockNum);
        if (middleBlockData == null || loadBlockData == null)
        {
            ChangeUnSavedColor();
            return;
        }
        if (middleBlockData.Equal(loadBlockData)) ChangeSavedColor();
        else                                      ChangeUnSavedColor();
    }

    /// <summary>
    /// SaveButtonのUIの見た目を変化させるメソッド
    /// マテリアルのパラメーターの設定が保存されていることを色で可視化する
    /// </param>
    public void ChangeSavedColor()
    {
        saveUIImage.color = saveColor_complete;
    }

    /// <summary>
    /// SaveButtonのUIの見た目を変化させるメソッド
    /// マテリアルのパラメーターの設定がまだ保存されていないことを色で可視化する
    /// </param>
    public void ChangeUnSavedColor()
    {
        saveUIImage.color = saveColor_incomplete;
    }
}
