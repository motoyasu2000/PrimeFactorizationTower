﻿using Amazon.Runtime.Internal.Auth;
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
        void SetPropertyColor<TEnum>(TEnum property, Color value) where TEnum : Enum;
        void SetPropertyFloat<TEnum>(TEnum property, float value) where TEnum : Enum;
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

        //Materialをロードするための抽象メソッド。継承先で実装。※ResourcesからマテリアルをロードしてそのままMaterial変数に設定するのではなく、コピーしてからMaterial変数に設定する。
        protected abstract Material LoadMaterial();

        //-------------------列挙型の要素からシェーダーのプロパティを操作するメソッドたち----------------------
        //float型のプロパティの操作
        public void SetPropertyColor<TEnumProperty>(TEnumProperty property, Color value) where TEnumProperty : Enum
        {
            if (typeof(TEnum) == typeof(TEnumProperty))
            {
                SetProperty<Color>(property, value, Material.SetColor);
            }
            else
            {
                throw new InvalidOperationException("Enumの型が不一致です。");
            }
        }
        //Color型のプロパティの操作
        public void SetPropertyFloat<TEnumProperty>(TEnumProperty property, float value) where TEnumProperty : Enum
        {
            if (typeof(TEnum) == typeof(TEnumProperty))
            {
                SetProperty<float>(property, value, Material.SetFloat);
            }
            else
            {
                throw new InvalidOperationException("Enumの型が不一致です。");
            }
        }

        void SetProperty<TProperty>(Enum property, TProperty value, Action<string, TProperty> action)
        {
            var propertyInfo = property.GetType().GetField(property.ToString());
            var attribute = propertyInfo.GetCustomAttribute<ShaderPropertyAttribute>();
            if (attribute != null)
            {
                action(attribute.PropertyName, value);
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