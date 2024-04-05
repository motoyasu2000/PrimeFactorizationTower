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
            var loadedMaterial = Resources.Load<Material>(MaterialPasses.GetDefaultBlocksMaterialName());
            if (loadedMaterial == null) Debug.LogError("�}�e���A���̃��[�h�Ɏ��s���܂����B");
            return new Material(loadedMaterial);
        }
    }
}