using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// チュートリアルでのUIの説明の際に、今どのUIについて説明しているか、説明中のUIを点滅させるクラス。
    /// これにより、視覚的な理解ができるようにする。
    /// 対応するUIを点滅させるべき時に、そのUIを点滅させるためのこのスクリプトがアタッチされたゲームオブジェクトのenableがtrueになる。
    /// </summary>
    public class ExplainUpNumbers : MonoBehaviour
    {
        static readonly float blinkSpeedCoefficient = 1.4f; // 時間当たりにどのくらい点滅するのかを調整するための値
        float timeCounter = 0;
        Color startColor;
        GameObject originNumber;
        GameObject nextOriginNumber;
        GameObject conditionNumber;
        TextMeshProUGUI nowUpNumberText;
        TextMeshProUGUI nextUpNumberText;
        TextMeshProUGUI conditionNumberText;
        TextMeshProUGUI nowText;

        void Start()
        {
            originNumber = GameObject.Find("OriginNumberText");
            nextOriginNumber = GameObject.Find("NextOriginNumberText");
            conditionNumber = GameObject.Find("ConditonNumberText");
            nowUpNumberText = originNumber.GetComponent<TextMeshProUGUI>();
            nextUpNumberText = nextOriginNumber.GetComponent<TextMeshProUGUI>();
            conditionNumberText = conditionNumber.GetComponent<TextMeshProUGUI>();

            //今説明しているテキストのゲームオブジェクト名に合わせてどのUIを点滅させるのかを選択する。
            if (gameObject.name == "ExplainNowUpNumber") nowText = nowUpNumberText;
            if (gameObject.name == "ExplainNextUpNumber") nowText = nextUpNumberText;
            if (gameObject.name == "ExplainConditionNumber") nowText = conditionNumberText;
            startColor = nowText.color;
        }

        void Update()
        {
            timeCounter += Time.deltaTime * blinkSpeedCoefficient;
            if (timeCounter > 1) timeCounter = 0;
            nowText.color = new Color(timeCounter, timeCounter, timeCounter);
        }

        private void OnDisable()
        {
            nowText.color = startColor;
        }
    }
}