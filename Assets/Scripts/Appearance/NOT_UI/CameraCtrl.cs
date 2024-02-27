using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ブロックが積みあがっていき、高くなりすぎるとブロックがカメラにとらえられなくなる恐れがある。
//そこでブロックの最高点からカメラの高さや範囲を計算して拡大するクラス。
public class CameraCtrl : MonoBehaviour
{
    float startHeight = 6; //カメラの移動を開始する高さ
    float newCameraHeight; //最新のカメラの高さ
    Vector3 defo; //初期のカメラの座標
    ScoreManager scoreManager;
    public float StartHeight => startHeight;

    public float NewCameraHeight => newCameraHeight;
    void Start()
    {
        defo = transform.position;
        scoreManager = ScoreManager.ScoreManagerInstance;
    }
    void Update()
    {
        if (scoreManager.NowScore < startHeight) return;
        Camera.main.orthographicSize = scoreManager.NowScore - startHeight + 10; //scoreManager.MaxHeight - startHeightは変化量、10は初期値
        newCameraHeight = defo.y + (scoreManager.NowScore - startHeight) * 0.3f; //画面の下30％部分を固定してカメラの範囲を拡大
        Camera.main.transform.position = new Vector3(defo.x,newCameraHeight, defo.z);
    }
}
