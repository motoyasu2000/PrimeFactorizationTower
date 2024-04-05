using MaterialLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLibrary
{
    //EnumParameterBinderを継承したクラスの情報を保持するクラス
    public static class EnumParameterBinderManager
    {
        static readonly IEnumParametersBinder[] binders = new IEnumParametersBinder[]
        {
            new DefaultMaterialEnumBinder(),
            new StripesMaterialEnumBinder(),
        };

        public static int bindersCount => binders.Length;

        public static IEnumParametersBinder[] Binders
        {
            get { return binders; }
        }
    }
}
