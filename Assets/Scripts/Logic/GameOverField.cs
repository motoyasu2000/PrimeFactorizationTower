using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            gameManager.GameOver();
        }
    }
}
