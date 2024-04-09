using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カメラコンポーネントが無効にならないようするためのクラス(AIの観察に設定下カメラが実行時に無効になるため)
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
