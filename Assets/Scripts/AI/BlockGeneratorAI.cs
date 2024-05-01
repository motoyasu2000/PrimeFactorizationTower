using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BlockGeneratorAI : Agent
{
    static readonly float posXScale = 5.0f; //x�����ɂǂ̂��炢�L���w��ł��邩(-posXScale~posXScale)
    public bool isRunningSingleAction = false;
    bool isRunningActionSequence = false;
    int actionID = 0; //(0:�����A1:��]�A2:����)
    int generatedPrimeNumber; //���������f��������
    int generatedPrimeNumberIndex;
    int spin45Count; //��]��
    float blockPosX; //�����ʒu
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
        Vector3 actionVector = new Vector3(generatedPrimeNumberIndex, spin45Count, blockPosX/posXScale); //�s���̏����ώ@�ɓn���B�����ʒu�̓X�P�[�����O���s��
        sensor.AddObservation(actionVector);
    }

    //����f���ɑΉ�����Observation�̔ԍ��ɂ��̑f���̐�������悤�ɂ����B
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
        GenerateBlock(actionBuffers);
        isRunningSingleAction = false;
    }

    //��������u���b�N��I������1
    void GenerateBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("�u���b�N����");
        for (int i = 0; i < GameModeManager.Ins.PrimeNumberPool.Length; i++)
        {
            if (i >= 9) Debug.LogError("9�ȏ�̑f���𐶐����悤�Ƃ��Ă��܂��B");
            int primeNumber = GameModeManager.Ins.PrimeNumberPool[i];
            primeNumberScores[primeNumber] = actionBuffers.ContinuousActions[i];
        }
        //�S�Ă̑f���ɑ΂��āA-1~1�܂ł̒l�����Ă͂߁A�����\�Ȃ��̂̒��ŁA�ł��������̂𐶐�����
        actions.GenerateBlock(primeNumberScores);
        generatedPrimeNumber = actions.GeneratedPrimeNumber;
        generatedPrimeNumberIndex = GameModeManager.Ins.GetPrimeNumberPoolIndex(generatedPrimeNumber);
        Debug.Log("PrimeNumberScores: " + string.Join(", ", primeNumberScores));
    }
}
