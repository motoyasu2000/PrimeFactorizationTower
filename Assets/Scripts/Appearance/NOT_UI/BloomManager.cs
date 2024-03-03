using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//isLightUpStartがtrueになると、bloomの値が上昇し続けて画面がまぶしくなっていくようにするクラス。ゲームオーバー時に呼ばれる演出。
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
