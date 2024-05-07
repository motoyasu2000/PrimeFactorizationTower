using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

public class LambdaAccesser : MonoBehaviour
{
    private string apiUrl = "https://3tbzpdw367.execute-api.ap-northeast-1.amazonaws.com/dev/";
    private string getRankingUrl;
    private string saveRankingUrl;
    void Start()
    {
        getRankingUrl = apiUrl + "/ranking/query";
        saveRankingUrl = apiUrl + "/ranking/update";
    }

    public IEnumerator GetScoreTop10(string modeAndLevel, Action<List<PlayerScoreRecord>> callback)
    {
        //POST���N�G�X�g���쐬
        using (UnityWebRequest request = new UnityWebRequest(getRankingUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(modeAndLevel);
            //body��head�̐ݒ�
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "text/plain");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("�G���[: " + request.error);
            }

            //�f�V���A���C�Y
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
            // SON�f�[�^���o�C�g�z��ɕϊ�
            byte[] sendingJsonData = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(sendingJsonData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            //���N�G�X�g�𑗐M
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("�G���[: " + request.error);
            }
            else
            {
                Debug.Log("�󂯎�����l: " + request.downloadHandler.text);
            }
        }
    }


}
