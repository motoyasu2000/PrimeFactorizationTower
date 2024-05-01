using Amazon.CognitoIdentity;
using AWS;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//スコアランキングの入出力を行うクラス
public class DynamoDBManager : MonoBehaviour
{
    static readonly string identityPoolId = "ap-northeast-1:749fa680-9001-4214-aa6f-dfa0c5edc588";
    string playerID;
    LambdaAccesser lambdaAccesser;

    private void Start()
    {
        CognitoAWSCredentials cognitoAWSCredentials = new CognitoAWSCredentials(identityPoolId, Amazon.RegionEndpoint.APNortheast1);
        playerID = cognitoAWSCredentials.GetIdentityId();
        lambdaAccesser = GetComponent<LambdaAccesser>();
    }

    public async Task<List<PlayerScoreRecord>> GetScoreTop10()
    {
        var tcs = new TaskCompletionSource<List<PlayerScoreRecord>>();
        StartCoroutine(lambdaAccesser.GetScoreTop10(GameModeManager.Ins.NowModeAndLevel, (result) => tcs.SetResult(result)));
        return await tcs.Task;
    }

    public void SaveScore(int newScore)
    {
        StartCoroutine(lambdaAccesser.SaveScore(GameModeManager.Ins.NowModeAndLevel, newScore, playerID, PlayerInfoManager.Ins.name));
    }
}

[System.Serializable]
public class PlayerScoreRecord
{
    public string ModeAndLevel;
    public int Score;
    public string PlayerID;
    public string Name;
}
