using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//横幅750を基準とし、現在の画面幅では、どのくらいUIを拡張するのかを計算するクラス
namespace UI
{
    public class CanvasManager : MonoBehaviour
    {
        const int originWidth = 750;
        int nowWidth;
        static float nowScaleFactor;
        public static float NowScaleFactor => nowScaleFactor;
        void Awake()
        {
            //nowWidth = Screen.width;
            //nowScaleFactor = nowWidth / originWidth;
            //GetComponent<Canvas>().scaleFactor = nowScaleFactor;
            nowScaleFactor = GetComponent<Canvas>().scaleFactor;
        }
    }
}
