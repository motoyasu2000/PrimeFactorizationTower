using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タップした位置にタップエフェクトを表示するためのクラス。
/// 指を話した時のSEの再生も行う。
/// TapEffectScene内に存在するゲームオブジェクトにアタッチされている。
/// </summary>
public class TapEffectSpawner : MonoBehaviour
{
    GameObject tapEffect;
    SoundManager soundManager;
    Camera tapEffectCamera;
    Touch touch;
    Scene tapEffectScene;

    private void Start()
    {
        tapEffect = Resources.Load("TapEffect") as GameObject;
        soundManager = SoundManager.Ins;
        tapEffectCamera = GameObject.Find("TapEffectCamera").GetComponent<Camera>();
        tapEffectScene = SceneManager.GetSceneByName("TapEffectScene");
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0); //最初のタッチを取得

            //タッチが開始された瞬間にエフェクトを生成
            if (touch.phase == TouchPhase.Began)
            {
                SpawnEffect();
            }
            //指が画面にずっとついている状態であり、動いていないときエフェクトを生成
            if (touch.phase == TouchPhase.Stationary)
            {
                SpawnEffect();
            }
            //指が画面にずっとついている状態であり、動いているときエフェクトを生成
            if (touch.phase == TouchPhase.Stationary)
            {
                SpawnEffect();
            }
            //指を話したとき、SEを再生
            if(touch.phase == TouchPhase.Ended)
            {
                soundManager.PlayAudio(soundManager.SE_TAP);
            }
        }
    }

    void SpawnEffect()
    {
        //スクリーン座標をワールド座標に変換
        Vector3 touchPosition = tapEffectCamera.ScreenToWorldPoint(touch.position);
        touchPosition.z = 0;

        //エフェクトをインスタンス化して、TapEffectScene内に送る
        GameObject nowTapEffect = Instantiate(tapEffect, touchPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(nowTapEffect, tapEffectScene);
    }
}
