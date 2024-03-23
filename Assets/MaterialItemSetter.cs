using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialLibrary;

//ƒeƒXƒg
public class MaterialItemSetter : MonoBehaviour
{
    StripesMaterialItem stripesMaterialItem = new StripesMaterialItem();
    void Start()
    {
        stripesMaterialItem = new StripesMaterialItem();
        GetComponent<SpriteRenderer>().material = stripesMaterialItem.Material;
        stripesMaterialItem.SetPropertyColor(StripesMaterialProperty.MainColor, Color.red);
        stripesMaterialItem.SetPropertyColor(StripesMaterialProperty.AnotherColor, Color.green);
        stripesMaterialItem.SetPropertyFloat(StripesMaterialProperty.Space, 30);
    }

    private void Update()
    {
        stripesMaterialItem.SetPropertyFloat(StripesMaterialProperty.Timer, Time.time * 10);
    }


}
