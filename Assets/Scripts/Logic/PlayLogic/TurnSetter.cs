using Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ターンの設定を行うクラス、異なるシーン間で設定を引き継ぎたいので、シングルトンパターンを使っている。
public class TurnSetter : MonoBehaviour
{
    //シングルトンのインスタンス
    private static TurnSetter instance;
    public static TurnSetter Ins => instance;

    List<string> names = new List<string>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this; //単一のstaticインスタンスの生成。
            DontDestroyOnLoad(this.gameObject); //シーンの切り替え時に破棄されないようにする
            SceneManager.sceneLoaded += InitializeTurnSettings; 
        }
    }

    private void InitializeTurnSettings(Scene scene, LoadSceneMode mode)
    {
        InitializeTurnSettings();
    }

    private void InitializeTurnSettings()
    {
        if (names.Count == 0) return;
        TurnMangaer.SetNumberOfPlayer(names.Count);
        TurnMangaer.SetPlayerNames(names);
    }

    public void SetNames_Single()
    {
        names.Clear();
        names.Add(PlayerInfoManager.Ins.PlayerName);
    }

    public void SetNames_AI()
    {
        names.Clear();
        names.Add(PlayerInfoManager.Ins.PlayerName);
        names.Add(GameInfo.AIName);
    }
}
