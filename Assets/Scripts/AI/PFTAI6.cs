using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PFTAI6 : Agent
{
    static readonly float posXScale = 5.0f; //x方向にどのくらい広く指定できるか(-posXScale~posXScale)
    static readonly int numOfSpinPattern = 8; //回転の種類
    bool isRunningSingleAction = false;
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
        Debug.Log("アクション呼び出し。");
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

    //回転角度を決める
    void SpinBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("ブロック回転");
        //0~315°回転する。(0*45°,1*45°,2*45°,...,7*45°) 生成したブロックごとに異なる回転のアクション空間を利用。
        spin45Count = actionBuffers.DiscreteActions[0];
        actions.SpinBlock45SeveralTimes(spin45Count);
        Debug.Log("Spin45Count: " + spin45Count);
    }

    //落下位置を決める
    void DropBlock(ActionBuffers actionBuffers)
    {
        Debug.Log("ブロック落下");
        //-5.0~5.0 生成したブロックごと、回転ごとに異なるアクション空間を利用する
        blockPosX = actionBuffers.ContinuousActions[GameModeManager.Ins.PrimeNumberPool.Length] * posXScale; //0~GameModeManager.Ins.PrimeNumberPool.Length-1はブロックの選択に使っている。
        actions.MoveBlockXAndRelease(blockPosX);
        Debug.Log("PosX: " + blockPosX);
    }

    //アクションを同時に行うのではなく、生成→観察→回転→観察→落下→観察と、すべてのアクションに間隔を設けて観察を行うことによって、生成されたブロックに合わせた回転、回転に合わせた落下ができることを期待する
    public IEnumerator RunActionSequence()
    {
        if (isRunningActionSequence)
        {
            Debug.LogWarning("短期間でアクションシーケンスが連続で呼び出されました。");
            yield break;
        }

        isRunningActionSequence = true;

        //アクションIDをループしてActionを処理する
        for (actionID = 0; actionID < 3; actionID++)
        {
            isRunningSingleAction = true;
            RequestDecision();
            while (isRunningSingleAction)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame(); //確実に観察をおこなうために、追加で1フレーム待つ
        }

        isRunningActionSequence = false;
    }
}
