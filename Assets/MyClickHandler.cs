using UnityEngine;
using UnityEngine.EventSystems;

public class MyClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // �N���b�N���ꂽ�Ƃ��̏����������ɏ���
        Debug.Log("Object was clicked!");
    }
}
