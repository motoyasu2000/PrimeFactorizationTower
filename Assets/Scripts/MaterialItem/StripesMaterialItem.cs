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
        [ShaderProperty("_Timer")]
        Timer,
    }

    public class StripesMaterialItem : MaterialItem<StripesMaterialProperty>
    {
        protected override Material LoadMaterial()
        {
            return Resources.Load<Material>("MaterialsOfItem/StripesMaterial");
        }
    }
}
