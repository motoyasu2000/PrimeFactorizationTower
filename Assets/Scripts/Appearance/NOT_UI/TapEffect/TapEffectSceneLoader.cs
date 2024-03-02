using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TapEffectSceneLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        //TapEffectSceneを非同期ロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TapEffectScene", LoadSceneMode.Additive);

        //ロードが完了するまで待機
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //ロード完了後
        Scene tapEffectScene = SceneManager.GetSceneByName("TapEffectScene");
        GameObject[] rootGameObjects = tapEffectScene.GetRootGameObjects();
        Camera tapEffectCamera = null;
        foreach (GameObject obj in rootGameObjects)
        {
            if (obj.name == "TapEffectCamera") tapEffectCamera = obj.GetComponent<Camera>();
        }

        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        //Debug.Log($"CameraData: {cameraData}, CameraDataStack: {cameraData.cameraStack}, TapEffectCamera: {tapEffectCamera}");
        cameraData.cameraStack.Add(tapEffectCamera);

    }

}