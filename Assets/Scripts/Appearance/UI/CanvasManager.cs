using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����750����Ƃ��A���݂̉�ʕ��ł́A�ǂ̂��炢UI���g������̂����v�Z����N���X
namespace UI
{
    public class CanvasManager : MonoBehaviour
    {
        const int originWidth = 750;
        int nowWidth;
        static int nowScaleFactor;
        public static int NowScaleFactor => nowScaleFactor;
        void Awake()
        {
            nowWidth = Screen.width;
            nowScaleFactor = nowWidth / originWidth;
            GetComponent<Canvas>().scaleFactor = nowScaleFactor;
        }
    }
}
