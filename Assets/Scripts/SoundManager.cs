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

    public float volume_BGM;
    public float volume_SE;
    public float volume_Voice;

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
    public void PlayAudio(AudioSource audioSource)
    {
        audioSource.Play();
    }
    public IEnumerator PlayAudio(AudioSource audioSource, float seconds)
    {
        yield return new WaitForSeconds (seconds);
        PlayAudio(audioSource);
    }

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

    public void SaveSoundData()
    {
        SoundManager dSoundmanagerInstance = instance;
        string jsonstr = JsonUtility.ToJson(dSoundmanagerInstance);
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Savedata/System/SoundSetting.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public void LoadSoundData()
    {
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
