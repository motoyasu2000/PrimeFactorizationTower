using UnityEngine;
using UnityEngine.EventSystems;

public class MyClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // ƒNƒŠƒbƒN‚³‚ê‚½‚Æ‚«‚Ìˆ—‚ğ‚±‚±‚É‘‚­
        Debug.Log("Object was clicked!");
    }
}
