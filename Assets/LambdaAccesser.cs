using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LambdaAccesser : MonoBehaviour
{
    private string apiUrl = "https://3tbzpdw367.execute-api.ap-northeast-1.amazonaws.com/dev/";
    private string getRankingUrl;
    private string saveRankingUrl;
    void Start()
    {
        getRankingUrl = apiUrl + "/ranking/query";
        saveRankingUrl = apiUrl + "/ranking/update";
        StartCoroutine(GetScoreTop10("test"));
        StartCoroutine(SaveScore("abc", 1000, "id", "ottoseiuchi2"));
    }

    IEnumerator GetScoreTop10(string modeAndLevel)
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
            else
            {
                Debug.Log("受け取った値: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator SaveScore(string modeAndLevel, int newScore, string playerID, string name)
    {
        PlayerScoreRecord newScoreRecord = new PlayerScoreRecord
        {
            ModeAndLevel = modeAndLevel,
            Score = newScore,
            PlayerID = playerID,
            Name = name
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

    [System.Serializable]
    public class PlayerScoreRecord
    {
        public string ModeAndLevel;
        public int Score;
        public string PlayerID;
        public string Name;
    }
}
