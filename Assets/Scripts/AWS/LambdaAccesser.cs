using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

/// <summary>
/// AWSのapi gatewayを使用し、HTTPリクエストを送り、DynamoDBを操作するLambdaにアクセスするクラス
/// </summary>
public class LambdaAccesser : MonoBehaviour
{
    static readonly string apiUrl = "https://3tbzpdw367.execute-api.ap-northeast-1.amazonaws.com/dev/";
    static readonly string getRankingUrl = apiUrl + "/ranking/query";
    static readonly string saveRankingUrl = apiUrl + "/ranking/update";

    /// <summary>
    /// 非同期でDynamoDBからTop10のレコードを取得、各レコードをリストとしてコールバックで返す。
    /// </summary>
    /// <param name="modeAndLevel">[mode]_[lebel]</param>
    /// <param name="callback">Top10の各レコードのリスト</param>
    /// <returns></returns>
    public IEnumerator GetScoreTop10(string modeAndLevel, Action<List<PlayerScoreRecord>> callback)
    {
        //POSTリクエストを作成
        using (UnityWebRequest request = new UnityWebRequest(getRankingUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(modeAndLevel);
            //bodyとheadの設定
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "text/plain");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("エラー: " + request.error);
            }

            //デシリアライズ
            var serializer = new DataContractJsonSerializer(typeof(List<PlayerScoreRecord>));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(request.downloadHandler.text)))
            {
                var scores = (List<PlayerScoreRecord>)serializer.ReadObject(ms);
                callback(scores);
            }
        }
    }

    public IEnumerator SaveScore(string playerID, string modeAndLevel, int newScore, string playerName)
    {
        PlayerScoreRecord newScoreRecord = new PlayerScoreRecord
        {
            ModeAndLevel = modeAndLevel,
            Score = newScore,
            PlayerID = playerID,
            PlayerName = playerName
        };

        string json = JsonUtility.ToJson(newScoreRecord);

        using (UnityWebRequest request = new UnityWebRequest(saveRankingUrl, "POST"))
        {
            // SONデータをバイト配列に変換
            byte[] sendingJsonData = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(sendingJsonData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            //リクエストを送信
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("エラー: " + request.error);
            }
            else
            {
                Debug.Log("受け取った値: " + request.downloadHandler.text);
            }
        }
    }


}
