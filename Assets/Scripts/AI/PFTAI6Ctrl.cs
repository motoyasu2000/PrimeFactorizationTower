using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PFTAI6Ctrl : MonoBehaviour
{
    bool getRewardFlag;
    bool wasActedNowFrame;
    int nowCondition;
    int preCondition;
    int nowTotalTurn;
    int preTotalTurn;
    ConditionManager conditionManager;
    GameObject primeNumberCheckField;
    GameObject completedField;
    PFTAI6 agent;
    GameManager gameManager;
    GameOverManager gameOverManager;

    private void Start()
    {
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        completedField = GameObject.Find("CompletedField");
        agent = GetComponent<PFTAI6>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
        nowCondition = conditionManager.ConditionNumber;
        preCondition = conditionManager.ConditionNumber;
    }
    void Update()
    {
        //����AI�̃^�[���ŁA���u���b�N���������Ă��Ȃ��Ȃ�
        if (TurnMangaer.GetPlayerNames_NowTurn() == GameInfo.AIName && !gameManager.IsDropBlockNowTurn)
        {
            if (!wasActedNowFrame)
            {
                getRewardFlag = false;//AI�̃^�[���ɂȂ����΂���Ȃ̂ŁA�����ł͂܂���V�͎󂯎���Ă��Ȃ�
                wasActedNowFrame = true;
                StartCoroutine(agent.RunActionSequence());
            }
        }

        //�O�̃^�[����AI�ŃQ�[�����p�����Ă���΁A��V���󂯎��ׂ�
        if (TurnMangaer.GetPlayerNames_BeforeTurn() == GameInfo.AIName)
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

        preTotalTurn = nowTotalTurn;
        nowTotalTurn = TurnMangaer.GetTotalTurn();
        if(preTotalTurn != nowTotalTurn)
        {
            wasActedNowFrame = false;
        }
    }

    int CalculateTotalBlocksCount()
    {
        return primeNumberCheckField.transform.childCount + completedField.transform.childCount;
    }
}
