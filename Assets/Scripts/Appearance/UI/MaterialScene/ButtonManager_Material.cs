using UnityEngine;
using Common;

/// <summary>
/// MaterialScene内のボタンによって呼ばれる機能を定義したクラス
/// </summary>
public class ButtonManager_Material : MonoBehaviour
{
    public void MoveTitleScene()
    {
        Helper.LoadScene("TitleScene");
    }
}
