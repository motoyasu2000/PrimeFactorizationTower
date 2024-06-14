using UnityEngine;

namespace Common
{
    //ゲーム内の情報。複数のクラスから参照される可能性がある定数を集めたクラス
    public static class GameInfo
    {
        //ゲーム内で不変の情報
        static bool aiLearning = true; //強化学習中にはこのフラグがtrueになるようにする。(jsonファイルの書き込みやデータベースの更新を行わないように)
        static readonly int rankDisplayLimit = 10; //表示するランキングの上限値
        static readonly int cameraTrackingStartHeight = 4; //どこまで高く積んだらカメラが動き出すか
        static readonly float groundHeight = 0.5f; //元の地面の高さ
        static readonly string aiName = "___AI___"; //AIの名前を表す特殊な文字列
        static readonly Color buttonGray = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1);
        static readonly Color fleezeColor = new Color(23f / 255f, 1f, 1f);

        //visual studioでどこから参照されているのかが追跡できるようにするために、プロパティで参照するようにする。
        public static bool AILearning
        {
            get { return aiLearning; }
        }
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

        public static string AIName
        {
            get { return aiName; }
        }

        public static Color ButtonGray
        {
            get { return buttonGray; }
        }

        public static Color FleezeColor
        {
            get { return fleezeColor; }
        }

        //ゲーム内で可変の情報
        public static class Variables
        {
            static int nowScore = 0;
            static int oldMaxScore = 0;
            
            public static void SetNowScore(int newScore)
            {
                nowScore = newScore;
            }
            public static void SetOldMaxScore(int newScore)
            {
                oldMaxScore = newScore;
            }

            public static int GetNowScore()
            {
                return nowScore;
            }
            public static int GetOldMaxScore()
            {
                return oldMaxScore;
            }
        }
    }
}
