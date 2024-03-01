using System.Collections;
using UnityEngine;
using TMPro;

namespace UI
{
    //文字を表示させるクラス。現状だと条件達成やfreezeの表示
    public class EffectTextManager : MonoBehaviour
    {
        TextMeshProUGUI effectText;
        const float displayTime = 1.2f;//テキストを表示してから非表示にするまでの時間。
        private void Start()
        {
            effectText = GetComponent<TextMeshProUGUI>();
        }

        //引数で受けった文字列をeffectTextに表示させる。
        public void PrintEffectText(string str)
        {
            effectText.gameObject.SetActive(true);
            effectText.text = str;
            StartCoroutine(HiddenEffectText());
        }

        //第一引数で受け取ったテキストを第二引数で受け取った時間後にeffectTextに表示させる
        public IEnumerator PrintEffectText(string str, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            effectText.gameObject.SetActive(true);
            effectText.text = str;
            StartCoroutine(HiddenEffectText());
            yield return null;
        }
        //1.2秒経過後にeffectTextを非表示にする
        IEnumerator HiddenEffectText()
        {
            yield return new WaitForSeconds(displayTime);
            effectText.gameObject.SetActive(false);
            yield return null;
        }
    }
}
