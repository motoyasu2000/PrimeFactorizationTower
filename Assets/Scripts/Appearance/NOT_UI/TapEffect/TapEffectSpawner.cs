using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapEffectSpawner : MonoBehaviour
{
    GameObject tapEffect;
    SoundManager soundManager;
    Camera tapEffectCamera;
    Scene tapEffectScene;

    private void Start()
    {
        tapEffect = Resources.Load("TapEffect") as GameObject;
        soundManager = SoundManager.SoundManagerInstance;
        tapEffectCamera = GameObject.Find("TapEffectCamera").GetComponent<Camera>();
        tapEffectScene = SceneManager.GetSceneByName("TapEffectScene");
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); //最初のタッチを取得

            //タッチが開始された瞬間にエフェクトを生成
            if (touch.phase == TouchPhase.Began)
            {
                //スクリーン座標をワールド座標に変換
                Vector3 touchPosition = tapEffectCamera.ScreenToWorldPoint(touch.position);
                touchPosition.z = 0;

                //エフェクトをインスタンス化して、TapEffectManager内に送る
                GameObject nowTapEffect = Instantiate(tapEffect, touchPosition, Quaternion.identity);
                SceneManager.MoveGameObjectToScene(nowTapEffect, tapEffectScene);

                soundManager.PlayAudio(soundManager.SE_TAP);
            }
        }
    }
}
