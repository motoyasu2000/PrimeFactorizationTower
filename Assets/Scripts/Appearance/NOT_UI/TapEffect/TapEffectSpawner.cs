using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectSpawner : MonoBehaviour
{
    GameObject tapEffect;

    private void Start()
    {
        tapEffect = Resources.Load("TapEffect") as GameObject;
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
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = 0;

                //エフェクトをインスタンス化
                Instantiate(tapEffect, touchPosition, Quaternion.identity);
            }
        }
    }
}
