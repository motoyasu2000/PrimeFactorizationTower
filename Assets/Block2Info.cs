using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block2Info : BlockInfo
{
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("TwoBlock");
    }

    public override void AddRigidbody2D()
    {
        Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.freezeRotation = true;
    }
    public override void SetMyNumber()
    {
        myNumber = 2;
    }
}
