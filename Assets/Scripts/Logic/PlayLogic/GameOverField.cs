using UnityEngine;

/// <summary>
/// ブロックが地面から落下したかの判定を行うためのクラス。
/// </summary>
public class GameOverField : MonoBehaviour
{
    GameOverManager gameOverManager;
    private void Start()
    {
        gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PrimeNumberBlock"))
        {
            gameOverManager.SetGameOverReason(GameOverManager.GameOverReason.DropDown);
            gameOverManager.GameOver();
        }
    }
}
