using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineViewer : MonoBehaviour
{
    LineRenderer lineRenderer;
    Collider2D myCollider;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        //�������g�ƏՓ˂��Ē������`�悳��Ȃ��Ȃ��Ă��܂����߈�U�R���C�_�[�������Ă����A�����āA�N���b�N�����Ƃ��ɃR���C�_�[�̔���𕜊������A���̃R���|�[�l���g����������B
        myCollider = GetComponent<Collider2D>(); 
        myCollider.enabled = false;
    }
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit.collider != null)
        {
            //Debug.Log(hit.point);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
    }
}
