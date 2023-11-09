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
        //自分自身と衝突して直線が描画されなくなってしまうため一旦コライダーを消しておく、そして、クリックしたときにコライダーの判定を復活させ、このコンポーネントも消去する。
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
