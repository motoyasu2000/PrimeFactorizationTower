using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfoManager : MonoBehaviour
{
    //シングルトンのインスタンス
    private static PlayerInfoManager instance;
    public static PlayerInfoManager Ins => instance;

    //保存する情報
    [SerializeField] string name;
    [SerializeField] MaterialDatabase materialDatabase;

    //名前を入力させるUI　nameがjsonで保存されていなかった場合に、使う
    GameObject inputNameMenuBackGround;

    //プロパティ
    public string Name => name;


    void Awake()
    {
        //非アクティブなオブジェクトなので、transform.Findで探している。
        if(SceneManager.GetActiveScene().name == "TitleScene") inputNameMenuBackGround = GameObject.Find("Canvas").transform.Find("InputNameMenuBackGround").gameObject;

        if (instance == null)
        {
            instance = this; //単一のstaticインスタンスの生成。
            DontDestroyOnLoad(this.gameObject); //シーンの切り替え時に破棄されないようにする
        }
        LoadPlayerInfo();
    }

    public void InputNameProcess(string name)
    {
        this.name = name;
        SavePlayerInfo();
    }


    //プレイヤーデータをJson形式で保存する
    public void SavePlayerInfo()
    {
        PlayerInfoManager dPlayerInfoInstance = instance;
        string jsonstr = JsonUtility.ToJson(dPlayerInfoInstance);
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/PlayerInfo.json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    //Json形式のプレイヤーデータをロードする
    public void LoadPlayerInfo()
    {
        //プレイヤー情報が保存されていない場合、つまりおそらく初めてプレイした場合に、名前入力の画面を表示させる。
        if (!File.Exists(Application.persistentDataPath + "/PlayerInfo.json"))
        {
            inputNameMenuBackGround.SetActive(true);
            return;
        }
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/PlayerInfo.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var obj = JsonUtility.FromJson<PlayerInfo>(datastr); //Monobehaviorを継承したクラスではJsonファイルを読み込むことができないため、他のクラスを生成し読み込む
        instance.name = obj.name;
    }

    class PlayerInfo
    {
        public string name;
        public MaterialDatabase materialDatabase;
    }
}
