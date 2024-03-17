using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.DynamoDBv2.Model;
//※dynamoDBのapiをたたくのは初めてなので、勉強のメモ用のコメントが多くなっております。


//----------------------------------------------------Taskメモ-------------------------------------------------------
//TaskはC#で非同期プログラミングをする際によく使われる型、バックグラウンドで実行される作業の完了を待つことができる。
//作業の完了を待つだけで特定の結果を返さない場合にもTaskが使えるし、Task<TResult>と書くことにより、TResult型の結果を返すこともできる。
//awaitをTaskの前に書くと、Taskが表すバックグラウンド作業が完了するまでメソッドの実行を一時停止してくれる。
//-------------------------------------------------------------------------------------------------------------------


//staticなクラスにしたいが、Debug.LogErrorを使用したいため、Monobehaviorを継承させる
public class DynamoDBManager : MonoBehaviour
{
    AmazonDynamoDBClient client;
    private DynamoDBContext context;
    public void Initialize(AmazonDynamoDBClient client)
    {
        context = new DynamoDBContext(client);
        this.client = client;
        if (context == null) Debug.LogError("clientからcontextが取得できませんでした。");
        else Debug.Log($"正常にcontextが取得できました。: {context}");
    }

    //スコアをDynamoDBに非同期で保存する。
    public async Task SaveScoreAsync(string modeAndLevel, int score, int playerID)
    {
        var ranking = new DynamoDBDatas
        {
            ModeAndLevel = modeAndLevel,
            Score = score,
            PlayerID = playerID
        };

        //DynamoDBにスコアを保存、うまくいったかどうかの情報や、なぜ失敗したのかの情報がresultにcallbackされる
        context.SaveAsync(ranking, result =>
        {
            if (result.Exception == null)
            {
                Debug.Log("スコア更新成功！");
            }
            else
            {
                Debug.LogError($"スコア更新失敗: {result.Exception.Message}");
            }
        });
    }

    //引数で指定されたモードとレベルとスコアに対するrecordをDynamoDBから非同期で取得する。(Task型で戻す)
    public Task<DynamoDBDatas> GetScoreAsync(string modeAndLevel, int score)
    {
        var source = new TaskCompletionSource<DynamoDBDatas>();
        //AmazonDynamoDBCallbackは非同期操作の結果を受け取るために使われる。非同期操作が完了すると、SDK内部からこのコールバックが呼び出される。
        context.LoadAsync<DynamoDBDatas>(modeAndLevel, score, new AmazonDynamoDBCallback<DynamoDBDatas>((result) =>
        {
            if (result.Exception == null)
            {
                Debug.Log($"レコード取得成功！: {result.Result}");
                source.SetResult(result.Result);
            }
            else
            {
                Debug.LogError($"レコード取得失敗: {result.Exception.Message}");
                source.SetException(result.Exception);
            }
        }));
        
        return source.Task;
    }

    //引数で指定されたモードとレベルに応じてDynamoDBから非同期で取得し、結果をコールバックとして戻す。
    public void GetTop10Scores(string modeAndLevel, Action<List<DisplayScores>> callback)
    {
        //QueryAsyncを使用する際にはクエリの情報を持つクラスQueryRequestのインスタンスが必要で、ここでクエリの細かい設定を行う。
        var queryRequest = new QueryRequest
        {
            TableName = "PFT_PlayerScoreRanking",
            KeyConditionExpression = "ModeAndLevel = :v1", //ModeAndLevelキーが:v1と一致するレコードを検索する。v1はプレースホルダーで、下の式で実際の値を割り当てている。

            //ExpressionAttributeValuesは、プレースホルダー(Key)に実際の値(Value)を割り当てる役割を持っている。
            //AttributeValueはDynamoDBの特別なオブジェクト型でDynamoDBで用いる様々な型のデータを扱えるようになっている。今回はString
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> {　
            {":v1", new AttributeValue { S = modeAndLevel }}
        },
            ScanIndexForward = false, //降順で結果を表示
            Limit = 10 //レコード数の上限
        };

        //設定したクエリ(queryRequest)を非同期で実行、結果がresponseに格納される。
        client.QueryAsync(queryRequest, response =>
        {
            //エラーがあればエラー内容を表示
            if (response.Exception != null)
            {
                Debug.LogError($"Query Error: {response.Exception.Message}");
                return;
            }
            
            var result = response.Response;//クエリの結果をresultに格納
            List<DisplayScores> scores = new List<DisplayScores>(); //表示するrecordのトップ１０を格納するリスト、この値をコールバックする

            //foreach文を使用して取得したレコードを一つずつ処理する。ここでは結果をデバッグログに出力し、scoresリストに追加している。
            foreach (var item in result.Items)
            {
                scores.Add(new DisplayScores {
                    //「.N」はAttributeValueの持つ数値(N)を意味する。文字列(S)、数値(N)、バイナリ(B)、文字列リスト(SS)、数値リスト(NS)などがある。全て文字列として値が返ってくる。
                    score = int.Parse(item["Score"].N), 
                    playerID = item["PlayerID"].N 
                });
                
                Debug.Log($"Score: {item["Score"].N}, PlayerID: {item["PlayerID"].N}");
            }
            callback(scores);
        });
    }


    [DynamoDBTable("PFT_PlayerScoreRanking")]
    public class DynamoDBDatas
    {
        [DynamoDBHashKey]
        public string ModeAndLevel { get; set; }

        [DynamoDBRangeKey]
        public int Score { get; set; }

        [DynamoDBProperty]
        public int PlayerID { get; set; }
    }

    public class DisplayScores
    {
        public int score;
        public string playerID;
    }

    public bool ContextIsNull()
    {
        return context == null;
    }
}