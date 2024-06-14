using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///ブロックの最高点からメインカメラを範囲を計算し、拡大するためのクラス。
///ブロックが積みあがっていき、高くなりすぎるとブロックがカメラにとらえられなくなる恐れがあるため。
///また、メインカメラを拡大してもUIが正常に保たれるようにする。
/// </summary>
public class CamerasManager : MonoBehaviour
{
    float downerUI_MAXY; //画面下部のUIの最高点。この高さを軸に拡大していく。
    float newCameraHeight; //新たに変更されるカメラの高さ
    float orthographicSize_defo; //初期のカメラの大きさ
    Vector3 position_defo; //初期のカメラの座標
    Vector3 oldCameraPosition; //1フレーム前のカメラの位置
    Vector3 newCameraPosition; //現在のカメラの位置(ブロックが高くなるごとにカメラが上に行く)
    RectTransform downerUITransform; //画面下部のUI
    Camera mainCamera;
    Camera UICamera;
    MaxHeightCalculator maxHeightCalculator;

    public bool changeCameraPosition => oldCameraPosition != newCameraPosition; 

    public Vector3 NowCameraPosition => newCameraPosition;
    void Start()
    {
        //プレイシーンであれば初期化
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
        downerUITransform = GameObject.Find("DownerUI").GetComponent<RectTransform>();
        downerUI_MAXY = downerUITransform.anchorMax.y;
        position_defo = transform.position;
        mainCamera = Camera.main;
        UICamera = transform.Find("UICamera").GetComponent<Camera>();
        orthographicSize_defo = mainCamera.orthographicSize;
        maxHeightCalculator = GameObject.Find("MaxHeightCalculator").GetComponent<MaxHeightCalculator>();

        //Debug.Log(downerUI_MAXY);
    }
    void Update()
    {
        //playSceneでなかったり、特定の高さ以上になっていない場合は処理を行わない。
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
        if (GameInfo.CameraTrackingStartHeight > maxHeightCalculator.NowHeight) return;

        float moveingDistance = maxHeightCalculator.NowHeight - GameInfo.CameraTrackingStartHeight; //開始高度からの移動距離
        float newOrthographicSize = moveingDistance + orthographicSize_defo; //新たなカメラの大きさ
        mainCamera.orthographicSize = newOrthographicSize;
        UICamera.orthographicSize = newOrthographicSize; //UICameraの大きさも変更しないと、mainCameraが大きくなるにつれ、UIが相対的に小さくなっていってしまう。

        oldCameraPosition = newCameraPosition;
        newCameraHeight = position_defo.y + moveingDistance * downerUI_MAXY; //中心が画面下UIの最大点になるようにする。
        newCameraPosition = new Vector3(position_defo.x, newCameraHeight, position_defo.z);
        if(changeCameraPosition) mainCamera.transform.position = newCameraPosition;
    }
}
