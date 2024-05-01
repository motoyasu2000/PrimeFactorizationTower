using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LambdaAccesser : MonoBehaviour
{
    private string apiUrl = "https://3tbzpdw367.execute-api.ap-northeast-1.amazonaws.com/dev/";
    private string getSRankingUrl;
    void Start()
    {
        getSRankingUrl = apiUrl + "/ranking/query";
        StartCoroutine(GetScoreTop10());
    }

    IEnumerator GetScoreTop10()
    {
        string modeAndLevel = "test";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(modeAndLevel);

        //POST���N�G�X�g���쐬
        using (UnityWebRequest request = new UnityWebRequest(getSRankingUrl, "POST"))
        {
            //body��head�̐ݒ�
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "text/plain");

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
