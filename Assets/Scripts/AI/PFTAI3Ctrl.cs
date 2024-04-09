using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PFTAI3Ctrl : MonoBehaviour
{
    bool getRewardFlag = false;
    int nowCondition;
    int preCondition;
    ConditionManager conditionManager;
    GameObject primeNumberCheckField;
    GameObject completedField;
    PFTAI3 agent;
    GameManager gameManager;
    GameOverManager gameOverManager;

    private void Start()
    {
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        completedField = GameObject.Find("CompletedField");
        agent = GetComponent<PFTAI3>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
        nowCondition = conditionManager.ConditionNumber;
        preCondition = conditionManager.ConditionNumber;
    }
    void Update()
    {
        //����AI�̃^�[���ŁA���u���b�N���������Ă��Ȃ��Ȃ�
        if (TurnMangaer.GetPlayerNames_NowTurn() == GameInfo.GetAIName && !gameManager.IsDropBlockNowTurn)
        {
            getRewardFlag = false;//AI�̃^�[���ɂȂ����΂���Ȃ̂ŁA�����ł͂܂���V�͎󂯎���Ă��Ȃ�
            agent.RequestDecision();
        }

        //�O�̃^�[����AI�ŃQ�[�����p�����Ă���΁A��V���󂯎��ׂ�
        if (TurnMangaer.GetPlayerNames_BeforeTurn() == GameInfo.GetAIName)
        {
            if (!getRewardFlag)
            {
                //���݂̃u���b�N���̓��/2������V�����炦��
                agent.AddReward(CalculateTotalBlocksCount() * CalculateTotalBlocksCount() / 2);
                getRewardFlag = true;
            }
        }

        if (gameOverManager.IsGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            agent.EndEpisode();
        }
        preCondition = nowCondition;
        nowCondition = conditionManager.ConditionNumber;
        //�������ω������������B������V��^����
        if (preCondition != nowCondition)
        {
            agent.AddReward(1);
        }
    }

    int CalculateTotalBlocksCount()
    {
        return primeNumberCheckField.transform.childCount + completedField.transform.childCount;
    }
}
