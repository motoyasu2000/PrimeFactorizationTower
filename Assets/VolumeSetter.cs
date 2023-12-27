using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetter : MonoBehaviour
{
    Slider[] sliders = new Slider[3];
    private void Awake()
    {
        sliders[0] = GameObject.Find("BGM_Volume").GetComponent<Slider>();
        sliders[1] = GameObject.Find("SE_Volume").GetComponent<Slider>();
        sliders[2] = GameObject.Find("Voice_Volume").GetComponent<Slider>();

        if (sliders[0] != null) sliders[0].value = SoundManager.SoundManagerInstance.volume_BGM;
        if (sliders[0] != null) sliders[1].value = SoundManager.SoundManagerInstance.volume_SE;
        if (sliders[0] != null) sliders[2].value = SoundManager.SoundManagerInstance.volume_Voice;

        Debug.Log(sliders[0].name + " " +sliders[1].name + " " + sliders[2].name);
    }
    public void SetVolumeBGM()
    {
        SoundManager.SoundManagerInstance.volume_BGM = GetComponent<Slider>().value;
        Debug.Log(gameObject);
        SoundManager.SaveSoundData();
    }
    public void SetVolumeSE()
    {
        SoundManager.SoundManagerInstance.volume_SE = GetComponent<Slider>().value;
        Debug.Log(gameObject);
        SoundManager.SaveSoundData();
    }
    public void SetVolumeVoice()
    {
        SoundManager.SoundManagerInstance.volume_Voice = GetComponent<Slider>().value;
        Debug.Log(gameObject);
        SoundManager.SaveSoundData();
    }
}
