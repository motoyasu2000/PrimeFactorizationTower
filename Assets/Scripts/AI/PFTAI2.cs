using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;
using Unity.Sentis;

public class PFTAI2 : Agent
{
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
        AddObservationsOderindependent(conditionManager.ConditionNumberDict, sensor);
        AddObservationsOderindependent(originManager.CurrentOriginNumberDict, sensor);
        sensor.AddObservation(generateManager.GeneratingPoint);
    }

    //ある素数に対応する場所にその素数の数を入れるようにした。
    void AddObservationsOderindependent(Dictionary<int,int> dict, VectorSensor sensor)
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
        for (int i=0; i< GameModeManager.Ins.PrimeNumberPool.Length; i++)
        {
            if (i >= 9) Debug.LogError("9個以上の素数を学習しようとしていますが、上書きされます。");
            int primeNumber = GameModeManager.Ins.PrimeNumberPool[i];
            primeNumberScores[primeNumber] = actionBuffers.ContinuousActions[i];
        }
        //-1.0~1.0
        actions.GenerateBlock(primeNumberScores);

        //0~7 (整数)
        int spin45Count = actionBuffers.DiscreteActions[0];
        actions.SpinBlock45SeveralTimes(spin45Count);

        //-5.0~5.0
        float blockPosX = actionBuffers.ContinuousActions[9] * 5;
        actions.MoveBlockXAndRelease(blockPosX);
    }


}
