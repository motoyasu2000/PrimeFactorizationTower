using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

//ブロックが積みあがっていき、高くなりすぎるとブロックがカメラにとらえられなくなる恐れがある。
//そこでブロックの最高点からカメラを範囲を計算し、拡大する必要がある。カメラの範囲を変更してもUIが正常に保たれるようにする。
//これらを行うクラス
public class CamerasManager : MonoBehaviour
{
    float downerUI_MAXY; //画面下部のUIの最高点。この高さを軸に拡大していく。
    float newCamerasHeight; //新たに変更されるカメラの高さ
    float orthographicSize_defo; //初期のカメラの大きさ
    Vector3 position_defo; //初期のカメラの座標
    RectTransform downerUITransform; //画面下部のUI
    ScoreManager scoreManager;
    Camera mainCamera;
    Camera UICamera;
    void Start()
    {
        //プレイシーンであれば初期化
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
        downerUITransform = GameObject.Find("DownerUI").GetComponent<RectTransform>();
        downerUI_MAXY = downerUITransform.anchorMax.y;
        position_defo = transform.position;
        scoreManager = ScoreManager.Ins;
        mainCamera = Camera.main;
        UICamera = transform.Find("UICamera").GetComponent<Camera>();
        orthographicSize_defo = mainCamera.orthographicSize;

        //Debug.Log(downerUI_MAXY);
    }
    void Update()
    {
        //playSceneでなかったり、特定の高さ以上になっていない場合は処理を行わない。
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
        if (GameInfo.CameraTrackingStartHeight > scoreManager.NowHeight) return;

        float moveingDistance = scoreManager.NowHeight - GameInfo.CameraTrackingStartHeight; //開始高度からの移動距離
        float newOrthographicSize = moveingDistance + orthographicSize_defo; //新たなカメラの大きさ
        mainCamera.orthographicSize = newOrthographicSize;
        UICamera.orthographicSize = newOrthographicSize; //UICameraの大きさも変更しないと、mainCameraが大きくなるにつれ、UIが相対的に小さくなっていってしまう。

        newCamerasHeight = position_defo.y + moveingDistance * downerUI_MAXY; //中心が画面下UIの最大点になるようにする。
        mainCamera.transform.position = new Vector3(position_defo.x,newCamerasHeight, position_defo.z);
    }
}
