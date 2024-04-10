using UnityEngine;

namespace MaterialLibrary
{
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

    public class StripesMaterialEnumBinder : EnumParametersBinder<StripesMaterialProperty>
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
