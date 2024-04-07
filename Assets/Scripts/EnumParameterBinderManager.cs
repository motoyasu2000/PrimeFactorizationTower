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
