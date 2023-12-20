using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
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
    void Start()
    {
        transVoices = transform.Find("Voices");
        transSEs = transform.Find("SEs");
        voice_done = transVoices.Find("Done").GetComponent<AudioSource>();
        voice_criteriaMet = transVoices.Find("CriteriaMet").GetComponent<AudioSource>();
        voice_freeze = transVoices.Find("Freeze").GetComponent<AudioSource>();
        se_done = transSEs.Find("Done").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
