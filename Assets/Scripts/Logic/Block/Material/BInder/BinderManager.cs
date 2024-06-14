namespace MaterialLibrary
{
    /// <summary>
    /// 複数のBinderを管理するためのクラス 
    /// </summary>
    public static class BinderManager
    {
        //EnumParametersBinderのインスタンスを格納する
        static readonly IBinder[] binders = new IBinder[]
        {
            new DefaultMaterialBinder(),
            new StripesMaterialBinder(),
            new WaveMaterialBinder(),
        };

        public static int BindersCount => binders.Length;

        public static IBinder[] Binders
        {
            get { return binders; }
        }

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
