using AWS;
using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    const float delayTime = 1.2f;
    int compositeNumberAtGameOver; //ゲームオーバー時の合成数
    int blockNumberAtGameOver; //ゲームオーバーの引き金となったブロックの素数

    bool isGameOver = false; //ゲームオーバーになったらこのフラグをtrueにし、falseの時のみゲームオーバーの処理を実行するようにすることで、ゲームオーバーの処理が1度しか呼ばれないようにする。
    GameObject primeNumberCheckField; //ブロックを落下させた瞬間、そのブロックは、このゲームオブジェクトの子要素となる
    GameObject gameOverMenu;
    GameObject gameOverBlock; //ゲームオーバーの引き金となったブロック
    BloomManager bloomManager; //ゲームオーバー時の演出用
    OriginManager originManager;
    DynamoDBManager ddbManager;

    public int CompositeNumberAtGameOver => compositeNumberAtGameOver;
    public int BlockNumberAtGameOver => blockNumberAtGameOver;

    public bool IsBreakScore => (GameInfo.Variables.GetOldMaxScore() < GameInfo.Variables.GetNowScore()); //スコアを更新したかを判定するフラグ

    private void Awake()
    {
        originManager = GameObject.Find("OriginManager").GetComponent<OriginManager>();
        ddbManager = GameObject.Find("DynamoDBManager").GetComponent<DynamoDBManager>();
        primeNumberCheckField = GameObject.Find("PrimeNumberCheckField");
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
    }
    public async void GameOver(bool isFactorizationIncorrect)
    {
        //このメソッドが1度しか呼ばれないように
        if (isGameOver) return;
        else isGameOver = true;

        Debug.Log("GameOver");

        //素因数分解を間違えてしまった場合、最後のゲームオーバー理由の出力の際に、元の合成数とその時選択してしまった素数の情報が必要なので、変数に入れておく。
        if (isFactorizationIncorrect)
        {
            gameOverBlock = primeNumberCheckField.transform.GetChild(primeNumberCheckField.transform.childCount - 1).gameObject;
            blockNumberAtGameOver = gameOverBlock.GetComponent<BlockInfo>().GetPrimeNumber();
            compositeNumberAtGameOver = originManager.OriginNumber * blockNumberAtGameOver / CalculateBlocksCompositNumberAtGameOver(); //CalculateBlocksCompositNumberAtGameOver()にはblockNumberAtGameOverが含まれているためblockNumberAtGameOverをかける
        }

        //ゲームオーバー時の演出とスコアの更新、後処理の呼び出し。
        bloomManager.LightUpStart();
        GameInfo.Variables.SetOldMaxScore(ScoreManager.Ins.PileUpScores[GameModeManager.Ins.NowDifficultyLevel][0]);  //ソート前に過去の最高スコアの情報を取得しておく(のちにこのゲームで最高スコアを更新したかを確認するため)
        ScoreManager.Ins.InsertPileUpScoreAndSort(GameInfo.Variables.GetNowScore()) ;
        ScoreManager.Ins.SaveScoreData();
        SoundManager.Ins.FadeOutVolume();
        //スコアを更新していれば、データベースの更新
        if (IsBreakScore) await ddbManager.SaveScoreAsyncHandler(GameModeManager.Ins.ModeAndLevel, GameInfo.Variables.GetNowScore());

        StartCoroutine(PostGameOver(delayTime));
    }


    //ゲームオーバー後、一定時間後にゲームオーバーメニューを表示し、bgmのストップ。ゲームオーバーの後処理
    IEnumerator PostGameOver(float time)
    {
        yield return new WaitForSeconds(time);
        gameOverMenu.SetActive(true);
        SoundManager.Ins.StopAudio(SoundManager.Ins.BGM_PLAY);
        SoundManager.LoadSoundSettingData();
    }

    int CalculateBlocksCompositNumberAtGameOver()
    {
        int blocksCompositNumberAtGameOver = 1;
        foreach(Transform block in primeNumberCheckField.transform)
        {
            BlockInfo blockInfo = block.GetComponent<BlockInfo>();
            blocksCompositNumberAtGameOver *= blockInfo.GetPrimeNumber();
        }
        return blocksCompositNumberAtGameOver;
    }
}
