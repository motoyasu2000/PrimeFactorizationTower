using Amazon.Runtime.Internal.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シェーダーのプロパティをＣ＃スクリプトから操作する場合には、文字列でプロパティ名を指定しなければならない。
//それを解決するために、列挙型の要素と文字列をマッピングして、列挙型の要素によって文字列を指定するようにしたい。
//しかし今回のゲームでは多くのマテリアルが存在することが想定されるため、マテリアルを扱うクラスの抽象クラスをつくりたかったが、
//今回はマテリアルごとに要素が異なるenumを持ちたかったが、enumは定数の集まりであるべきで、abstractと組み合わせることはできない。
//そこで、ジェネリックを使用して表現してみた。
//また、リストとしてMaterialItemクラスを格納するために、さらに抽象的なインターフェースを定義した。

namespace MaterialLibrary
{
    public interface IMaterialItem
    {
        Material Material { get; set; }
    }

    public abstract class MaterialItem<T> : IMaterialItem where T : Enum
    {
        public Material Material { get; set; }
        public abstract T Property { get; set; } //文字列を使わずにプロパティを操作するための列挙型
        public Dictionary<T, string> PropertyNamesDict { get; set; } //列挙型の要素とシェーダーのプロパティ名を対応付ける辞書

        //-------------------列挙型の要素からシェーダーのプロパティを操作するメソッドたち----------------------
        //float型のプロパティの操作
        public void SetPropertyFloat(T property, float value)
        {
            if (PropertyNamesDict.TryGetValue(property, out string propertyName))
            {
                Material.SetFloat(propertyName, value);
            }
        }
        //Color型のプロパティの操作
        public void SetPropertyColor(T property, Color value)
        {
            if (PropertyNamesDict.TryGetValue(property, out string propertyName))
            {
                Material.SetColor(propertyName, value);
            }
        }
        //-------------------列挙型の要素からシェーダーのプロパティを操作するメソッドたち----------------------
    }
}