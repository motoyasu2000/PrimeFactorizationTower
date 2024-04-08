using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カメラコンポーネントが無効にならなようするためのクラス
public class CameraEnabler : MonoBehaviour
{
    Camera camera;
    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        camera.enabled = true;
    }
}
