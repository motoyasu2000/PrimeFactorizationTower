using UnityEngine;

namespace MaterialLibrary
{
    /// <summary>
    /// しましまのマテリアルのプロパティ。
    /// しましまを構成する2色としましまの間隔、しましまの流れる速度を指定できる。
    /// </summary>
    public enum StripesMaterialProperty
    {
        [ShaderProperty("_Color")]
        MainColor,
        [ShaderProperty("_AnotherColor")]
        AnotherColor,
        [ShaderProperty("_Space")]
        Space,
        [ShaderProperty("_Speed")]
        Speed,
    }

    /// <summary>
    /// しましまのマテリアルのBinder
    /// </summary>
    public class StripesMaterialBinder : AbstBinder<StripesMaterialProperty>
    {
        public override string MaterialPathAndName => MaterialPasses.GetStripesMaterialName();
        protected override Material LoadMaterial()
        {
            var loadedMaterial = Resources.Load<Material>(MaterialPathAndName);
            if (loadedMaterial == null) Debug.LogError("マテリアルのロードに失敗しました。");
            return new Material(loadedMaterial);
        }
    }
}
