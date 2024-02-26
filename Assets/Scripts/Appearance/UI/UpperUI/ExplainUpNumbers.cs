using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    //チュートリアルでのUIの説明の際に、今説明しているUIがを点滅させる関数。これにより今どこの説明をしているのかがわかりやすくなる。
    public class ExplainUpNumbers : MonoBehaviour
    {
        GameObject nowUpNumber;
        GameObject nextUpNumber;
        GameObject conditionNumber;
        TextMeshProUGUI nowUpNumberText;
        TextMeshProUGUI nextUpNumberText;
        TextMeshProUGUI conditionNumberText;
        TextMeshProUGUI nowText;
        Color startColor;
        float timeCounter = 0;
        void Start()
        {
            nowUpNumber = GameObject.Find("NowUpNumber");
            nextUpNumber = GameObject.Find("NextUpNumber");
            conditionNumber = GameObject.Find("ConditonNumber");
            nowUpNumberText = nowUpNumber.GetComponent<TextMeshProUGUI>();
            nextUpNumberText = nextUpNumber.GetComponent<TextMeshProUGUI>();
            conditionNumberText = conditionNumber.GetComponent<TextMeshProUGUI>();

            //今説明しているテキストのゲームオブジェクト名に合わせてどのUIを点滅させるのかを選択する。
            if (gameObject.name == "ExplainNowUpNumber") nowText = nowUpNumberText;
            if (gameObject.name == "ExplainNextUpNumber") nowText = nextUpNumberText;
            if (gameObject.name == "ExplainConditionNumber") nowText = conditionNumberText;
            startColor = nowText.color;
        }

        void Update()
        {
            timeCounter += Time.deltaTime * 1.4f;
            if (timeCounter > 1) timeCounter = 0;
            nowText.color = new Color(timeCounter, timeCounter, timeCounter);
        }

        private void OnDisable()
        {
            nowText.color = startColor;
        }
    }
}