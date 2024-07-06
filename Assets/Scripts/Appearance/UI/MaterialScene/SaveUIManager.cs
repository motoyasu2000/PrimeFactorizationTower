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
        if (materialDatabaseManager.MiddleMaterialDatabase == null || PlayerInfoManager.Ins.MaterialDatabase == null) return;
        BlockMaterialData middleBlockData = materialDatabaseManager.MiddleMaterialDatabase.GetBlockMaterialData(blockNum);
        BlockMaterialData loadBlockData = PlayerInfoManager.Ins.MaterialDatabase.GetBlockMaterialData(blockNum);
        if (middleBlockData == null || loadBlockData == null) return;
        if (middleBlockData.Equal(loadBlockData)) SetColor(true);
        else                                      SetColor(false);
    }

    /// <summary>
    /// SaveButtonのUIの見た目を変化させるメソッド
    /// マテリアルのパラメーターが保存されているか否かを可視化するために使用している。
    /// </summary>
    /// <param name="isComplete">
    /// セーブされた状態の見た目にする(true)か否(false)か
    /// </param>
    public void SetColor(bool isComplete)
    {
        if(isComplete) saveUIImage.color = saveColor_complete;
        else           saveUIImage.color = saveColor_incomplete;
    }
}
