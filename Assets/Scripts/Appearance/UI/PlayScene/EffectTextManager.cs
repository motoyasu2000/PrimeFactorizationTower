using System.Collections;
using UnityEngine;
using TMPro;

namespace UI
{
    /// <summary>
    /// 何らかの出来事が起こった時に文字を表示させるクラス。現状だと条件達成やfreezeの表示
    /// </summary>
    public class EffectTextManager : MonoBehaviour
    {
        TextMeshProUGUI effectText;

        private void Start()
        {
            effectText = GetComponent<TextMeshProUGUI>();
        }

        //引数で受けった文字列をeffectTextに表示させる。
        public void DisplayEffectText(string displayText, float durationTime, Color displayColor)
        {
            effectText.gameObject.SetActive(true);
            effectText.text = displayText;
            effectText.color = displayColor;    
            StartCoroutine(HiddenEffectText(durationTime));
        }

        //第一引数で受け取ったテキストを第二引数で受け取った時間後にeffectTextに表示させる
        public IEnumerator DisplayEffectText(string displayText, float durationTime, float displayStartTime, Color displayColor)
        {
            yield return new WaitForSeconds(displayStartTime);
            DisplayEffectText(displayText, durationTime, displayColor);
            yield return null;
        }
        //durationTime秒経過後にeffectTextを非表示にする
        IEnumerator HiddenEffectText(float durationTime)
        {
            yield return new WaitForSeconds(durationTime);
            effectText.gameObject.SetActive(false);
            yield return null;
        }
    }
}
