using Common;
using System.Collections;
using UnityEngine;

/// <summary>
/// ゲームオーバーを管理するクラス
/// </summary>
public class GameOverManager : MonoBehaviour
{
    const float delayTime = 1.2f;

    bool isGameOver = false; //ゲームオーバーになったらこのフラグをtrueにし、falseの時のみゲームオーバーの処理を実行するようにすることで、ゲームオーバーの処理が1度しか呼ばれないようにする。
    GameObject gameOverMenu;
    BloomManager bloomManager; //ゲームオーバー時の演出用
    DynamoDBManager ddbManager;

    public bool IsGameOver => isGameOver;  
    public bool IsBreakScore => GameInfo.Variables.GetOldMaxScore() < GameInfo.Variables.GetNowScore(); //スコアを更新したかを判定するフラグ


    private void Awake()
    {
        ddbManager = GameObject.Find("DynamoDBManager").GetComponent<DynamoDBManager>();
        gameOverMenu = GameObject.Find("Canvas").transform.Find("GameOverMenu").gameObject;
        bloomManager = GameObject.Find("GlobalVolume").GetComponent<BloomManager>();
    }

    /// <summary>
    /// ゲームオーバー時に一度だけ呼ばれるゲームオーバー処理
    /// 画面全体がまぶしくなり、スコアの更新を行い、BGMが減衰していき、PostGameOberを実行する
    /// </summary>
    public void GameOver()
    {
        //このメソッドが1度しか呼ばれないように
        if (isGameOver) return;
        else isGameOver = true;

        if (GameInfo.AILearning) return; //isGameOverの値に応じてエピソードを再実行するかを決定するため、isGameOverの更新のあとでreturn

        Debug.Log("GameOver");

        //ゲームオーバー時の演出とスコアの更新、後処理の呼び出し。
        bloomManager.LightUpStart();
        GameInfo.Variables.SetOldMaxScore(ScoreManager.Ins.PileUpScores[GameModeManager.Ins.NowDifficultyLevel][0]);  //ソート前に過去の最高スコアの情報を取得しておく(のちにこのゲームで最高スコアを更新したかを確認するため)
        ScoreManager.Ins.InsertPileUpScoreAndSort(GameInfo.Variables.GetNowScore()) ;
        ScoreManager.Ins.SaveScoreData();
        SoundManager.Ins.FadeOutVolume();
        //スコアを更新していれば、データベースの更新
        if (IsBreakScore) ddbManager.SaveScore(GameInfo.Variables.GetNowScore());

        StartCoroutine(PostGameOver(delayTime));
    }

    /// <summary>
    /// ゲームオーバーの後処理を行う。
    /// ゲームオーバー後、一定時間後にゲームオーバーメニューを表示し、bgmのストップ。
    /// </summary>
    /// <param name="time">GameOber処理の後、どのくらいでこの処理が実行されるか</param>
    IEnumerator PostGameOver(float time)
    {
        yield return new WaitForSeconds(time);
        gameOverMenu.SetActive(true);
        SoundManager.Ins.StopAudio(SoundManager.Ins.BGM_PLAY);
        SoundManager.LoadSoundSettingData();
    }
}
