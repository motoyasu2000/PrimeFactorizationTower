using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLibrary {
    public static class MaterialPasses
    {
        //�}�e���A���̃p�X
        static readonly string materialsPass = "MaterialsOfItem";

        //�}�e���A���̖��O
        static readonly string stripesMaterialName = "StripesMaterial";
        static readonly string defaultBlocksMaterialItem = "DafaultBlocksMaterial";

        //�p�X���擾���郁�\�b�h
        static string GetMaterialPass(string materialName)
        {
            return $"{materialsPass}/{materialName}";
        }

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
