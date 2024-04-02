using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PFTAI1Ctrl : MonoBehaviour
{
    bool getRewardFlag = false;
    GameObject primeNumberCheckField;
    GameObject completedField;
    PFTAI1 agent;
    GameManager gameManager;
    GameOverManager gameOverManager;

    private void Start()
    {
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        completedField = GameObject.Find("CompletedField");
        agent = GetComponent<PFTAI1>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameOverManager = GameObject.Find("GameOverManager").GetComponent <GameOverManager>();
    }
    void Update()
    {   
        //現在AIのターンで、かつブロックが落下していないなら
        if(TurnMangaer.GetPlayerNames_NowTurn() == GameInfo.GetAIName && !gameManager.IsDropBlockNowTurn)
        {
            getRewardFlag = false;//AIのターンになったばかりなので、ここではまだ報酬は受け取っていない
            agent.RequestDecision();
        }

        //前のターンがAIでゲームが継続していれば、報酬を受け取るべき
        if(TurnMangaer.GetPlayerNames_BeforeTurn() == GameInfo.GetAIName)
        {
            if (!getRewardFlag)
            {
                //現在のブロック数だけ報酬がもらえる
                agent.AddReward(CalculateTotalBlocksCount());
                getRewardFlag = true;
            }
        }

        if (gameOverManager.IsGameOver)
        {
            //行動のタイミングでないため、正確に学習できるか不明
            agent.AddReward(-1.0f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            agent.EndEpisode();
        }
    }

    int CalculateTotalBlocksCount()
    {
        return primeNumberCheckField.transform.childCount + completedField.transform.childCount;
    }
}
