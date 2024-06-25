using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PFTAIMASK : Agent
{
    static readonly float posXScale = 5.0f; //x方向にどのくらい広く指定できるか(-posXScale~posXScale)
    int generatedPrimeNumber; //生成した素数が何か
    int generatedPrimeNumberIndex;
    //現在の状況で選ぶべき素数のスコア。ただしゲーム側の制限で全てのキーが生成できるわけではないので、生成できる中で最もスコアの高いものを選択するロジックにする。
    Dictionary<int, float> primeNumberScores = new Dictionary<int, float>();
    AIActions actions;
    OriginManager originManager;
    ConditionManager conditionManager;
    SingleBlockManager generateManager;
    public override void Initialize()
    {
        actions = transform.parent.GetComponent<AIActions>();
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        conditionManager = GameObject.Find("ConditionManager").GetComponent<ConditionManager>();
        generateManager = GameObject.Find("PrimeNumberGeneratingPoint").GetComponent<SingleBlockManager>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        AddObservationsOderindependent(originManager.CurrentOriginNumberDict, sensor);
        AddObservationsOderindependent(conditionManager.ConditionNumberDict, sensor);
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

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        for (int i = 0; i < GameModeManager.Ins.PrimeNumberPool.Length; i++)
        {
            if (!originManager.GetCurrentOriginSet().Contains(i))
            {
                actionMask.SetActionEnabled(0, i, false);
                //Debug.Log($"{i}はfalse");
            }

        }
    }

    //actionBuffers0~8、素数の選択  actionBuffers9、落下位置の選択
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //全ての素数に対して、-1~1までの値をあてはめ、生成可能なものの中で、最も高いものを生成する
        int choosePrimeNumberIndex = actionBuffers.DiscreteActions[0];
        //Debug.Log(choosePrimeNumberIndex);
        actions.GenerateBlock(GameModeManager.Ins.PrimeNumberPool[choosePrimeNumberIndex]);

        //0~315°回転する。(0*45°,1*45°,2*45°,...,7*45°)
        int spin45Count = actionBuffers.DiscreteActions[1];
        actions.SpinBlock45SeveralTimes(spin45Count);

        //-5.0~5.0
        float blockPosX = actionBuffers.ContinuousActions[0] * posXScale;
        actions.MoveBlockXAndRelease(blockPosX);

        //Debug.Log("ChoosePrimeNumberIndex: " + string.Join(", ", choosePrimeNumberIndex));
        //Debug.Log("Spin45Count: " + spin45Count);
        //Debug.Log("PosX: " + blockPosX);
    }
}
