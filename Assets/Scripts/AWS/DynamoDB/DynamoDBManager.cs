using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using static Unity.VisualScripting.Member;
using UnityEngine.SocialPlatforms.Impl;
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
        public async Task SaveScoreAsync(string modeAndLevel, int newScore)
        {
            try
            {
                int oldScore = 0;
                GetRecordAsync(modeAndLevel,(record)=>  oldScore=record.Score); //データベース上の現在の最高スコアを探し
                //新しいスコアが既存のスコアを上回っていたら、スコアを更新する。
                if (newScore > oldScore)
                {
                    var ranking = new DynamoDBDatas
                    {
                        ModeAndLevel = modeAndLevel,
                        Score = newScore,
                        PlayerID = playerID,
                    };

                    //DynamoDBにスコアを保存、なぜ失敗したのかなどの情報がresultにcallbackされる
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
                    Debug.LogError("現在の最高スコア以下を現在の最高スコア以下に更新しようとしました。");
                }
            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }



        //引数で指定されたモードに対するrecordをDynamoDBから非同期で取得する。(Task型で戻す)
        public void GetRecordAsync(string modeAndLevel, Action<DisplayScores> callback)
        {
            try
            {
                var singleQuery = GetSingleScoreQuery(modeAndLevel, playerID);
                //設定したクエリ(queryRequest)を非同期で実行、結果がresponseに格納される。
                client.QueryAsync(singleQuery, response =>
                {
                    //エラーがあればエラー内容を表示
                    if (response.Exception != null)
                    {
                        Debug.LogError($"Query Error: {response.Exception.Message}");
                        return;
                    }

                    var result = response.Response;//クエリの結果をresultに格納

                    DisplayScores record;
                    if (result.Items.Count == 1)
                    {
                        record = new DisplayScores()
                        {
                            ModeAndLevel = result.Items[0]["ModeAndLevel"].S,
                            Score = int.Parse(result.Items[0]["Score"].N),
                            PlayerID = result.Items[0]["PlayerID"].S,
                        };
                        if (result.Items.Count > 1) Debug.LogError("単一のレコードを返すためのクエリに、複数のレコードが返ってきています。");
                    }
                    else if(result.Items.Count == 0)
                    {
                        record = new DisplayScores()
                        {
                            ModeAndLevel = "",
                            Score = 0,
                            PlayerID = playerID
                        };
                        Debug.LogWarning("初めてのPlayerIDAndModeAndLevelの更新です。");
                    }
                    else
                    {
                        record = new DisplayScores()
                        {
                            ModeAndLevel = "",
                            Score = 0,
                            PlayerID = playerID
                        };
                        Debug.LogError("レコード数が異常値です");
                    }
                    callback(record);
                });
                

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        //引数で指定されたモード・レベルに対応するレコードを削除する処理。スコアの更新の際、古いスコアを消去するために使う。
        Task DeleteOldScoreAsync(string modeAndLevel, int oldScore)
        {
            var source = new TaskCompletionSource<bool>();
            try
            {
                Debug.Log($"{oldScore}がスコアのrecordを消去します");
                var key = new DynamoDBDatas { ModeAndLevel = modeAndLevel, Score = oldScore };
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
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                source.SetException(e);
            }
            return source.Task;
        }

        //引数で指定されたモードとレベルに応じてDynamoDBから非同期で取得し、結果をコールバックとして戻す。
        public void GetTop10Scores(string modeAndLevel, Action<List<DisplayScores>> callback)
        {
            try
            {
                //QueryAsyncを使用する際にはクエリの情報を持つクラスQueryRequestのインスタンスが必要で、ここでクエリの細かい設定を行う。
                var top10ScoreQuery = GetTop10RecordsQuery(modeAndLevel);

                //設定したクエリ(queryRequest)を非同期で実行、結果がresponseに格納される。
                client.QueryAsync(top10ScoreQuery, response =>
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
                            //「.N」や「.S」は内部のデータの型を表しており他にもバイナリ(B)、文字列リスト(SS)、数値リスト(NS)などがある。全て文字列として値が返ってくる点に注意。
                            ModeAndLevel = item["ModeAndLevel"].S,
                            Score = int.Parse(item["Score"].N),
                            PlayerID = item["PlayerID"].S
                        }); ;
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
            public string ModeAndLevel;
            public int Score;
            public string PlayerID;
        }

        public bool WaitInitialize()
        {
            return context == null;
        }

        //クエリを返すメソッド、引数でModeAndLevelとIDからスコア一意に取得する。
        QueryRequest GetSingleScoreQuery(string modeAndLevel, string playerID)
        {
            return new QueryRequest
            {
                TableName = "PFT_PlayerScoreRanking",
                KeyConditionExpression = "ModeAndLevel = :v1", //ModeAndLevelキーが:v1と一致するレコードを検索する。v1はプレースホルダーで、下の式で実際の値を割り当てている。
                FilterExpression = "PlayerID = :v2", //playerIDに対するフィルタ式、フィルタはクエリの実行後に適用されるため、大量のデータを扱う場合には注意

                //ExpressionAttributeValuesは、プレースホルダー(Key)に実際の値(Value)を割り当てる役割を持っている。
                //AttributeValueはDynamoDBの特別なオブジェクト型でDynamoDBで用いる様々な型のデータを扱えるようになっている。今回はString
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                {":v1", new AttributeValue { S = modeAndLevel }},
                {":v2", new AttributeValue { S = playerID }}
                },
                Limit = 1 //レコード数の上限
            };
        }

        //引数で受け取ったmodeAndLevelに応じたトップ10のレコードを取得するクエリ。
        QueryRequest GetTop10RecordsQuery(string modeAndLevel)
        {
            return new QueryRequest
            {
                TableName = "PFT_PlayerScoreRanking",
                KeyConditionExpression = "ModeAndLevel = :v1",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                {":v1", new AttributeValue { S = modeAndLevel }},
                },

                ScanIndexForward = false,
                Limit = 10
            };
        }
    }



}