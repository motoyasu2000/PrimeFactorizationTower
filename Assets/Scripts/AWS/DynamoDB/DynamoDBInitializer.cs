using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
namespace AWS
{
    public class DynamoDBInitializer : MonoBehaviour
    {
        DynamoDBManager ddbManager;

        private void Awake()
        {
            _ = InitializeAWSAsync(); //完了を待たないので注意
        }

        private async Task InitializeAWSAsync()
        {
            ddbManager = transform.parent.GetComponent<DynamoDBManager>();

            //DynamoDBに必要な情報の初期化
            if (ddbManager.WaitInitialize())
            {
                UnityInitializer.AttachToGameObject(gameObject);
                CognitoAWSCredentials cognitoAWSCredentials = new CognitoAWSCredentials(AWSInfo.PlayerIDPoolID, AWSInfo.Region);

                //Cognitoの認証を非同期で待つ
                await WaitGettingIDAsync(cognitoAWSCredentials);

                BasicAWSCredentials credentials = new BasicAWSCredentials(AWSInfo.AccessKeyID, AWSInfo.SecretAccessKey);
                AmazonDynamoDBConfig config = new AmazonDynamoDBConfig()
                {
                    RegionEndpoint = AWSInfo.Region
                };

                //認証が完了した後にDynamoDBClientを初期化
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, config);
                ddbManager.Initialize(client, cognitoAWSCredentials);

                var records = await ddbManager.GetTop10Scores(GameModeManager.Ins.ModeAndLevel);
                foreach (var record in records)
                {
                    Debug.Log(record.Score);
                    //ddbManager.DeleteOldScoreAsync(GameModeManager.GameModemanagerInstance.ModeAndLevel, record.Score);
                }
                
            }
        }

        //認証が終わるまで待機するためのコルーチン
        private Task WaitGettingIDAsync(CognitoAWSCredentials cognitoAWSCredentials)
        {
            var tcs = new TaskCompletionSource<bool>();
            cognitoAWSCredentials.GetIdentityIdAsync(response =>
            {
                if (response.Exception == null)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(response.Exception);
                }
            });
            return tcs.Task;
        }
    }
}