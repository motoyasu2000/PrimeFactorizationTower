using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 音声を管理するクラス
/// シングルトンパターンを使用している
/// </summary>
[Serializable]
public class SoundManager : MonoBehaviour
{
    //インスタンス
    private static SoundManager instance;
    public static SoundManager Ins => instance;

    //保存を行う変数たち、スライダーの値によって変更する
    [SerializeField] float volume_BGM;
    [SerializeField] float volume_SE;
    [SerializeField] float volume_Voice;

    public float Volume_BGM => volume_BGM;
    public float Volume_SE => volume_SE;
    public float Volume_Voice => volume_Voice;

    //音声データを持つゲームオブジェクトの親オブジェクトのtransform
    Transform transVoices;
    Transform transSEs;
    Transform transBGMs;

    //音声データ
    AudioSource voice_done;
    AudioSource voice_criteriaMet;
    AudioSource voice_freeze;
    AudioSource se_done;
    AudioSource se_freeze;
    AudioSource se_tap;
    AudioSource se_warning;
    AudioSource bgm_play;
    AudioSource bgm_title;
    AudioSource bgm_material;

    //音声データのプロパティ
    public AudioSource VOICE_DONE => voice_done;
    public AudioSource VOICE_CRITERIAMAT => voice_criteriaMet;
    public AudioSource VOICE_FREEZE => voice_freeze;
    public AudioSource SE_DONE => se_done;
    public AudioSource SE_FREEZE => se_freeze;
    public AudioSource SE_TAP => se_tap;
    public AudioSource SE_Warning => se_warning;
    public AudioSource BGM_PLAY => bgm_play;
    public AudioSource BGM_TITLE => bgm_title;
    public AudioSource BGM_MATERIAL => bgm_material;

    //音声データを声・効果音・BGMに分けたリスト
    List<AudioSource> Voices = new List<AudioSource>();
    List<AudioSource> SEs = new List<AudioSource>();
    List <AudioSource> BGMs = new List<AudioSource>();

    void Awake()
    {
        //音声データの初期化処理
        transVoices = transform.Find("Voices");
        transSEs = transform.Find("SEs");
        transBGMs = transform.Find("BGMs");
        voice_done = transVoices.Find("Done").GetComponent<AudioSource>();
        voice_criteriaMet = transVoices.Find("CriteriaMet").GetComponent<AudioSource>();
        voice_freeze = transVoices.Find("Freeze").GetComponent<AudioSource>();
        se_done = transSEs.Find("Done").GetComponent<AudioSource>();
        se_freeze = transSEs.Find("Freeze").GetComponent<AudioSource>();
        se_tap = transSEs.Find("Tap").GetComponent<AudioSource>();
        se_warning = transSEs.Find("Warning").GetComponent<AudioSource>();
        bgm_play = transBGMs.Find("Play").GetComponent <AudioSource>();
        bgm_title = transBGMs.Find("Title").GetComponent<AudioSource>();
        bgm_material = transBGMs.Find("Material").GetComponent<AudioSource>();

        //リストの更新処理
        Voices.Add(voice_done);
        Voices.Add(voice_criteriaMet);
        Voices.Add(voice_freeze);
        SEs.Add(se_done);
        SEs.Add(se_freeze);
        SEs.Add(se_tap);
        SEs.Add(se_warning);
        BGMs.Add(bgm_play);
        BGMs.Add(bgm_title);
        BGMs.Add(bgm_material);

        //インスタンスの生成
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "PlayScene") PlayAudio(BGM_PLAY);
            else if (sceneName == "TitleScene") PlayAudio(BGM_TITLE);
        }

        //音声システムデータの読み込み
        LoadSoundSettingData();
    }
    private void Update()
    {
        instance.SoundSetting();
    }

    //音量を設定する関数たち
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
        //もし新しくBGMを流す命令が入ってきたらBGMを止める。
        if (instance.BGMs.Contains(audioSource))
        {
            foreach(var BGM in BGMs)
            {
                BGM.Stop();
            }
        }
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

    //BGMがフェードアウトしていくようにするメソッドで、ゲームオーバー時に呼び出される。
    public void FadeOutVolume()
    {
        GameObject gameObject = new GameObject("FadeoutVolumer");
        gameObject.AddComponent<FadeOutVolumer>();
    }

    //音声設定データ(BGM音量・SE音量・ボイス音量)をJson形式で保存する
    public static void SaveSoundSettingData()
    {
        SoundManager dSoundmanagerInstance = instance;
        string jsonstr = JsonUtility.ToJson(dSoundmanagerInstance);
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/SoundSetting.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    //Json形式の音声設定データ(BGM音量・SE音量・ボイス音量)をロードする
    public static void LoadSoundSettingData()
    {
        //ロード時にデータが存在しなければ、初期値で生成する。
        if (!File.Exists(Application.persistentDataPath + "/SoundSetting.json"))
        {
            float defaultVolume = 0.5f;
            instance.volume_BGM = defaultVolume;
            instance.volume_SE = defaultVolume;
            instance.volume_Voice = defaultVolume;
            return;
        }
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/SoundSetting.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<SoundSettingData>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.volume_BGM = obj.volume_BGM;
        instance.volume_SE = obj.volume_SE;
        instance.volume_Voice = obj.volume_Voice;
    }
}

//Jsonからインスタンスを生成するためのクラス
class SoundSettingData
{
    public float volume_BGM;
    public float volume_SE;
    public float volume_Voice;
}
