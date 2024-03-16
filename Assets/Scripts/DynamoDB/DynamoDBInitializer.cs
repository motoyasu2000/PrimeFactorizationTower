using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using UnityEngine;


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
        ddbManager.SaveScoreAsync("aaa", 10, 100);
    }
}
