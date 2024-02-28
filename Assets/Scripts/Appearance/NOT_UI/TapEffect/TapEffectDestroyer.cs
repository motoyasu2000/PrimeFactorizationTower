using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectDestroyer : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constant);
    }
}
