using UnityEngine;

namespace MaterialLibrary
{
    /// <summary>
    /// 初期のマテリアルのプロパティ
    /// 単色
    /// </summary>
    public enum DefaultBlocksMaterialProperty
    {
        [ShaderProperty("_Color")]
        MainColor,
    }

    /// <summary>
    /// 初期のマテリアルのBinder
    /// 単色
    /// </summary>
    public class DefaultMaterialBinder : AbstBinder<DefaultBlocksMaterialProperty>
    {
        public override string MaterialPathAndName => MaterialPasses.GetDefaultBlocksMaterialName();
        protected override Material LoadMaterial()
        {
            var loadedMaterial = Resources.Load<Material>(MaterialPathAndName);
            if (loadedMaterial == null) Debug.LogError("マテリアルのロードに失敗しました。");
            return new Material(loadedMaterial);
        }
    }
}