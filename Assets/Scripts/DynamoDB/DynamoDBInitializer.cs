using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
//※dynamoDBのapiをたたくのは初めてなので、勉強のメモ用のコメントが多くなっております。

public class DynamoDBInitializer : MonoBehaviour
{
    DynamoDBManager ddbManager;

    string accessKeyId = Environment.GetEnvironmentVariable("PFT_LOCAL_ACCESS_KEY_ID");
    string secretAccessKey = Environment.GetEnvironmentVariable("PFT_LOCAL_ACCESS_KEY");
    private void Start()
    {
        Debug.Log(accessKeyId);
        ddbManager = transform.parent.GetComponent<DynamoDBManager>();
        if (ddbManager.ContextIsNull())
        {
            UnityInitializer.AttachToGameObject(gameObject);
            BasicAWSCredentials credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
            Debug.Log(credentials);
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = Amazon.RegionEndpoint.APNortheast1
            };
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials,config);
            ddbManager.Initialize(client);
        }

        //テスト用
        //ddbManager.SaveScoreAsync("aaa", 20, 100);
        //ddbManager.SaveScoreAsync("aaa", 30, 200);
        ddbManager.GetScoreAsync("aaa", 10).ContinueWith(task =>　//ContinueWithで非同期操作が完了した後に何をするのかを指定している。
        {
            if (task.IsFaulted)
            {
                // エラーが発生した場合
                Debug.LogError($"レコード取得失敗: {task.Exception}");
            }
            else
            {
                // 成功した場合、取得したレコードの詳細をログに出力
                var ranking = task.Result;
                if (ranking != null)
                {
                    Debug.Log($"ModeAndLevel: {ranking.ModeAndLevel}, Score: {ranking.Score}, PlayerID: {ranking.PlayerID}");
                }
                else
                {
                    Debug.Log("レコードが見つかりませんでした。");
                }
            }
        });
        ddbManager.GetTop10Scores("aaa", (records) =>
        {
            foreach (var record in records)
            {
                Debug.Log($"Score: {record.score}, PlayerID: {record.playerID}");
            }
        });
    }
}
