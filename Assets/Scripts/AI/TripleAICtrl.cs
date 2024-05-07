using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.MLAgents;

public class TripleAICtrl : MonoBehaviour
{
    bool getRewardFlag;
    bool wasActedNowFrame;
    bool isRunningActionSequence = false;
    int nowCondition;
    int preCondition;
    int nowTotalTurn;
    int preTotalTurn;
    float rewardScale = 0.5f;
    ConditionManager conditionManager;
    Transform parent;
    GameObject primeNumberCheckField;
    GameObject completedField;
    BlockGeneratorAI blockGeneratorAI;
    BlockSpinerAI blockSpinerAI;
    BlockDroperAI blockDroperAI;
    Agent[] agents;
    GameManager gameManager;
    GameOverManager gameOverManager;

    private void Start()
    {
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        completedField = GameObject.Find("CompletedField");
        parent = transform.parent;
        blockGeneratorAI = parent.GetComponentInChildren<BlockGeneratorAI>();
        blockSpinerAI = parent.GetComponentInChildren<BlockSpinerAI>();
        blockDroperAI = parent.GetComponentInChildren<BlockDroperAI>();
        agents = new Agent[3] { blockGeneratorAI, blockSpinerAI, blockDroperAI };
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
        nowCondition = conditionManager.ConditionNumber;
        preCondition = conditionManager.ConditionNumber;
    }
    void Update()
    {
        //現在AIのターンで、かつブロックが落下していないなら
        if (TurnMangaer.GetPlayerNames_NowTurn() == GameInfo.GetAIName && !gameManager.IsDropBlockNowTurn)
        {
            if (!wasActedNowFrame)
            {
                getRewardFlag = false;//AIのターンになったばかりなので、ここではまだ報酬は受け取っていない
                wasActedNowFrame = true;
                StartCoroutine(RunActionSequence());
            }
        }

        //前のターンがAIでゲームが継続していれば、報酬を受け取るべき
        if (TurnMangaer.GetPlayerNames_BeforeTurn() == GameInfo.GetAIName)
        {
            if (!getRewardFlag)
            {
                //現在のブロック数の二乗/2だけ報酬がもらえる
                float totalBlocks = CalculateTotalBlocksCount();
                float reward = totalBlocks * totalBlocks * rewardScale;

                foreach(var agent  in agents)
                {
                    agent.AddReward(reward);
                }
                getRewardFlag = true;
            }
        }

        if (gameOverManager.IsGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            foreach (var agent in agents)
            {
                agent.EndEpisode();
            }
        }

        preCondition = nowCondition;
        nowCondition = conditionManager.ConditionNumber;
        //条件が変化した→条件達成→報酬を与える
        if (preCondition != nowCondition)
        {
            foreach (var agent in agents)
            {
                agent.AddReward(1);
            }
        }

        preTotalTurn = nowTotalTurn;
        nowTotalTurn = TurnMangaer.GetTotalTurn();
        if (preTotalTurn != nowTotalTurn)
        {
            wasActedNowFrame = false;
        }
    }
    int CalculateTotalBlocksCount()
    {
        return primeNumberCheckField.transform.childCount + completedField.transform.childCount;
    }
    public IEnumerator RunActionSequence()
    {
        if (isRunningActionSequence)
        {
            Debug.LogWarning("短期間でアクションシーケンスが連続で呼び出されました。");
            yield break;
        }

        isRunningActionSequence = true;

        foreach (var agent in agents)
        {
            agent.RequestDecision();

            if (agent is BlockGeneratorAI blockGeneratorAI)
            {
                blockGeneratorAI.isRunningSingleAction = true;
                while (blockGeneratorAI.isRunningSingleAction)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }

            if (agent is BlockSpinerAI blockSpinerAI)
            {
                blockSpinerAI.isRunningSingleAction = true;
                while (blockSpinerAI.isRunningSingleAction)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }

            if (agent is BlockDroperAI blockDroperAI)
            {
                blockDroperAI.isRunningSingleAction = true;
                while (blockDroperAI.isRunningSingleAction)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }
        }
        isRunningActionSequence = false;
    }
}
