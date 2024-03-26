using MaterialLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BlockとMaterialItemの関係を管理するクラス。また、実際にゲームオブジェクトにマテリアルを割り当てる。
public class BlockMaterialItemManager : MonoBehaviour
{
    //BlockとMaterialItemのマッピング
    private Dictionary<GameObject, IMaterialItem> blockMaterialItemDict = new Dictionary<GameObject, IMaterialItem>();
    AllBlocksManager allBlocksManager;

    //本来は外からマッピングを行うし、パラメーターの設定も外から行うが、今回はテストとしてAwakeで設定してみる
    private void Awake()
    {
        allBlocksManager = GameObject.Find("AllBlocksManager").GetComponent<AllBlocksManager>();
        IMaterialItem stripesMaterialItem = new StripesMaterialItem();
        stripesMaterialItem.SetPropertyColor(StripesMaterialProperty.MainColor, Color.black);
        ApplyMaterial(allBlocksManager.BlocksDict[2], stripesMaterialItem);
    }

    //MaterialItemをBlockに割り当てる
    private void SetMaterialItem(GameObject block, IMaterialItem materialItem)
    {
        if (blockMaterialItemDict.ContainsKey(block))
        {
            Debug.Log($"{block.name}のマテリアルを{blockMaterialItemDict[block]}から{materialItem}に変更します。");
        }
        blockMaterialItemDict[block] = materialItem;
    }

    //BlockのMaterialItemを取得する
    public IMaterialItem GetMaterialItem(GameObject block)
    {
        if (blockMaterialItemDict.TryGetValue(block, out IMaterialItem materialItem))
        {
            return materialItem;
        }
        else
        {
            Debug.LogError("指定されたGameObjectにはMaterialItemが割り当てられていません。");
            return null;
        }
    }

    //実際にBlockにマテリアルを割り当てる。
    public void ApplyMaterial(GameObject block, IMaterialItem materialItem)
    {
        SetMaterialItem(block, materialItem);

        var renderer = block.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.material = new Material(materialItem.Material);
        }
        else
        {
            Debug.LogError("指定されたGameObjectにRendererコンポーネントが見つかりません。");
        }
    }
}
