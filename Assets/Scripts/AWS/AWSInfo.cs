using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�F�؂ɕK�v�ȏ���region�̏��ACognito�̏��ȂǁAAWS�̑���ŗp���邽�߂̐ÓI�ȕϐ��������Ƃ��ꏊ
namespace AWS
{
    public static class AWSInfo
    {
        static readonly string accessKeyID = Environment.GetEnvironmentVariable("PFT_LOCAL_ACCESS_KEY_ID");
        static readonly string secretAccessKey = Environment.GetEnvironmentVariable("PFT_LOCAL_ACCESS_KEY");
        static readonly string playerIDPoolID = Environment.GetEnvironmentVariable("PFT_IDPool_ID");
        static readonly RegionEndpoint region = RegionEndpoint.APNortheast1;

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
