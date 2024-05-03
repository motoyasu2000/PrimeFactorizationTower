using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//素因数分解を間違えた場合に、ペナルティとして地震を発生させるクラス。
public class EarthQuakeManager : MonoBehaviour
{
    static readonly float earthQuakeScale = 1f; //地震の振幅のスケール
    static readonly float timeScale = 2f; //地震の波長のスケール
    static readonly float earthQuakeTime = 0.5f; //地震の長さ

    bool isEarthquakeHappening = false; //今地震が起きているか
    int magnitude = 0; //地震の大きさ、ミスするごとに大きくなっていく。この数値分だけ、指数関数的に自身が大きくなる 最初は演出のみで揺らさない。
    float elapsedEarthQuakeTime = 0; //地震の経過時間

    GameObject ground;
    Rigidbody2D groundRb;
    CameraShaker cameraShaker;

    //今振動中かどうか
    public bool IsEarthquakeHappening => isEarthquakeHappening;

    //総合的な波長スケール。2πをかけることで、earthQuakeTime*timeScaleが整数値であれば移動開始地点で止まるようにする。
    float ComprehensiveTimeScale => timeScale * Mathf.PI * 2;

    //失敗ごとに加算されるmagnitudeを2乗することで、ペナルティのリスクを高める。
    float ComprehensiveEarthQuakeScale => earthQuakeScale * magnitude * magnitude;

    void Start()
    {
        ground = GameObject.Find("GroundGenerator");
        groundRb = ground.GetComponent<Rigidbody2D>();
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
    }

    //地震を発生させるメソッド
    public void TriggerEarthQuake()
    {
        isEarthquakeHappening = true;
        StartCoroutine(SwayUpAndDown());
        StartCoroutine(UpdateElapsedIime());
        StartCoroutine(ShakeCamera());
    }

    //地震終了後に様々な初期化を行うメソッド
    void InitializeEarthQuake()
    {
        isEarthquakeHappening = false;
        elapsedEarthQuakeTime = 0;
        groundRb.velocity = Vector2.zero;
        groundRb.rotation = 0;
        cameraShaker.InitCameraPosition();
        magnitude++;
    }

    //一定時間地面を上下に揺らす
    IEnumerator SwayUpAndDown()
    {
        while (isEarthquakeHappening)
        {
            float moveY = Mathf.Sin(elapsedEarthQuakeTime * ComprehensiveTimeScale) * ComprehensiveEarthQuakeScale;
            groundRb.velocity = new Vector2(0, moveY);
            yield return new WaitForEndOfFrame();
        }
    }

    //一定時間地震時間を管理
    IEnumerator UpdateElapsedIime()
    {
        while (isEarthquakeHappening)
        {
            elapsedEarthQuakeTime += Time.deltaTime;
            if (elapsedEarthQuakeTime > earthQuakeTime) InitializeEarthQuake();
            yield return new WaitForEndOfFrame();
        }
    }

    //カメラを振動させる。
    IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = Camera.main.transform.position;
        while (isEarthquakeHappening)
        {
            cameraShaker.MoveRandomCamera(magnitude);
            yield return new WaitForEndOfFrame();
        }
    }

}
