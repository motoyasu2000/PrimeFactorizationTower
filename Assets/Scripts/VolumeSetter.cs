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

        if (sliders[0] != null) sliders[0].value = SoundManager.SoundManagerInstance.Volume_BGM;
        if (sliders[1] != null) sliders[1].value = SoundManager.SoundManagerInstance.Volume_SE;
        if (sliders[2] != null) sliders[2].value = SoundManager.SoundManagerInstance.Volume_Voice;

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
        SoundManager.SoundManagerInstance.SetVolumeBGM(sliders[0].value);
        //Debug.Log(gameObject);
        if (preSliderValue[0] != sliders[0].value) SoundManager.SaveSoundData(); //スライダーの値が変更されたらセーブを行う
        preSliderValue[0] = sliders[0].value;
    }
    public void SetVolumeSE()
    {
        SoundManager.SoundManagerInstance.SetVolumeSE(sliders[1].value);
        //Debug.Log(gameObject);
        if (preSliderValue[1] != sliders[1].value) SoundManager.SaveSoundData(); //スライダーの値が変更されたらセーブを行う
        preSliderValue[1] = sliders[1].value;
    }
    public void SetVolumeVoice()
    {
        SoundManager.SoundManagerInstance.SetVolumeVoice(sliders[2].value);
        //Debug.Log(gameObject);
        if (preSliderValue[2] != sliders[2].value) SoundManager.SaveSoundData(); //スライダーの値が変更されたらセーブを行う
        preSliderValue[2] = sliders[2].value;
    }
}
