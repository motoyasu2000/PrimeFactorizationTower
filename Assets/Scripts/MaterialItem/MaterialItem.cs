using Amazon.Runtime.Internal.Auth;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//リフレクションやジェネリックや属性、Lazyパターンなど慣れていないことが多いため、メモ用のコメントが多くなっております。
namespace MaterialLibrary
{
    //MaterialItemを継承したクラスのリストを作るために、より抽象的なインターフェースを定義しておく。
    public interface IMaterialItem
    {
        Material Material { get; set; }
    }

    //enumを持つことを間接的に継承先のクラスに強制させるために、ジェネリックを持たせる
    public abstract class MaterialItem<TEnum> : IMaterialItem where TEnum : Enum
    {
        private Material _material = null;

        //コンストラクタでマテリアルをロードするとコンパイル時にロードされてしまい、Unity側の初期化が終わっていないことがある。
        //そのため、Materialプロパティを呼び出すときにマテリアルをロードするようにする。
        //マテリアルのプロパティを呼び出すのはUnity上のAwakeやStartであることがおおいため、基本的にUnity側の初期化が終わった後でロードできる。
        public Material Material
        {
            get
            {
                if (_material == null)
                {
                    _material = LoadMaterial();
                }
                return _material;
            }
            set { _material = value; }
        }

        //Materialをロードするための抽象メソッド。継承先で実装。
        protected abstract Material LoadMaterial();

        //-------------------列挙型の要素からシェーダーのプロパティを操作するメソッドたち----------------------
        //float型のプロパティの操作
        public void SetPropertyFloat(TEnum property, float value)
        {
            SetProperty<float>(property, value, Material.SetFloat);
        }
        //Color型のプロパティの操作
        public void SetPropertyColor(TEnum property, Color value)
        {
            SetProperty<Color>(property, value, Material.SetColor);
        }

        void SetProperty<TProperty>(TEnum property, TProperty value, Action<string,TProperty> action)
        {
            var propertyInfo = property.GetType().GetField(property.ToString()); //ジェネリックで受け取った列挙型の値に基づいて、その列挙型の値が定義されているフィールドのメタデータを取得している
            var attribute = propertyInfo.GetCustomAttribute<ShaderPropertyAttribute>(); //propertyInfoからShaderPropertyAttributeの取得。見つからなければnullが返る。
            if (attribute != null)
            {
                action(attribute.PropertyName, value); //第三引数で受け取ったメソッドの実行。
            }
            else
            {
                Debug.LogError("指定された列挙型の値に対応するShaderPropertyAttributeが見つかりませんでした。");
            }
        }
        //----------------------------------------------------------------------------------------------------
    }

    //enum内で列挙型の値とシェーダーのプロパティ名のマッピングを行えるようにする。
    [AttributeUsage(AttributeTargets.Field)]
    public class ShaderPropertyAttribute : Attribute
    {
        public string PropertyName { get; private set; }

        public ShaderPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}