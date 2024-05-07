using Amazon;
using Amazon.CognitoIdentity;
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
        UnityInitializer.AttachToGameObject(this.gameObject);
        CognitoAWSCredentials cognitoAWSCredentials = new CognitoAWSCredentials(identityPoolId, Amazon.RegionEndpoint.APNortheast1);
        playerID = cognitoAWSCredentials.GetIdentityId();
        lambdaAccesser = GetComponent<LambdaAccesser>();
    }

    public async Task<List<PlayerScoreRecord>> GetScoreTop10(string modeAndLevel)
    {
        var tcs = new TaskCompletionSource<List<PlayerScoreRecord>>();
        StartCoroutine(lambdaAccesser.GetScoreTop10(modeAndLevel, (result) => tcs.SetResult(result)));
        return await tcs.Task;
    }

    public void SaveScore(int newScore)
    {
        StartCoroutine(lambdaAccesser.SaveScore(playerID, GameModeManager.Ins.NowModeAndLevel, newScore, PlayerInfoManager.Ins.PlayerName));
    }
}

[System.Serializable]
public class PlayerScoreRecord
{
    public string PlayerID;
    public string ModeAndLevel;
    public int Score;
    public string PlayerName;
}
