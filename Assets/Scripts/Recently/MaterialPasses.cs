using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLibrary {
    public static class MaterialPasses
    {
        //マテリアルのパス
        static readonly string materialsPass = "MaterialsOfItem";

        //マテリアルの名前
        static readonly string stripesMaterialName = "StripesMaterial";
        static readonly string defaultBlocksMaterialItem = "DefaultBlocksMaterial";

        //パスを取得するメソッド
        static string GetMaterialPass(string materialName)
        {
            return $"{materialsPass}/{materialName}";
        }



        //特定のマテリアルのパスを返す
        public static string GetStripesMaterialName()
        {
            return GetMaterialPass(stripesMaterialName);
        }
        public static string GetDefaultBlocksMaterialName()
        {
            return GetMaterialPass(defaultBlocksMaterialItem);
        }
    }
}
