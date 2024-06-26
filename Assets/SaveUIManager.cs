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
        int blockNum = blockSelector.NowBlockNum;
        BlockMaterialData middleBlockData = materialDatabaseManager.MiddleMaterialDatabase.GetBlockMaterialData(blockNum);
        BlockMaterialData loadBlockData = PlayerInfoManager.Ins.MaterialDatabase.GetBlockMaterialData(blockNum);
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
        else                saveUIImage.color = saveColor_incomplete;
    }
}
