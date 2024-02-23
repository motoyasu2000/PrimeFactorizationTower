using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    //チュートリアルのテキストをトグルするクラス
    public class ToggleExplainText : MonoBehaviour
    {
        int toggleCounter = 0; //toggleした回数を数える変数
        int overCount = -1; //toggleCounterの値がいくつでtoggle数をオーバーするか
        void Awake()
        {
            overCount = transform.childCount;
        }

        //ひとつ前のメニューを非表示にして、現在のメニューを表示する。
        public void Toggle()
        {
            //ひとつ前の画面を非表示にする
            if (toggleCounter >= 1) transform.GetChild(toggleCounter - 1).gameObject.SetActive(false);

            //画面の更新とtoggleCounterの更新と終了
            if (toggleCounter >= overCount)
            {
                gameObject.SetActive(false);
                toggleCounter = 0;
            }
            else
            {
                transform.GetChild(toggleCounter).gameObject.SetActive(true);
                toggleCounter++;
            }
        }

        private void OnEnable()
        {
            Toggle();
        }
    }
}