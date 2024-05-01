using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BlockDroperAI : Agent
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
        DropBlock(actionBuffers);
        isRunningSingleAction = false;
    }

    //�����ʒu�����߂�
    void DropBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("�u���b�N����");
        //-5.0~5.0 ���������u���b�N���ƁA��]���ƂɈقȂ�A�N�V������Ԃ𗘗p����
        blockPosX = actionBuffers.ContinuousActions[0] * posXScale;
        actions.MoveBlockXAndRelease(blockPosX);
        Debug.Log("PosX: " + blockPosX);
    }
}
