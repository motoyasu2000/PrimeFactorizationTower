namespace MaterialLibrary
{
    /// <summary>
    /// 複数のBinderを管理するためのクラス 
    /// </summary>
    public static class BinderManager
    {
        /// <summary>
        /// ここにすべてのバインダーを定義する。
        /// </summary>
        static readonly IBinder[] binders = new IBinder[]
        {
            new DefaultMaterialBinder(),
            new StripesMaterialBinder(),
            new WaveMaterialBinder(),
        };

        /// <summary>
        /// すべてのバインダーが格納された配列
        /// </summary>
        public static IBinder[] Binders
        {
            get { return binders; }
        }

        public static void ResetBinders()
        {
            foreach (var binder in binders)
            {
                binder.ResetMaterial();
            }
        }

        public static int BindersCount => binders.Length;

        //EnumParametersBinderを継承したクラスから、bindersのインデックスを取得。
        public static int GetBindersIndex(IBinder ibinder)
        {
            int returnIndex = 0;
            foreach (IBinder binder in binders)
            {
                if (binder.GetType() == ibinder.GetType())
                {
                    return returnIndex;
                }
                returnIndex++;
            }
            return -1;
        }
    }
}
