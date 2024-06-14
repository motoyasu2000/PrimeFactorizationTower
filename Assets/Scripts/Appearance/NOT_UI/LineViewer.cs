using UnityEngine;

/// <summary>
/// ブロックの落下地点がわかりやすくなるように、y軸下方向に線を伸ばすクラス。各ブロックにアタッチされている。
/// </summary>
public class LineViewer : MonoBehaviour
{
    LineRenderer lineRenderer;
    Collider2D myCollider;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        //自分自身と衝突して直線が描画されなくなってしまうため一旦コライダーを消しておく、そして、クリックしたときにコライダーの判定を復活させ、LineViewerコンポーネントも消去する。
        myCollider = GetComponent<Collider2D>(); 
        myCollider.enabled = false;
    }

    private void Update()
    {
        //このスクリプトがアタッチされたゲームオブジェクトから、下方向に伸ばしたRaycastHitが衝突したポイントまで、直線を伸ばす。
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit.collider != null)
        {
            //Debug.Log(hit.point);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
    }
}
