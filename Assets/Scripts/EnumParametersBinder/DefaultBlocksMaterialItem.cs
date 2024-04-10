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
        public override string MaterialPathAndName => MaterialPasses.GetDefaultBlocksMaterialName();
        protected override Material LoadMaterial()
        {
            var loadedMaterial = Resources.Load<Material>(MaterialPathAndName);
            if (loadedMaterial == null) Debug.LogError("�}�e���A���̃��[�h�Ɏ��s���܂����B");
            return new Material(loadedMaterial);
        }
    }
}