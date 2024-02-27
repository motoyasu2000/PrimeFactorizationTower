using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    //画面下部にあるボタンUIの数値を設定するクラス
    public class ButtonGenerator : MonoBehaviour
    {
        readonly float[] splitPoints = { 0, 0.33f, 0.66f, 1 };
        GameObject buttonArea;
        GameObject buttonPrefab;
        GameModeManager gameModeManager;
        void Awake()
        {
            buttonArea = GameObject.Find("ButtonArea");
            buttonPrefab = Resources.Load("ButtonPrefab") as GameObject;
            gameModeManager = GameModeManager.GameModemanagerInstance;
            for(int i=0; i<gameModeManager.PrimeNumberPool.Length; i++)
            {
                //左端(もしくは下端)を基準にしたインデックス
                int xi_left = i % 3;
                int yi_left = 2 - (i / 3); //今回のゲームだとy座標が高いほど小さい数値となるなので、上から設置するために逆順にする。

                GameObject newButton = Instantiate(buttonPrefab);
                newButton.transform.SetParent(buttonArea.transform);

                RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
                buttonRectTransform.anchorMin = new Vector2(splitPoints[xi_left], splitPoints[yi_left]);
                buttonRectTransform.anchorMax = new Vector2(splitPoints[xi_left + 1], splitPoints[yi_left + 1]);

                buttonRectTransform.offsetMin = Vector2.zero;
                buttonRectTransform.offsetMax = Vector2.zero;

                int[] myPrimeNumberPool = gameModeManager.GetGameModeMatchDifficultyLevel();
                if(i < myPrimeNumberPool.Length)newButton.GetComponent<BlockGenerator>().SetPrimeNumber(myPrimeNumberPool[i]);
            }
        }
    }
}
