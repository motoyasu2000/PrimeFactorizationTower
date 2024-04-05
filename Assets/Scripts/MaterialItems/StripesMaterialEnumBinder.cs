using Common;
using MaterialLibrary;
using System.Collections;
using System.Collections.Generic;
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
        protected override Material LoadMaterial()
        {
            var loadedMaterial = Resources.Load<Material>(MaterialPasses.GetStripesMaterialName());
            if (loadedMaterial == null) Debug.LogError("マテリアルのロードに失敗しました。");
            return new Material(loadedMaterial);
        }
    }
}
