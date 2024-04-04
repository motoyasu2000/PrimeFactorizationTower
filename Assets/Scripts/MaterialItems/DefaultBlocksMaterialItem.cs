using Common;
using MaterialLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLibrary
{
    public enum DefaultBlocksMaterialProperty
    {
        [ShaderProperty("_Color")]
        MainColor,
    }

    public class DefaultMaterialEnumBinder : EnumParametersBinder<DefaultBlocksMaterialProperty>
    {
        protected override Material LoadMaterial()
        {
            
            var originalMaterial = Resources.Load<Material>($"{GameInfo.MaterialsOfItemPass}/{"DafaultBlocksMaterial"}");
            return new Material(originalMaterial);
        }
    }
}