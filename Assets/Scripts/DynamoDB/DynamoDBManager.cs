using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;
using UnityEngine;

//staticなクラスにしたいが、Debug.LogErrorを使用したいため、Monobehaviorを継承させる
public class DynamoDBManager : MonoBehaviour
{
    private DynamoDBContext context;
    public void Initialize(AmazonDynamoDBClient client)
    {
        context = new DynamoDBContext(client);
        if (context == null) Debug.LogError("clientからcontextが取得できませんでした。");
        else Debug.Log($"正常にcontextが取得できました。: {context}");
    }

    public async Task SaveScoreAsync(string modeAndLevel, int score, int playerID)
    {
        var ranking = new Ranking
        {
            ModeAndLevel = modeAndLevel,
            Score = score,
            PlayerID = playerID
        };

        // DynamoDBにスコアを保存
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

    [DynamoDBTable("PFT_PlayerScoreRanking")]
    public class Ranking
    {
        [DynamoDBHashKey]
        public string ModeAndLevel { get; set; }

        [DynamoDBRangeKey]
        public int Score { get; set; }

        [DynamoDBProperty]
        public int PlayerID { get; set; }
    }

    public bool ContextIsNull()
    {
        return context == null;
    }
}