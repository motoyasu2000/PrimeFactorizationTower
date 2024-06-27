using UnityEngine;
using Common;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 画面下部にあるブロック生成ボタンを生成するクラス
    /// </summary>
    public class ButtonGenerator : MonoBehaviour
    {
        static readonly int splitCount = 3;
        static readonly float xScale = 0.97f;
        static readonly float yScale = 0.93f;
        static readonly float[] splitPoints = Helper.CalculateSplitAnchorPoints(splitCount);
        GameObject buttonArea;
        GameObject buttonPrefab;
        GameModeManager gameModeManager;
        void Awake()
        {
            buttonArea = GameObject.Find("BlockGanerateButtonArea");
            buttonPrefab = Resources.Load("ButtonPrefab") as GameObject;
            gameModeManager = GameModeManager.Ins;
            int[] nowPrimeNumberPool = gameModeManager.GetPrimeWithDifficultyLevel();
            for (int i=0; i<nowPrimeNumberPool.Length; i++)
            {
                //左端(もしくは上端)を基準にしたインデックス
                int xi_left = i % splitCount;
                int yi_up = (splitCount-1) - (i / splitCount); //今回のゲームだとy座標が高いほど小さい数値となるなので、上から設置するために逆順にする。

                //ボタンを生成し、複数のボタンを子オブジェクトとして持つようのゲームオブジェクトであるButtonArea内に移動
                GameObject newButton = Instantiate(buttonPrefab);
                newButton.transform.SetParent(buttonArea.transform);

                //ボタンの位置や大きさをビューポート座標で指定(3*3)
                RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
                buttonRectTransform.anchorMin = new Vector2(splitPoints[xi_left], splitPoints[yi_up]);
                buttonRectTransform.anchorMax = new Vector2(splitPoints[xi_left + 1], splitPoints[yi_up + 1]);

                //上で指定したアンカーとの誤差を無くす
                buttonRectTransform.offsetMin = Vector2.zero;
                buttonRectTransform.offsetMax = Vector2.zero;

                //大きさの調整と、z座標の調整
                buttonRectTransform.localScale = new Vector3(xScale, yScale, 1);
                buttonRectTransform.anchoredPosition3D = Vector3.zero;

                //ボタンのテキストと生成するブロックに素数を割り当てる。
                int prime = nowPrimeNumberPool[i];
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = prime.ToString(); 
                newButton.GetComponent<BlockGenerator>().SetPrimeNumber(prime);


            }
        }
    }
}
