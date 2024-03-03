using UnityEngine;

//ブロックを積み木するための土台となる地面を生成するためのクラス。地面を真っ平にしてしまうと。2,5だけひたすら積んでく動きが強すぎになってしまう。
public class GroundGenerator : MonoBehaviour
{
    GameObject groundToken;
    private void Start()
    {
        groundToken = (GameObject)Resources.Load("GroundToken");

        for(int i=-3; i<=3; i++)
        {
            //生成
            GameObject newGround = Instantiate(groundToken, new Vector3(i, 0, 0), Quaternion.identity);

            //変形
            float randomHeight = Random.Range(0.5f, 1.5f);
            newGround.transform.localScale = new Vector3(1, randomHeight, 1);

            //回転
            float randomSpinAngle = Random.Range(-20f, 20f);
            newGround.transform.Rotate(new Vector3(0, 0, randomSpinAngle)); 

            //親とタグの設定
            newGround.transform.parent = transform;
            newGround.tag = "Ground";
        }
    }
}
