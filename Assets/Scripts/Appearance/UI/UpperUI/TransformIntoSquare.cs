using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    //��ʏ㕔�����ɕ\�������A�����\�ȃu���b�N�̏������L�ڂ������l�𐳕��`�̃u���b�N��ɕ\��������֐��B�F�X�ȉ�ʕ��̂���X�}�z�ɓ��I�ɑΉ����邽�߁B
    public class TransformIntoSquare : MonoBehaviour
    {
        public RectTransform myRectTransform;
        private void Update()
        {
            AdjustSize();
        }
        private void AdjustSize()
        {

            float height = myRectTransform.rect.height;
            myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, height);

        }
        private void OnEnable()
        {
            myRectTransform = GetComponent<RectTransform>();
            AdjustSize();
        }
    }
}