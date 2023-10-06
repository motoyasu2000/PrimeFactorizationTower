using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTwo : TouchPrimeNumber
{
    private void Start()
    {
        SetSelfPrefab();
    }
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("TwoBlock");
    }

    public override void AddRigidbody2D()
    {
        Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.freezeRotation = true;
    }
}
