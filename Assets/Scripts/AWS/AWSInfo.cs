using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//認証に必要な情報やregionの情報、Cognitoの情報など、AWSの操作で用いるための静的な変数を書いとく場所
namespace AWS
{
    public static class AWSInfo
    {
        static readonly string tableName = "PFT_PlayerScoreRanking";
        static readonly string accessKeyID = Environment.GetEnvironmentVariable("PFT_LOCAL_ACCESS_KEY_ID");
        static readonly string secretAccessKey = Environment.GetEnvironmentVariable("PFT_LOCAL_ACCESS_KEY");
        static readonly string playerIDPoolID = Environment.GetEnvironmentVariable("PFT_IDPool_ID");
        static readonly RegionEndpoint region = RegionEndpoint.APNortheast1;

        public static string TableName
        {
            get { return tableName; }
        }
        public static string AccessKeyID
        {
            get { return accessKeyID; }
        }

        public static string SecretAccessKey
        {
            get { return secretAccessKey; }
        }

        public static string PlayerIDPoolID
        {
            get { return playerIDPoolID; }
        }

        public static RegionEndpoint Region
        {
            get { return region; }
        }
    }
}
