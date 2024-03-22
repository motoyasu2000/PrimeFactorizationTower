using MaterialLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialLibrary
{
    public enum StripesMaterialProperty
    {
        MainColor,
        AnotherColor,
        Space,
        Timer,
    }

    public class StripesMaterialItem : MaterialItem<StripesMaterialProperty>
    {
        public override StripesMaterialProperty Property { get; set; }

        public StripesMaterialItem()
        {
            Material = Resources.Load<Material>("MaterialsOfItem/StripesMaterial");
            PropertyNamesDict = new Dictionary<StripesMaterialProperty, string>()
            {
                { StripesMaterialProperty.MainColor, "_MainColor" },
                { StripesMaterialProperty.AnotherColor, "_AnotherColor" },
                { StripesMaterialProperty.Space, "_Space" },
                { StripesMaterialProperty.Timer, "_Timer" },
            };
        }
    }
}
