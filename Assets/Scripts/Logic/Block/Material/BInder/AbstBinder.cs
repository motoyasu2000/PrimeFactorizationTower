using System;
using System.Reflection;
using UnityEngine;

//リフレクションやジェネリックや属性、Lazyパターンなど慣れていないことが多いため、メモ用のコメントが多くなっております。
namespace MaterialLibrary
{
    /// <summary>
    /// EnumParametersBinderを継承したクラスのリストを作るために、より抽象的なインターフェースを定義しておく。
    /// </summary>
    public interface IBinder
    {
        string MaterialPathAndName { get; }
        Type EnumType { get; }
        Material Material { get; set; }
        void ResetMaterial();
        void SetPropertyColor<TEnum>(TEnum property, Color value) where TEnum : Enum;
        void SetPropertyFloat<TEnum>(TEnum property, float value) where TEnum : Enum;
    }

    /// <summary>
    /// マテリアルのシェーダーのプロパティを列挙型で指定できるようにするためのクラス
    /// enumを持つことを間接的に継承先のクラスに強制させるために、ジェネリックを持たせる
    /// </summary>
    /// <typeparam name="TEnumGeneral">継承先のクラスのもつ列挙型</typeparam>

    public abstract class AbstBinder<TEnumGeneral> : IBinder where TEnumGeneral : Enum
    {
        public abstract string MaterialPathAndName { get; }

        public Type EnumType => typeof(TEnumGeneral);

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

        //Materialをロードするための抽象メソッド。継承先で実装。※Resourcesからマテリアルをロードしてそれを参照してMaterial変数に設定するのではなく、コピーしてからMaterial変数に設定する。
        protected abstract Material LoadMaterial();

        //-------------------列挙型の要素からシェーダーのプロパティを操作するメソッドたち----------------------
        /// <summary>
        /// Color型のプロパティを操作する
        /// </summary>
        /// <typeparam name="TEnumSpecific">どの列挙型のプロパティを変更するか</typeparam>
        /// <param name="property">どのプロパティを変更するか</param>
        /// <param name="color">どのような色にするか</param>
        public void SetPropertyColor<TEnumSpecific>(TEnumSpecific property, Color color) where TEnumSpecific : Enum
        {
            if (typeof(TEnumGeneral) == typeof(TEnumSpecific))
            {
                SetProperty<Color>(property, color, Material.SetColor);
            }
            else
            {
                throw new InvalidOperationException("Enumの型が不一致です。");
            }
        }

        /// <summary>
        /// binderのマテリアルに対して行った設定をすべてリセットする。
        /// </summary>
        public void ResetMaterial()
        {
            _material = LoadMaterial();
        }

        /// <summary>
        /// float型のプロパティを操作する
        /// </summary>
        /// <typeparam name="TEnumSpecific">どの列挙型のプロパティを変更するか</typeparam>
        /// <param name="property">どのプロパティを変更するか</param>
        /// <param name="value">どのくらいの値にするか</param>
        public void SetPropertyFloat<TEnumSpecific>(TEnumSpecific property, float value) where TEnumSpecific : Enum
        {
            if (typeof(TEnumGeneral) == typeof(TEnumSpecific))
            {
                SetProperty<float>(property, value, Material.SetFloat);
            }
            else
            {
                throw new InvalidOperationException("Enumの型が不一致です。");
            }
        }

        /// <summary>
        /// 指定されたプロパティに対して実際に値を設定する
        /// </summary>
        /// <typeparam name="TEnumSpecific">どの列挙型のプロパティを変更するか</typeparam>
        /// <param name="property">どのプロパティを変更するか</param>
        /// <param name="value">どのくらいの値にするか</param>
        /// <param name="action">プロパティの名前と値を受け取り、実際に値を設定するアクション</param>
        void SetProperty<TEnumSpecific>(Enum property, TEnumSpecific value, Action<string, TEnumSpecific> action)
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

    /// <summary>
    /// enum内で列挙型の値とシェーダーのプロパティ名のマッピングを行えるようにする。
    /// </summary>
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