using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource Voice_done;
    [SerializeField] AudioSource SE_done;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayAudio(string name)
    {
        switch (name)
        {
            case "V_Done":
                Voice_done.Play();
                break;
            case "SE_Done":
                SE_done.Play();
                break;
            default:
                Debug.LogError("SoundManagerのPlayAudioメソッドの引数が間違っています。");
                break;
        }
    }
}
