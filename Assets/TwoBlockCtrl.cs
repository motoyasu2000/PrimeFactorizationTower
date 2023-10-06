using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoBlockCtrl : PrimeNumberBlockCtrl
{
    private void Start()
    {
        SetSelfPrefab();
    }
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("TwoBlock");
    }
}
