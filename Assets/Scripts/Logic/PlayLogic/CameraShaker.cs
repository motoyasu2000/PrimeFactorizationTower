using UnityEngine;

/// <summary>
/// mainカメラを振動させるスクリプト
/// 素因数分解をミスした際の演出に使用する
/// </summary>
public class CameraShaker : MonoBehaviour
{
    public float shakePower = 0.001f; //振動の強さ(揺れる範囲)

    //引数で受け取ったmagnitudeに合わせて、カメラを動かす強さも調整する(1を加算するのはmagnitudeが0の時にも地震を発生させるため)
    public void MoveRandomCamera(float magnitude)
    {
        transform.localPosition = Random.insideUnitSphere * shakePower *(magnitude+1);
    }

    //カメラ位置を初期化
    public void InitializeCameraPosition()
    {
        transform.localPosition = Vector3.zero;
    }
}
