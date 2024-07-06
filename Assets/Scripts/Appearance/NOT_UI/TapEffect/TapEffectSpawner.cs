using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// タップした位置にタップエフェクトを表示するためのクラス。
/// 指を話した時のSEの再生も行う。
/// TapEffectScene内に存在するゲームオブジェクトにアタッチされている。
/// </summary>
public class TapEffectSpawner : MonoBehaviour
{
    GameObject tapEffect;
    SoundManager soundManager;
    EventSystem eventSystem;
    GraphicRaycaster raycaster;
    Camera uiCamera;
    Camera tapEffectCamera;
    Touch touch;
    Scene tapEffectScene;

    private void Start()
    {
        tapEffect = Resources.Load("TapEffect") as GameObject;
        soundManager = SoundManager.Ins;
        eventSystem = FindObjectOfType<EventSystem>();
        raycaster = FindObjectOfType<GraphicRaycaster>();
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
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
                SpawnVisualEffect();
            }
            //指が画面にずっとついている状態であり、動いていないときエフェクトを生成
            if (touch.phase == TouchPhase.Stationary)
            {
                SpawnVisualEffect();
            }
            //指が画面にずっとついている状態であり、動いているときエフェクトを生成
            if (touch.phase == TouchPhase.Moved)
            {
                SpawnVisualEffect();
            }
            //指を話したとき、下にあるゲームオブジェクトに合わせてSEを再生する
            if(touch.phase == TouchPhase.Ended)
            {
                GameObject touchObj = GetTouchObj(touch);
                SpawnSoundEffect(touchObj);
            }
        }
    }

    void SpawnVisualEffect()
    {
        //スクリーン座標をワールド座標に変換
        Vector3 touchPosition = tapEffectCamera.ScreenToWorldPoint(touch.position);
        touchPosition.z = 0;

        //エフェクトをインスタンス化して、TapEffectScene内に送る
        GameObject nowTapEffect = Instantiate(tapEffect, touchPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(nowTapEffect, tapEffectScene);
    }

    /// <summary>
    /// タップしたゲームオブジェクトを返す。UIが優先。
    /// </summary>

    GameObject GetTouchObj(Touch touch)
    {
        GameObject UIObj = GetTouchButton(touch);
        GameObject NotUIObj = GetTouchNotButton(touch);
        if(UIObj && UIObj.GetComponent<Button>()) return UIObj;
        else if(NotUIObj) return NotUIObj;
        else return null;
    }

    /// <summary>
    /// タップしたButtonのゲームオブジェクトを返す　何もタップしていなければnullを返す
    /// </summary>
    /// <param name="touch"></param>
    /// <returns></returns>
    GameObject GetTouchButton(Touch touch)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        GameObject buttonObj = results.Find(g => g.gameObject.GetComponent<Button>() != null).gameObject;
        Debug.Log($"TapUIButton = {buttonObj}");
        return buttonObj;
    }

    /// <summary>
    /// タップしたButtonでないゲームオブジェクトを返す 何もタップしていなければnullを返す
    /// </summary>
    GameObject GetTouchNotButton(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.collider == null) return null;
        Debug.Log($"TapNotUIButton = {hit.collider.gameObject.name}");
        return hit.collider.gameObject;
    }

    /// <summary>
    /// マウス下にあるものに合わせて異なる音を鳴らす
    /// </summary>
    /// <param name="touchObj"></param>
    void SpawnSoundEffect(GameObject touchObj)
    {
        if (touchObj == null) PlaySE_NormalTap();
        else if (touchObj.GetComponent<Button>() != null) PlaySE_Button(touchObj);
        //else if(『ほかの条件』)
        else PlaySE_NormalTap();
    }

    /// <summary>
    /// なんでもない時用の音を鳴らす
    /// </summary>
    void PlaySE_NormalTap()
    {
        soundManager.PlayAudio(soundManager.SE_TAP);
    }

    /// <summary>
    /// ボタンをクリックし時用の音を鳴らす
    /// </summary>
    /// <param name="touchButtonObj"></param>
    void PlaySE_Button(GameObject touchButtonObj)
    {
        soundManager.PlayAudio(soundManager.SE_Button);
        //ボタンごとに異なる条件分岐で音をならすようにする
    }
}
