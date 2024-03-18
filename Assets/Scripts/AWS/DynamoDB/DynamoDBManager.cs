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
            string playerIDAndModeAndLevel = $"{playerID}_{modeAndLevel}";
            try
            {
                int oldScore = 0;
                GetRecordAsync(modeAndLevel,(record)=>  oldScore=record.Score); //データベース上の現在の最高スコアを探し
                //新しいスコアが既存のスコアを上回っていたら、スコアを更新する。
                if (newScore > oldScore)
                {
                    var ranking = new DynamoDBDatas
                    {
                        PlayerIDAndModeAndLevel = playerIDAndModeAndLevel,
                        Score = newScore,
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
            string playerIDAndModeAndLevel = $"{playerID}_{modeAndLevel}";
            try
            {
                //QueryAsyncを使用する際にはクエリの情報を持つクラスQueryRequestのインスタンスが必要で、ここでクエリの細かい設定を行う。
                var highestScoreQuery = GetScoreQuery(playerIDAndModeAndLevel, false, 1);

                //設定したクエリ(queryRequest)を非同期で実行、結果がresponseに格納される。
                client.QueryAsync(highestScoreQuery, response =>
                {
                    //エラーがあればエラー内容を表示
                    if (response.Exception != null)
                    {
                        Debug.LogError($"Query Error: {response.Exception.Message}");
                        return;
                    }

                    var result = response.Response;//クエリの結果をresultに格納

                    DisplayScores record;
                    if (result.Items.Count > 0)
                    {
                        record = new DisplayScores()
                        {
                            PlayerIDAndModeAndLevel = result.Items[0]["PlayerIDAndModeAndLevel"].S,
                            Score = int.Parse(result.Items[0]["Score"].N)
                        };
                        if (result.Items.Count > 1) Debug.LogError("単一のレコードを返すためのクエリに、複数のレコードが返ってきています。");
                    }
                    else
                    {
                        record = new DisplayScores()
                        {
                            PlayerIDAndModeAndLevel = "",
                            Score = 0
                        };
                        Debug.LogWarning("初めてのPlayerIDAndModeAndLevelの更新です。");
                    }
                    callback(record);

                });
                

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        //引数で指定されたモードとレベルに応じてDynamoDBから非同期で取得し、結果をコールバックとして戻す。
        public void GetTop10Scores(string modeAndLevel, Action<List<DisplayScores>> callback)
        {
            string playerIDAndModeAndLevel = $"{playerID}_{modeAndLevel}";
            try
            {
                //QueryAsyncを使用する際にはクエリの情報を持つクラスQueryRequestのインスタンスが必要で、ここでクエリの細かい設定を行う。
                var top10ScoreQuery = GetScoreQuery(playerIDAndModeAndLevel, false, 10);

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
                            PlayerIDAndModeAndLevel = item["PlayerIDAndModeAndLevel"].S,
                            Score = int.Parse(item["Score"].N)
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
            public string PlayerIDAndModeAndLevel { get; set; }

            [DynamoDBRangeKey]
            public int Score { get; set; }
        }

        public class DisplayScores
        {
            public string PlayerIDAndModeAndLevel;
            public int Score;
        }

        public bool WaitInitialize()
        {
            return context == null;
        }


        //現状使わないもの

        //引数で指定されたモード・レベルに対応するレコードを削除する処理。スコアの更新の際、古いスコアを消去するために使う。※同一IDであれば自動で上書きされることが判明したため、現状は不要だった。
        Task DeleteScoreAsync(string modeAndLevel)
        {
            string playerIDAndModeAndLevel = $"{playerID}_{modeAndLevel}";
            var source = new TaskCompletionSource<bool>();
            var key = new DynamoDBDatas { PlayerIDAndModeAndLevel = playerIDAndModeAndLevel };
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

        //最高スコアを取得するクエリ、引数でplayerIDAndModeAndLevel、昇順か降順か、取得するレコードの上限数を指定できる。
        QueryRequest GetScoreQuery(string playerIDAndModeAndLevel, bool isForward, int limitValue)
        {
            return new QueryRequest
            {
                TableName = "PFT_PlayerScoreRanking",
                KeyConditionExpression = "PlayerIDAndModeAndLevel = :v1", //PlayerIDAndModeAndLevelキーが:v1と一致するレコードを検索する。v1はプレースホルダーで、下の式で実際の値を割り当てている。

                //ExpressionAttributeValuesは、プレースホルダー(Key)に実際の値(Value)を割り当てる役割を持っている。
                //AttributeValueはDynamoDBの特別なオブジェクト型でDynamoDBで用いる様々な型のデータを扱えるようになっている。今回はString
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v1", new AttributeValue { S = playerIDAndModeAndLevel }}
                },
                ScanIndexForward = isForward, //降順で結果を表示
                Limit = limitValue //レコード数の上限
            };
        }
    }



}