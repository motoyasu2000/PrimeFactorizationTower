﻿using UnityEngine;

//徐々にBGM音量を下げていくクラス。ゲームオーバー時に、ゲームオブジェクトにこのクラスをアタッチすることで、BGMがフェードアウトするようにする。
public class FadeOutVolumer : MonoBehaviour
{
    const float fadeOutSpeedCoefficient = 0.5f; //音量の減少速度を調整する係数
    void Update()
    {
        float newVolume = SoundManager.SoundManagerInstance.Volume_BGM - Time.deltaTime * fadeOutSpeedCoefficient;
        SoundManager.SoundManagerInstance.SetVolumeBGM(newVolume);
    }
}
