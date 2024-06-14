using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音声設定データ(BGM音量・SE音量・ボイス音量)を設定するクラス
/// 音量を調節するバーによって操作される。
/// </summary>
public class VolumeSetter : MonoBehaviour
{
    Slider[] sliders = new Slider[3];
    float[] preSliderValue = new float[3];
    private void Awake()
    {
        sliders[0] = GameObject.Find("BGM_Volume").transform.GetChild(0).GetComponent<Slider>(); //BGM音量調整スライダー
        sliders[1] = GameObject.Find("SE_Volume").transform.GetChild(0).GetComponent<Slider>(); //SE音量調整スライダー
        sliders[2] = GameObject.Find("Voice_Volume").transform.GetChild(0).GetComponent<Slider>(); //ボイス音量調整スライダー

        //スライダーのvalueの初期化
        if (sliders[0] != null) sliders[0].value = SoundManager.Ins.Volume_BGM;
        if (sliders[1] != null) sliders[1].value = SoundManager.Ins.Volume_SE;
        if (sliders[2] != null) sliders[2].value = SoundManager.Ins.Volume_Voice;

        Debug.Log(sliders[0].value + " " +sliders[1].value + " " + sliders[2].value);
    }
    private void Update()
    {
        SetVolumeBGM();
        SetVolumeSE();
        SetVolumeVoice();
    }

    //サウンドマネージャーの音量を調整する変数を変更して、その情報を保存するメソッドたち
    public void SetVolumeBGM()
    {
        SoundManager.Ins.SetVolumeBGM(sliders[0].value);
        //Debug.Log(gameObject);
        if (preSliderValue[0] != sliders[0].value) SoundManager.SaveSoundSettingData(); //スライダーの値が変更されたらセーブを行う
        preSliderValue[0] = sliders[0].value;
    }
    public void SetVolumeSE()
    {
        SoundManager.Ins.SetVolumeSE(sliders[1].value);
        //Debug.Log(gameObject);
        if (preSliderValue[1] != sliders[1].value) SoundManager.SaveSoundSettingData(); //スライダーの値が変更されたらセーブを行う
        preSliderValue[1] = sliders[1].value;
    }
    public void SetVolumeVoice()
    {
        SoundManager.Ins.SetVolumeVoice(sliders[2].value);
        //Debug.Log(gameObject);
        if (preSliderValue[2] != sliders[2].value) SoundManager.SaveSoundSettingData(); //スライダーの値が変更されたらセーブを行う
        preSliderValue[2] = sliders[2].value;
    }
}
