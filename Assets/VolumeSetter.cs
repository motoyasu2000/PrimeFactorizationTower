using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetter : MonoBehaviour
{
    Slider[] sliders = new Slider[3];
    float[] preSliderValue = new float[3];
    private void Awake()
    {
        sliders[0] = GameObject.Find("BGM_Volume").transform.GetChild(0).GetComponent<Slider>();
        sliders[1] = GameObject.Find("SE_Volume").transform.GetChild(0).GetComponent<Slider>();
        sliders[2] = GameObject.Find("Voice_Volume").transform.GetChild(0).GetComponent<Slider>();

        if (sliders[0] != null) sliders[0].value = SoundManager.SoundManagerInstance.volume_BGM;
        if (sliders[0] != null) sliders[1].value = SoundManager.SoundManagerInstance.volume_SE;
        if (sliders[0] != null) sliders[2].value = SoundManager.SoundManagerInstance.volume_Voice;

        Debug.Log(sliders[0].value + " " +sliders[1].value + " " + sliders[2].value);
    }
    private void Update()
    {
        SetVolumeBGM();
        SetVolumeSE();
        SetVolumeVoice();
    }
    public void SetVolumeBGM()
    {
        preSliderValue[0] = sliders[0].value;
        SoundManager.SoundManagerInstance.volume_BGM = sliders[0].value;
        Debug.Log(gameObject);
        if (preSliderValue[0] != sliders[0].value) SoundManager.SaveSoundData(); //スライダーの値が変更されたらセーブを行う
    }
    public void SetVolumeSE()
    {
        preSliderValue[0] = sliders[0].value;
        SoundManager.SoundManagerInstance.volume_SE = sliders[1].value;
        Debug.Log(gameObject);
        if (preSliderValue[1] != sliders[1].value) SoundManager.SaveSoundData(); //スライダーの値が変更されたらセーブを行う
    }
    public void SetVolumeVoice()
    {
        preSliderValue[0] = sliders[0].value;
        SoundManager.SoundManagerInstance.volume_Voice = sliders[2].value;
        Debug.Log(gameObject);
        if (preSliderValue[2] != sliders[2].value) SoundManager.SaveSoundData(); //スライダーの値が変更されたらセーブを行う
    }
}
