using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
//※dynamoDBのapiをたたくのは初めてでデータベースの操作も不慣れのため、勉強のメモ用のコメントが多くなっております。
//コメントがぐちゃぐちゃで読みにくかったらごめんなさい。


//----------------------------------------------------Taskメモ-------------------------------------------------------
//TaskはC#で非同期プログラミングをする際によく使われる型、バックグラウンドで実行される作業の完了を待つことができる。
//作業の完了を待つだけで特定の結果を返さない場合にもTaskが使えるし、Task<TResult>と書くことにより、TResult型の結果を返すこともできる。
//awaitをTaskの前に書くと、Taskが表すバックグラウンド作業が完了するまでメソッドの実行を一時停止してくれる。
//-------------------------------------------------------------------------------------------------------------------


namespace AWS
{
    //staticなクラスにしたいが、Debug.LogErrorを使用したいため、MonoBehaviorを継承させる
    public class DynamoDBManager : MonoBehaviour
    {
        AmazonDynamoDBClient client;
        private DynamoDBContext context;
        string playerID;

        public void Initialize(AmazonDynamoDBClient client, CognitoAWSCredentials cognitoAWSCredentials)
        {
            context = new DynamoDBContext(client);
            this.client = client;
            playerID = cognitoAWSCredentials.GetCachedIdentityId();
            Debug.Log(playerID);
        }

        //スコアをDynamoDBに非同期で保存する。
        public async Task SaveScoreAsync(string modeAndLevel, int score)
        {
            try
            {
                var existingRecord = await GetRecordAsync(modeAndLevel);
                //既にrecordが存在していて、新しいスコアが既存のスコアを上回っていたら、もしくは既存のスコアが見当たらなければ、スコアを更新する。
                if ((existingRecord != null && score > existingRecord.Score) || existingRecord == null)
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
                else
                {
                    Debug.LogError("現在のスコアを下回るスコアに更新しようとしました。");
                }
            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        //引数で指定されたモード・レベルに対応するレコードを削除する処理。スコアの更新の際、古いスコアを消去するために使う。※同一IDであれば自動で上書きされることが判明したため、現状は不要だった。
        Task DeleteScoreAsync(string modeAndLevel)
        {
            var source = new TaskCompletionSource<bool>();
            var key = new DynamoDBDatas { ModeAndLevel = modeAndLevel, PlayerID = playerID };
            context.DeleteAsync(key, result =>
            {
                if (result.Exception == null)
                {
                    Debug.Log($"レコード削除成功！");
                    source.SetResult(true);
                }
                else
                {
                    Debug.LogError($"レコード削除失敗: {result.Exception.Message}");
                    source.SetException(result.Exception);
                }
            });

            return source.Task;
        }

        //引数で指定されたモードに対するrecordをDynamoDBから非同期で取得する。(Task型で戻す)
        public Task<DynamoDBDatas> GetRecordAsync(string modeAndLevel)
        {
            var source = new TaskCompletionSource<DynamoDBDatas>();
            // PlayerID を使ってレコードを取得
            context.LoadAsync<DynamoDBDatas>(modeAndLevel, playerID, new AmazonDynamoDBCallback<DynamoDBDatas>((result) =>
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
            try
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
                    List<DisplayScores> records = new List<DisplayScores>(); //表示するrecordのトップ１０を格納するリスト、この値をコールバックする

                    //foreach文を使用して取得したレコードを一つずつ処理する。ここではrecordsリストを生成追加している。
                    foreach (var item in result.Items)
                    {
                        records.Add(new DisplayScores
                        {
                            //「.N」はAttributeValueの持つ数値(N)を意味する。文字列(S)、数値(N)、バイナリ(B)、文字列リスト(SS)、数値リスト(NS)などがある。全て文字列として値が返ってくる点に注意。
                            score = int.Parse(item["Score"].N),
                            playerID = item["PlayerID"].S
                        });
                    }
                    callback(records);
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }


        [DynamoDBTable("PFT_PlayerScoreRanking")]
        public class DynamoDBDatas
        {
            [DynamoDBHashKey]
            public string ModeAndLevel { get; set; }

            [DynamoDBRangeKey]
            public int Score { get; set; }

            [DynamoDBProperty]
            public string PlayerID { get; set; }
        }

        public class DisplayScores
        {
            public int score;
            public string playerID;
        }

        public bool WaitInitialize()
        {
            return context == null;
        }
    }
}