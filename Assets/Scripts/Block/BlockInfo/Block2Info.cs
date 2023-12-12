using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block2Info : BlockInfo
{
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("TwoBlock");
    }

    public override void SetMyNumber()
    {
        myNumber = 2;
    }
}
