using UnityEngine;
using UnityEngine.UI;
using TMPro;

////////////////////////////////////////////
//�������p�̃N���X�@����̃Q�[���ɂ͗��p���Ȃ��B///
////////////////////////////////////////////
namespace UI
{
    public class UIDetailsDisplay : MonoBehaviour
    {
        public RectTransform targetUIElement;
        public TextMeshProUGUI detailsText;
        void Update()
        {
            Vector2 size = targetUIElement.rect.size;
            Vector2 anchorSize = targetUIElement.sizeDelta;
            detailsText.text = "UI Size: " + size.ToString() + "\nAnchor Size: " + anchorSize.ToString();
        }
    }
}
