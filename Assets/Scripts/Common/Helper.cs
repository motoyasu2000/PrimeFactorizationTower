using UnityEngine.SceneManagement;

namespace Common
{
    //様々なクラスで扱うメソッドを持つクラス
    public static class Helper
    {
        //シーンを推移すると同時に、BGMをシーンに合わせて適切に変更する
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SoundManager.LoadSoundSettingData();
            SoundManager soundManager = SoundManager.Ins;
            if (sceneName == "PlayScene") soundManager.PlayAudio(SoundManager.Ins.BGM_PLAY);
            else if (sceneName == "TitleScene") soundManager.PlayAudio(SoundManager.Ins.BGM_TITLE);
        }

        //引数で分割数を指定し、その分割数にあったアンカーポイントの集合を返す。(ビューポート座標)
        public static float[] CalculateSplitAnchorPoints(int numOfDiv)
        {
            float[] splitAnchorPoints = new float[numOfDiv+1];
            float anchorPointsInterval = 1.0f / (float)numOfDiv;
            float nowAnchorPoint = 0.0f;
            for(int i=0; i<numOfDiv; i++)
            {
                splitAnchorPoints[i] = nowAnchorPoint;
                nowAnchorPoint += anchorPointsInterval;
            }
            //浮動小数点誤差を無くすため、最後は直接1を入れる
            splitAnchorPoints[numOfDiv] = 1.0f;
            return splitAnchorPoints;
        }
    }
}