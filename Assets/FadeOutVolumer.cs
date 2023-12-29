using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutVolumer : MonoBehaviour
{
    void Update()
    {
        float newVolume = SoundManager.SoundManagerInstance.Volume_BGM - Time.deltaTime * 0.5f;
        SoundManager.SoundManagerInstance.SetVolumeBGM(newVolume);
    }
}
