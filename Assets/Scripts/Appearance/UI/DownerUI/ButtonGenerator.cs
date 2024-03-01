using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    //画面下部にあるボタンUIの数値を設定するクラス
    public class ButtonGenerator : MonoBehaviour
    {
        static readonly float xScale = 0.97f;
        static readonly float yScale = 0.93f;
        static readonly int splitCount = 3;
        static readonly float[] splitPoints = { 0, 0.33f, 0.66f, 1 };
        GameObject buttonArea;
        GameObject buttonPrefab;
        GameModeManager gameModeManager;
        void Awake()
        {
            buttonArea = GameObject.Find("ButtonArea");
            buttonPrefab = Resources.Load("ButtonPrefab") as GameObject;
            gameModeManager = GameModeManager.GameModemanagerInstance;
            int[] myPrimeNumberPool = gameModeManager.GetGameModeMatchDifficultyLevel();
            for (int i=0; i<myPrimeNumberPool.Length; i++)
            {
                
                //左端(もしくは下端)を基準にしたインデックス
                int xi_left = i % splitCount;
                int yi_left = (splitCount-1) - (i / splitCount); //今回のゲームだとy座標が高いほど小さい数値となるなので、上から設置するために逆順にする。

                //ボタンを生成し、複数のボタンを子オブジェクトとして持つようのゲームオブジェクトであるButtonArea内に移動
                GameObject newButton = Instantiate(buttonPrefab);
                newButton.transform.SetParent(buttonArea.transform);

                //ボタンの位置や大きさをビューポート座標で指定(3*3)
                RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
                buttonRectTransform.anchorMin = new Vector2(splitPoints[xi_left], splitPoints[yi_left]);
                buttonRectTransform.anchorMax = new Vector2(splitPoints[xi_left + 1], splitPoints[yi_left + 1]);

                //上で指定したアンカーと誤差を無くす
                buttonRectTransform.offsetMin = Vector2.zero;
                buttonRectTransform.offsetMax = Vector2.zero;

                //大きさの調整と、z座標の調整
                buttonRectTransform.localScale = new Vector3(xScale, yScale, 1);
                buttonRectTransform.anchoredPosition3D = Vector3.zero;

                //ボタンに素数を割り当てる
                newButton.GetComponent<BlockGenerator>().SetPrimeNumber(myPrimeNumberPool[i]);
            }
        }
    }
}
