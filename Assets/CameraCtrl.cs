using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    Vector3 defo; //初期のカメラの座標
    float startHeight = 6; //カメラの移動を開始する高さ
    public float StartHeight => startHeight;
    float newCameraHeight;
    public float NewCameraHeight => newCameraHeight;
    void Start()
    {
        defo = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreManager.MaxHeight < startHeight) return;
        Camera.main.orthographicSize = scoreManager.MaxHeight - startHeight + 10; //scoreManager.MaxHeight - startHeightは変化量、10は初期値
        newCameraHeight = defo.y + (scoreManager.MaxHeight - startHeight) * 0.3f; //画面の下30％部分を固定してカメラを拡大 //本来startHeightが存在しなかった場合を考え、startheightがあった場合にどのように逆算できるかを考えるとmaxheightにも0.3が掛けられている理由がわかる。
        Camera.main.transform.position = new Vector3(defo.x,newCameraHeight, defo.z);
    }
}
