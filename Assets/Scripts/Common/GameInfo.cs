using UnityEngine;

namespace Common
{
    /// <summary>
    /// 複数のクラスから参照される可能性がある定数を集めたクラス。
    /// Variablesは可変の情報を提供する
    /// </summary>
    public static class GameInfo
    {
        //ゲーム内で不変の情報
        static readonly bool aiLearning = false; //強化学習中にはこのフラグがtrueになるようにする。(jsonファイルの書き込みやデータベースの更新を行わないように)
        static readonly int rankDisplayLimit = 10; //表示するランキングの上限値
        static readonly int cameraTrackingStartHeight = 4; //どこまで高く積んだらカメラが動き出すか
        static readonly float groundHeight = 0.5f; //元の地面の高さ
        static readonly string aiName = "___AI___"; //AIの名前を表す特殊な文字列
        static readonly Color buttonGray = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1);
        static readonly Color fleezeColor = new Color(23f / 255f, 1f, 1f);
        static readonly Color miniBlockColor = Color.red;
        static readonly Color initialBlockColor = new Color(0.8f, 0.8f, 0.8f);

        //visual studioでどこから参照されているのかが追跡できるようにするために、プロパティで参照するようにする。
        public static bool AILearning => aiLearning;
        public static int RankDisplayLimit => rankDisplayLimit;
        /// <summary>
        /// カメラが一定の高さに到達すると、カメラはすべてのブロックが見えるように拡大し、上に移動し始める。
        /// その一定の高さがいくつであるのかを提供する定数
        /// </summary>
        public static int CameraTrackingStartHeight => cameraTrackingStartHeight;
        public static float GroundHeight => groundHeight;
        public static string AIName => aiName;
        public static Color ButtonGray => buttonGray;
        public static Color FleezeColor => fleezeColor;
        public static Color MiniBlockColor => miniBlockColor;

        public static Color InitialBlockColor => initialBlockColor;

        /// <summary>
        /// 可変の情報の提供と操作を行うクラス
        /// </summary>
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
