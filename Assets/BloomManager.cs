using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomManager : MonoBehaviour
{
    Volume volume;
    VolumeProfile profile;
    Bloom bloom;
    public bool isLightUpStart = false;
    void Start()
    {
        volume = GetComponent<Volume>();
        profile = volume.profile;
        profile.TryGet(out bloom);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLightUpStart)
        {
            bloom.intensity.value += 0.2f;
        }
    }
}
