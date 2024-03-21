using UnityEngine;

//ブロックが地面から落下したかの判定を行うためのクラス。
public class GameOverField : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            gameManager.GameOver(false);
        }
    }
}
