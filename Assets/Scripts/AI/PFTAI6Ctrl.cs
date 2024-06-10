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
        //現在AIのターンで、かつブロックが落下していないなら
        if (TurnMangaer.GetPlayerNames_NowTurn() == GameInfo.AIName && !gameManager.IsDropBlockNowTurn)
        {
            if (!wasActedNowFrame)
            {
                getRewardFlag = false;//AIのターンになったばかりなので、ここではまだ報酬は受け取っていない
                wasActedNowFrame = true;
                StartCoroutine(agent.RunActionSequence());
            }
        }

        //前のターンがAIでゲームが継続していれば、報酬を受け取るべき
        if (TurnMangaer.GetPlayerNames_BeforeTurn() == GameInfo.AIName)
        {
            if (!getRewardFlag)
            {
                //現在のブロック数の二乗/2だけ報酬がもらえる
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
        //条件が変化した→条件達成→報酬を与える
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
