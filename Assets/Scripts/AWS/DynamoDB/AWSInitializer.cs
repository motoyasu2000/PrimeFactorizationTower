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
    public class AWSInitializer : MonoBehaviour
    {
        DynamoDBManager ddbManager;

        private void Awake()
        {
            StartCoroutine(InitializeAWS());
        }

        private IEnumerator InitializeAWS()
        {
            ddbManager = transform.parent.GetComponent<DynamoDBManager>();

            //DynamoDBに必要な情報の初期化
            if (ddbManager.WaitInitialize())
            {
                UnityInitializer.AttachToGameObject(gameObject);
                CognitoAWSCredentials cognitoAWSCredentials = new CognitoAWSCredentials(AWSInfo.PlayerIDPoolID, AWSInfo.Region);

                //Cognitoの認証を非同期で待つ
                yield return StartCoroutine(WaitGettingID(cognitoAWSCredentials));

                BasicAWSCredentials credentials = new BasicAWSCredentials(AWSInfo.AccessKeyID, AWSInfo.SecretAccessKey);
                AmazonDynamoDBConfig config = new AmazonDynamoDBConfig()
                {
                    RegionEndpoint = AWSInfo.Region
                };

                //認証が完了した後にDynamoDBClientを初期化
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, config);
                ddbManager.Initialize(client, cognitoAWSCredentials);

                //ddbManager.SaveScoreAsync(GameModeManager.GameModemanagerInstance.ModeAndLevel, 100);

                ddbManager.GetTop10Scores(GameModeManager.GameModemanagerInstance.ModeAndLevel, records =>
                {
                    foreach (var record in records)
                    {
                        Debug.Log($"PlayerID: {record.playerID}, Score: {record.score}");
                    }
                });
            }
        }

        //認証が終わるまで待機するためのコルーチン
        private IEnumerator WaitGettingID(CognitoAWSCredentials cognitoAWSCredentials)
        {
            bool isComplete = false;

            cognitoAWSCredentials.GetIdentityIdAsync(response =>
            {
                isComplete = true;
                if(response.Exception != null) Debug.LogError(response.Exception.Message);
            });

            //認証が完了するまで待機
            yield return new WaitUntil(() => isComplete);
        }
    }
}