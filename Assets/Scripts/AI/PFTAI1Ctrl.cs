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
        //����AI�̃^�[���ŁA���u���b�N���������Ă��Ȃ��Ȃ�
        if(TurnMangaer.GetPlayerNames_NowTurn() == GameInfo.GetAIName && !gameManager.IsDropBlockNowTurn)
        {
            getRewardFlag = false;//AI�̃^�[���ɂȂ����΂���Ȃ̂ŁA�����ł͂܂���V�͎󂯎���Ă��Ȃ�
            agent.RequestDecision();
        }

        //�O�̃^�[����AI�ŃQ�[�����p�����Ă���΁A��V���󂯎��ׂ�
        if(TurnMangaer.GetPlayerNames_BeforeTurn() == GameInfo.GetAIName)
        {
            if (!getRewardFlag)
            {
                //���݂̃u���b�N��������V�����炦��
                agent.AddReward(CalculateTotalBlocksCount());
                getRewardFlag = true;
            }
        }

        if (gameOverManager.IsGameOver)
        {
            //�s���̃^�C�~���O�łȂ����߁A���m�Ɋw�K�ł��邩�s��
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
