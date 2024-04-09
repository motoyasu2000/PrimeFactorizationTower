using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PFTAI3 : Agent
{
    //���݂̏󋵂őI�Ԃׂ��f���̃X�R�A�B�������Q�[�����̐����őS�ẴL�[�������ł���킯�ł͂Ȃ��̂ŁA�����ł��钆�ōł��X�R�A�̍������̂�I�����郍�W�b�N�ɂ���B
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

    //����f���ɑΉ�����ꏊ�ɂ��̑f���̐�������悤�ɂ����B
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

    //actionBuffers0~8�A�f���̑I��  actionBuffers9�A�����ʒu�̑I��
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        for (int i = 0; i < GameModeManager.Ins.PrimeNumberPool.Length; i++)
        {
            if (i >= 9) Debug.LogError("9�ȏ�̑f�����w�K���悤�Ƃ��Ă��܂����A�㏑������܂��B");
            int primeNumber = GameModeManager.Ins.PrimeNumberPool[i];
            primeNumberScores[primeNumber] = actionBuffers.ContinuousActions[i];
        }
        //-1.0~1.0
        actions.GenerateBlock(primeNumberScores);

        //0~7 (����)
        int spin45Count = actionBuffers.DiscreteActions[GameModeManager.Ins.GetPrimeNumberPoolIndex(actions.GeneratedPrimeNumber)];
        actions.SpinBlock45SeveralTimes(spin45Count);

        //-5.0~5.0
        float blockPosX = actionBuffers.ContinuousActions[9] * 5;
        actions.MoveBlockXAndRelease(blockPosX);
    }
}