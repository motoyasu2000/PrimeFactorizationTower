using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BlockGeneratorAI : Agent
{
    static readonly float posXScale = 5.0f; //x方向にどのくらい広く指定できるか(-posXScale~posXScale)
    public bool isRunningSingleAction = false;
    bool isRunningActionSequence = false;
    int actionID = 0; //(0:生成、1:回転、2:落下)
    int generatedPrimeNumber; //生成した素数が何か
    int generatedPrimeNumberIndex;
    int spin45Count; //回転数
    float blockPosX; //落下位置
    //現在の状況で選ぶべき素数のスコア。ただしゲーム側の制限で全てのキーが生成できるわけではないので、生成できる中で最もスコアの高いものを選択するロジックにする。
    Dictionary<int, float> primeNumberScores = new Dictionary<int, float>();
    AIActions actions;
    OriginManager originManager;
    ConditionManager conditionManager;
    SingleGenerateManager generateManager;
    public override void Initialize()
    {
        actions = transform.parent.GetComponent<AIActions>();
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        generateManager = GameObject.Find("PrimeNumberGeneratingPoint").GetComponent<SingleGenerateManager>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 actionVector = new Vector3(generatedPrimeNumberIndex, spin45Count, blockPosX/posXScale); //行動の情報を観察に渡す。落下位置はスケーリングを行う
        sensor.AddObservation(actionVector);
    }

    //ある素数に対応するObservationの番号にその素数の数を入れるようにした。
    void AddObservationsOderindependent(Dictionary<int, int> dict, VectorSensor sensor)
    {
        foreach (int prime in GameModeManager.Ins.PrimeNumberPool)
        {
            if (dict.ContainsKey(prime))
            {
                sensor.AddObservation(prime);
            }
            else
            {
                sensor.AddObservation(0);
            }
        }
    }

    //actionBuffers0~8、素数の選択  actionBuffers9、落下位置の選択
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        GenerateBlock(actionBuffers);
        isRunningSingleAction = false;
    }

    //生成するブロックを選択する1
    void GenerateBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("ブロック生成");
        for (int i = 0; i < GameModeManager.Ins.PrimeNumberPool.Length; i++)
        {
            if (i >= 9) Debug.LogError("9個以上の素数を生成しようとしています。");
            int primeNumber = GameModeManager.Ins.PrimeNumberPool[i];
            primeNumberScores[primeNumber] = actionBuffers.ContinuousActions[i];
        }
        //全ての素数に対して、-1~1までの値をあてはめ、生成可能なものの中で、最も高いものを生成する
        actions.GenerateBlock(primeNumberScores);
        generatedPrimeNumber = actions.GeneratedPrimeNumber;
        generatedPrimeNumberIndex = GameModeManager.Ins.GetPrimeNumberPoolIndex(generatedPrimeNumber);
        Debug.Log("PrimeNumberScores: " + string.Join(", ", primeNumberScores));
    }
}
