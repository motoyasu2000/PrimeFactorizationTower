using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetter : MonoBehaviour
{
    public void SetVolumeBGM()
    {
        SoundManager.SoundManagerInstance.volume_BGM = GetComponent<Slider>().value;
        Debug.Log(gameObject);
        SoundManager.SoundManagerInstance.SaveSoundData();
    }
    public void SetVolumeSE()
    {
        SoundManager.SoundManagerInstance.volume_SE = GetComponent<Slider>().value;
        Debug.Log(gameObject);
        SoundManager.SoundManagerInstance.SaveSoundData();
    }
    public void SetVolumeVoice()
    {
        SoundManager.SoundManagerInstance.volume_Voice = GetComponent<Slider>().value;
        Debug.Log(gameObject);
        SoundManager.SoundManagerInstance.SaveSoundData();
    }
}
