using UnityEngine;

//タップエフェクトが、パーティクルの再生が終了したら自動的にDestroyされるようにするクラス。
public class TapEffectDestroyer : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constant);
    }
}
