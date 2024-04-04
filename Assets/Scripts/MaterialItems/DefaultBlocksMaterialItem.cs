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

    public class DefaultBlocksMaterialItem : MaterialItem<DefaultBlocksMaterialProperty>
    {
        protected override Material LoadMaterial()
        {
            var originalMaterial = Resources.Load<Material>("MaterialsOfItem/DefaultBlocksMaterial");
            return new Material(originalMaterial);
        }
    }
}