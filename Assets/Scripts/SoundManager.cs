using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager SoundManagerInstance => instance;

    Transform transVoices;
    Transform transSEs;

    [SerializeField] AudioSource voice_done;
    [SerializeField] AudioSource voice_criteriaMet;
    [SerializeField] AudioSource voice_freeze;
    [SerializeField] AudioSource se_done;

    public AudioSource VOICE_DONE => voice_done;
    public AudioSource VOICE_CRITERIAMAT => voice_criteriaMet;
    public AudioSource VOICE_FREEZE => voice_freeze;
    public AudioSource SE_DONE => se_done;
    void Awake()
    {
        transVoices = transform.Find("Voices");
        transSEs = transform.Find("SEs");
        voice_done = transVoices.Find("Done").GetComponent<AudioSource>();
        voice_criteriaMet = transVoices.Find("CriteriaMet").GetComponent<AudioSource>();
        voice_freeze = transVoices.Find("Freeze").GetComponent<AudioSource>();
        se_done = transSEs.Find("Done").GetComponent<AudioSource>();

        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
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
}
