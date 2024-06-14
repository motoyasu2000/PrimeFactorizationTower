using UnityEngine;

namespace MaterialLibrary
{
    /// <summary>
    /// 波が表示されるマテリアルのプロパティ。
    /// 波の上下の2色と、波の移動速度や物理的な値を設定できる。
    /// </summary>
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

    /// <summary>
    /// 波マテリアルのBinder
    /// </summary>
    public class WaveMaterialBinder : AbstBinder<WaveMaterialProperty>
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
