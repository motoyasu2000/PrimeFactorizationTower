using MaterialLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLibrary
{
    //EnumParameterBinderを継承したクラスの情報を管理するクラス
    public static class EnumParameterBinderManager
    {
        //EnumParametersBinderのインスタンスを格納するクラス
        static readonly IEnumParametersBinder[] binders = new IEnumParametersBinder[]
        {
            new DefaultMaterialEnumBinder(),
            new StripesMaterialEnumBinder(),
        };

        public static int BindersCount => binders.Length;

        public static IEnumParametersBinder[] Binders
        {
            get { return binders; }
        }

        //EnumParametersBinderを継承したクラスから、bindersのインデックスを取得。
        public static int GetBindersIndex(IEnumParametersBinder ibinder)
        {
            int returnIndex = 0;
            foreach (IEnumParametersBinder binder in binders)
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
