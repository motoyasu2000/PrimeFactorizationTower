using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;
using UnityEngine;

//static�ȃN���X�ɂ��������ADebug.LogError���g�p���������߁AMonobehavior���p��������
public class DynamoDBManager : MonoBehaviour
{
    private DynamoDBContext context;
    public void Initialize(AmazonDynamoDBClient client)
    {
        context = new DynamoDBContext(client);
        if (context == null) Debug.LogError("client����context���擾�ł��܂���ł����B");
        else Debug.Log($"�����context���擾�ł��܂����B: {context}");
    }

    public async Task SaveScoreAsync(string modeAndLevel, int score, int playerID)
    {
        var ranking = new Ranking
        {
            ModeAndLevel = modeAndLevel,
            Score = score,
            PlayerID = playerID
        };

        // DynamoDB�ɃX�R�A��ۑ�
        context.SaveAsync(ranking, result =>
        {
            if (result.Exception == null)
            {
                Debug.Log("�X�R�A�X�V�����I");
            }
            else
            {
                Debug.LogError($"�X�R�A�X�V���s: {result.Exception.Message}");
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