using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PrimeNumberBlockCtrl : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    protected GameObject selfPrefab;
    protected Vector3 initialPosition;

    public abstract void SetSelfPrefab();
    public void OnPointerDown(PointerEventData eventData)
    {
        DuplicatePrimeNumberBlock();
        initialPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void DuplicatePrimeNumberBlock()
    {
        Instantiate(selfPrefab, transform.position, Quaternion.identity);
    }

}
