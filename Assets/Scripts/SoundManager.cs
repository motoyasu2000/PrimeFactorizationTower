using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource done;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Recite(string name)
    {
        switch (name)
        {
            case "Done":
                done.Play();
                break;
            default:
                Debug.LogError("SoundManager��Ricite���\�b�h�̈������Ԉ���Ă��܂��B");
                break;
        }
    }
}
