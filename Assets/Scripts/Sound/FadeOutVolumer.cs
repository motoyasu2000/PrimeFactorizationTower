using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//徐々にBGM音量を下げていくクラス。ゲームオーバー時に、ゲームオブジェクトにこのクラスをアタッチすることで、BGMがフェードアウトするようにする。
public class FadeOutVolumer : MonoBehaviour
{
    void Update()
    {
        float newVolume = SoundManager.SoundManagerInstance.Volume_BGM - Time.deltaTime * 0.5f;
        SoundManager.SoundManagerInstance.SetVolumeBGM(newVolume);
    }
}
