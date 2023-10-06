using UnityEngine;
using UnityEngine.EventSystems;

public class MyClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // クリックされたときの処理をここに書く
        Debug.Log("Object was clicked!");
    }
}
