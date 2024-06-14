using UnityEngine;

/// <summary>
/// タップエフェクトが、パーティクルの再生が終了したら自動的にDestroyされるようにするためのクラス。
/// </summary>
public class TapEffectDestroyer : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constant);
    }
}
