using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.SocialPlatforms;

//素因数分解を間違えた場合に、ペナルティとして地震を発生させるクラス。
public class EarthQuakeManager : MonoBehaviour
{
    static readonly float earthQuakeScale = 1f; //地震の振幅のスケール
    static readonly float lambdaScale = 1f; //地震の波長のスケール
    static readonly float earthQuakeTime = 1f; //地震の長さ

    bool isEarthquakeHappening = false; //今地震が起きているか
    int magnitude = 0; //地震の大きさ、ミスするごとに大きくなっていく。この数値分だけ、指数関数的に自身が大きくなる 最初は演出のみで揺らさない。
    float elapsedEarthQuakeTime = 0; //地震の経過時間

    GameObject ground;
    Rigidbody2D groundRb;
    CameraShaker cameraShaker;
    EffectTextManager effectTextManager;

    //今振動中かどうか
    public bool IsEarthquakeHappening => isEarthquakeHappening;

    //総合的な波長スケール。2πをかけることで、earthQuakeTime*lambdaScaleが整数値であれば移動開始地点で止まるようにする。
    float ComprehensiveTimeScale => lambdaScale * Mathf.PI * 2;

    //失敗ごとに加算されるmagnitudeを2乗することで、ペナルティのリスクを高める。
    float ComprehensiveEarthQuakeScale => earthQuakeScale * magnitude * magnitude;

    void Start()
    {
        ground = GameObject.Find("GroundGenerator");
        groundRb = ground.GetComponent<Rigidbody2D>();
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        effectTextManager = GameObject.Find("EffectText").GetComponent<EffectTextManager>();

        //地震後に、地面が初期位置に戻るかのチェック(earthQuakeTime * lambdaScaleが整数値か)
        float initPointReturnChecker = (earthQuakeTime * lambdaScale);
        float epsilon = 0.01f;
        if (initPointReturnChecker - (int)initPointReturnChecker > epsilon) 
            Debug.LogError("地震発生後、地面は初期位置に戻りません。");
    }

    //地震を発生させるメソッド
    public void TriggerEarthQuake()
    {
        DisplayText();
        isEarthquakeHappening = true;
        StartCoroutine(UpdateElapsedIime());
        StartCoroutine(SwayUpAndDown());
        StartCoroutine(ShakeCamera());
        StartCoroutine(PlayWarningSound());
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

    //地震発生時のテキストを表示させる
    void DisplayText()
    {
        effectTextManager.DisplayEffectText($"MISS{magnitude+1}", earthQuakeTime, Color.red);
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

    //警告音を再生
    IEnumerator PlayWarningSound()
    {
        SoundManager.Ins.PlayAudio(SoundManager.Ins.SE_Warning);
        while (isEarthquakeHappening)
        {
            yield return new WaitForEndOfFrame();
        }
        SoundManager.Ins.StopAudio(SoundManager.Ins.SE_Warning);
    }
}
