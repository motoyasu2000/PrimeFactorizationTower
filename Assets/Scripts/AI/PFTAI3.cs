using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PFTAI3 : Agent
{
    static readonly float posXScale = 5.0f; //x方向にどのくらい広く指定できるか(-posXScale~posXScale)
    int generatedPrimeNumber; //生成した素数が何か
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
        //originの情報とconditionの情報を観察
        AddObservationsOderindependent(conditionManager.ConditionNumberDict, sensor);
        AddObservationsOderindependent(originManager.CurrentOriginNumberDict, sensor);

        //ブロックが生成される高さ、生成した素数、その素数の確率を観察として受け取る
        sensor.AddObservation(generateManager.GeneratingPoint.y);
        sensor.AddObservation(generatedPrimeNumber);
        if(primeNumberScores.ContainsKey(0))sensor.AddObservation(primeNumberScores[actions.GeneratedPrimeNumberIndex]);
        //Debug.Log(sensor.m_Observations.Count);
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
        for (int i = 0; i < GameModeManager.Ins.PrimeNumberPool.Length; i++)
        {
            if (i >= 9) Debug.LogError("9個以上の素数を生成しようとしていますが、上書きされます。");
            int primeNumber = GameModeManager.Ins.PrimeNumberPool[i];
            primeNumberScores[primeNumber] = actionBuffers.ContinuousActions[i];
        }
        //全ての素数に対して、-1~1までの値をあてはめ、生成可能なものの中で、最も高いものを生成する
        actions.GenerateBlock(primeNumberScores);

        //0~315°回転する。(0*45°,1*45°,2*45°,...,7*45°)
        generatedPrimeNumber = GameModeManager.Ins.GetPrimeNumberPoolIndex(actions.GeneratedPrimeNumberIndex);
        int spin45Count = actionBuffers.DiscreteActions[generatedPrimeNumber];
        actions.SpinBlock45SeveralTimes(spin45Count);

        //-5.0~5.0
        float blockPosX = actionBuffers.ContinuousActions[9] * 5.0f;
        actions.MoveBlockXAndRelease(blockPosX);

        Debug.Log("PrimeNumberScores: " + string.Join(", ", primeNumberScores));
        Debug.Log("Spin45Count: " + spin45Count);
        Debug.Log("PosX: " + blockPosX);
    }
}
