﻿using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PFTAI2Ctrl : MonoBehaviour
{
    bool getRewardFlag = false;
    int nowCondition;
    int preCondition;
    ConditionManager conditionManager;
    GameObject primeNumberCheckField;
    GameObject completedField;
    PFTAI2 agent;
    GameManager gameManager;
    GameOverManager gameOverManager;

    private void Start()
    {

        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        completedField = GameObject.Find("CompletedField");
        agent = GetComponent<PFTAI2>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameOverManager = GameObject.Find("GameOverManager").GetComponent <GameOverManager>();
        nowCondition = conditionManager.ConditionNumber;
        preCondition = conditionManager.ConditionNumber;
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
                //現在のブロック数の二乗/2だけ報酬がもらえる
                agent.AddReward(CalculateTotalBlocksCount()* CalculateTotalBlocksCount() / 2);
                getRewardFlag = true;
            }
        }

        if (gameOverManager.IsGameOver)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            agent.EndEpisode();
        }
        preCondition = nowCondition;
        nowCondition = conditionManager.ConditionNumber;
        //条件が変化した→条件達成→報酬を与える
        if(preCondition != nowCondition)
        {
            agent.AddReward(1);
        }
    }

    int CalculateTotalBlocksCount()
    {
        return primeNumberCheckField.transform.childCount + completedField.transform.childCount;
    }
}
