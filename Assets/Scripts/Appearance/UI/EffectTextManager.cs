using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    //������\��������N���X�B���󂾂Ə����B����freeze�̕\��
    public class EffectTextManager : MonoBehaviour
    {
        TextMeshProUGUI effectText;
        private void Start()
        {
            effectText = GetComponent<TextMeshProUGUI>();
        }

        //�����Ŏ󂯂����������effectText�ɕ\��������B
        public void PrintEffectText(string str)
        {
            effectText.gameObject.SetActive(true);
            effectText.text = str;
            StartCoroutine(HiddenEffectText());
        }

        //�������Ŏ󂯎�����e�L�X�g��������Ŏ󂯎�������Ԍ��effectText�ɕ\��������
        public IEnumerator PrintEffectText(string str, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            effectText.gameObject.SetActive(true);
            effectText.text = str;
            StartCoroutine(HiddenEffectText());
            yield return null;
        }
        //1.2�b�o�ߌ��effectText���\���ɂ���
        IEnumerator HiddenEffectText()
        {
            yield return new WaitForSeconds(1.2f);
            effectText.gameObject.SetActive(false);
            yield return null;
        }
    }
}
