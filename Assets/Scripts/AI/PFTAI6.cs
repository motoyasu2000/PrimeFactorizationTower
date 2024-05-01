using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PFTAI6 : Agent
{
    static readonly float posXScale = 5.0f; //x�����ɂǂ̂��炢�L���w��ł��邩(-posXScale~posXScale)
    static readonly int numOfSpinPattern = 8; //��]�̎��
    bool isRunningSingleAction = false;
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
        Debug.Log("�A�N�V�����Ăяo���B");
        if (actionID == 0)
        {
            GenerateBlock(actionBuffers);
        }
        else if (actionID == 1)
        {
            SpinBlock(actionBuffers);
        }
        else if (actionID == 2)
        {
            DropBlock(actionBuffers);
        }
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

    //��]�p�x�����߂�
    void SpinBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("�u���b�N��]");
        //0~315����]����B(0*45��,1*45��,2*45��,...,7*45��) ���������u���b�N���ƂɈقȂ��]�̃A�N�V������Ԃ𗘗p�B
        spin45Count = actionBuffers.DiscreteActions[0];
        actions.SpinBlock45SeveralTimes(spin45Count);
        Debug.Log("Spin45Count: " + spin45Count);
    }

    //�����ʒu�����߂�
    void DropBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("�u���b�N����");
        //-5.0~5.0 ���������u���b�N���ƁA��]���ƂɈقȂ�A�N�V������Ԃ𗘗p����
        blockPosX = actionBuffers.ContinuousActions[GameModeManager.Ins.PrimeNumberPool.Length] * posXScale; //0~GameModeManager.Ins.PrimeNumberPool.Length-1�̓u���b�N�̑I���Ɏg���Ă���B
        actions.MoveBlockXAndRelease(blockPosX);
        Debug.Log("PosX: " + blockPosX);
    }

    //�A�N�V�����𓯎��ɍs���̂ł͂Ȃ��A�������ώ@����]���ώ@���������ώ@�ƁA���ׂẴA�N�V�����ɊԊu��݂��Ċώ@���s�����Ƃɂ���āA�������ꂽ�u���b�N�ɍ��킹����]�A��]�ɍ��킹���������ł��邱�Ƃ����҂���
    public IEnumerator RunActionSequence()
    {
        if (isRunningActionSequence)
        {
            Debug.LogWarning("�Z���ԂŃA�N�V�����V�[�P���X���A���ŌĂяo����܂����B");
            yield break;
        }

        isRunningActionSequence = true;

        //�A�N�V����ID�����[�v����Action����������
        for (actionID = 0; actionID < 3; actionID++)
        {
            isRunningSingleAction = true;
            RequestDecision();
            while (isRunningSingleAction)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame(); //�m���Ɋώ@�������Ȃ����߂ɁA�ǉ���1�t���[���҂�
        }

        isRunningActionSequence = false;
    }
}
