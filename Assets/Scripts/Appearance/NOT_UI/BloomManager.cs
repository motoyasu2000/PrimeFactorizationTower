using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// bloomの値を上昇し続けて画面が明るくなっていくようにするクラス。
/// ゲームオーバー時に呼ばれる演出。
/// 外部からisLightUpStartをtrueにすることで、Update内で明るくする処理を行う
/// </summary>
public class BloomManager : MonoBehaviour
{
    const float lightUpPowerCoefficient = 30f; //時間当たりにどのくらいbloomの値を大きくするのかを決定する値
    bool isLightUpStart = false;
    Volume volume;
    VolumeProfile profile;
    Bloom bloom;

    void Start()
    {
        volume = GetComponent<Volume>();
        profile = volume.profile;
        profile.TryGet(out bloom);
    }
    void Update()
    {
        if (isLightUpStart)
        {
            bloom.intensity.value += Time.deltaTime * lightUpPowerCoefficient;
        }
    }
    public void LightUpStart()
    {
        isLightUpStart=true;
    }
}
