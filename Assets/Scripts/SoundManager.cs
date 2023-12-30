using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager SoundManagerInstance => instance;

    //保存を行う変数たち、スライダーの値によって変更する
    [SerializeField] float volume_BGM;
    [SerializeField] float volume_SE;
    [SerializeField] float volume_Voice;

    public float Volume_BGM => volume_BGM;
    public float Volume_SE => volume_SE;
    public float Volume_Voice => volume_Voice;

    Transform transVoices;
    Transform transSEs;
    Transform transBGMs;

    AudioSource voice_done;
    AudioSource voice_criteriaMet;
    AudioSource voice_freeze;
    AudioSource se_done;
    AudioSource bgm_play;

    List<AudioSource> Voices = new List<AudioSource>();
    List<AudioSource> SEs = new List<AudioSource>();
    List <AudioSource> BGMs = new List<AudioSource>();

    public AudioSource VOICE_DONE => voice_done;
    public AudioSource VOICE_CRITERIAMAT => voice_criteriaMet;
    public AudioSource VOICE_FREEZE => voice_freeze;
    public AudioSource SE_DONE => se_done;
    public AudioSource BGM_PLAY => bgm_play;

    bool isGameOver = false;
    void Awake()
    {
        transVoices = transform.Find("Voices");
        transSEs = transform.Find("SEs");
        transBGMs = transform.Find("BGMs");
        voice_done = transVoices.Find("Done").GetComponent<AudioSource>();
        voice_criteriaMet = transVoices.Find("CriteriaMet").GetComponent<AudioSource>();
        voice_freeze = transVoices.Find("Freeze").GetComponent<AudioSource>();
        se_done = transSEs.Find("Done").GetComponent<AudioSource>();
        bgm_play = transBGMs.Find("Play").GetComponent <AudioSource>();

        Voices.Add(voice_done);
        Voices.Add(voice_criteriaMet);
        Voices.Add(voice_freeze);

        SEs.Add(se_done);

        BGMs.Add(bgm_play);

        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        LoadSoundData();
    }
    private void Update()
    {
        instance.SoundSetting();
    }

    public void SetVolumeBGM(float newVolume)
    {
        volume_BGM = newVolume;
    }
    public void SetVolumeSE(float newVolume)
    {
        volume_SE = newVolume;
    }
    public void SetVolumeVoice(float newVolume)
    {
        volume_Voice = newVolume;
    }
    public void PlayAudio(AudioSource audioSource)
    {
        audioSource.Play();
    }
    public IEnumerator PlayAudio(AudioSource audioSource, float seconds)
    {
        yield return new WaitForSeconds (seconds);
        PlayAudio(audioSource);
    }

    public void StopAudio(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    //スライダーから設定したボリュームを実際の音量に反映
    void SoundSetting()
    {
        foreach(var SE in SEs)
        {
            SE.volume = volume_SE;
        }
        foreach (var BGM in BGMs)
        {
            BGM.volume = volume_BGM;
        }
        foreach (var Voice in Voices)
        {
            Voice.volume = volume_Voice;
        }
    }

    public void CheckFlagGameOver()
    {
        instance.isGameOver = true;
    }

    public void FadeOutVolume()
    {
        GameObject gameObject = new GameObject("FadeoutVolumer");
        gameObject.AddComponent<FadeOutVolumer>();
    }

    public static void SaveSoundData()
    {
        SoundManager dSoundmanagerInstance = instance;
        string jsonstr = JsonUtility.ToJson(dSoundmanagerInstance);
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Savedata/System/SoundSetting.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public static void LoadSoundData()
    {
        if (!File.Exists(Application.dataPath + "/Savedata/System/SoundSetting.json"))
        {
            instance.volume_BGM = 0.5f;
            instance.volume_SE = 0.5f;
            instance.volume_Voice = 0.5f;
            return;
        }
        StreamReader reader = new StreamReader(Application.dataPath + "/Savedata/System/SoundSetting.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<JsonLoadSoundManager>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.volume_BGM = obj.volume_BGM;
        instance.volume_SE = obj.volume_SE;
        instance.volume_Voice = obj.volume_Voice;
    }
}

//Jsonからインスタンスを生成するためのクラス
class JsonLoadSoundManager
{
    public float volume_BGM;
    public float volume_SE;
    public float volume_Voice;
}
