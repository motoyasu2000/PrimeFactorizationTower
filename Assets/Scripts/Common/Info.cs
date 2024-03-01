namespace Common
{
    //ゲーム内の情報。複数のクラスから参照される可能性がある定数を集めたクラス
    public static class Info
    {
        public static readonly int rankDisplayLimit = 10; //表示するランキングの上限値
        public static readonly int cameraTrackingStartHeight = 4; //どこまで高く積んだらカメラが動き出すか
        public static readonly float groundHeight = 0.5f; //元の地面の高さ
    }
}
