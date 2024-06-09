using UnityEngine;

namespace MaterialLibrary
{
    public enum WaveMaterialProperty
    {
        [ShaderProperty("_Color")]
        MainColor,
        [ShaderProperty("_AnotherColor")]
        AnotherColor,
        [ShaderProperty("_WaveAmplitude")]
        amplitude,
        [ShaderProperty("_WaveFrequency")]
        frequency,
        [ShaderProperty("_WaveSpeed")]
        speed,
        [ShaderProperty("_WaveHeight")]
        height,
    }

    public class WaveMaterialEnumBinder : EnumParametersBinder<WaveMaterialProperty>
    {
        public override string MaterialPathAndName => MaterialPasses.GetWaveMaterialName();
        protected override Material LoadMaterial()
        {
            var loadedMaterial = Resources.Load<Material>(MaterialPathAndName);
            if (loadedMaterial == null) Debug.LogError("マテリアルのロードに失敗しました。");
            return new Material(loadedMaterial);
        }
    }
}
