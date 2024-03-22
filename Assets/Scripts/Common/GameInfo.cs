
using UnityEngine;

namespace Common
{
    //ゲーム内の情報。複数のクラスから参照される可能性がある定数を集めたクラス
    public static class GameInfo
    {
        static readonly int rankDisplayLimit = 10; //表示するランキングの上限値
        static readonly int cameraTrackingStartHeight = 4; //どこまで高く積んだらカメラが動き出すか
        static readonly float groundHeight = 0.5f; //元の地面の高さ
        static readonly Color buttonGray = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1);

        //visual studioでどこから参照されているのかが追跡できるようにするために、プロパティで参照するようにする。
        public static int RankDisplayLimit
        {
            get { return rankDisplayLimit; }
        }
        public static int CameraTrackingStartHeight
        {
            get { return cameraTrackingStartHeight; }
        }
        public static float GroundHeight
        {
            get { return groundHeight; }
        }

        public static Color ButtonRed
        {
            get { return buttonGray; }
        }
    }
}
