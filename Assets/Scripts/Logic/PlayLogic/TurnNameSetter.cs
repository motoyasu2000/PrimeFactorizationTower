using Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TurnManagerのnamesの設定を行うクラス、
/// 異なるシーン間で設定を引き継ぎたいので、シングルトンパターンを使っている。
/// </summary>
public class TurnNameSetter : MonoBehaviour
{
    //シングルトンのインスタンス
    private static TurnNameSetter instance;
    public static TurnNameSetter Ins => instance;

    List<string> tmpNames = new List<string>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this; //単一のstaticインスタンスの生成。
            DontDestroyOnLoad(gameObject); //シーンの切り替え時に破棄されないようにする
            SceneManager.sceneLoaded += InitializeTurnSettings; 
        }
    }

    private void InitializeTurnSettings(Scene scene, LoadSceneMode mode)
    {
        InitializeTurnNameSettings();
    }

    private void InitializeTurnNameSettings()
    {
        //namesに何もない場合、とりあえずソロプレイに設定。(PlayerSceneを直接実行した場合など)
        if (tmpNames.Count == 0) SetNames_Single();
        TurnMangaer.SetPlayerNames(tmpNames);
    }

    //プレイヤーのみ
    public void SetNames_Single()
    {
        tmpNames.Clear();
        tmpNames.Add(PlayerInfoManager.Ins.PlayerName);
    }

    //プレイヤーVSAI
    public void SetNames_AI()
    {
        tmpNames.Clear();
        tmpNames.Add(PlayerInfoManager.Ins.PlayerName);
        tmpNames.Add(GameInfo.AIName);
    }

    //AIのみ(学習用)
    public void SetNames_AILearning()
    {
        tmpNames.Clear();
        tmpNames.Add(GameInfo.AIName);
    }
}
