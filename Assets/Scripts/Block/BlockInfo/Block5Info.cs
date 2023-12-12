using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block5Info : BlockInfo
{
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("FiveBlock");
    }
    public override void SetMyNumber()
    {
        myNumber = 5;
    }
}
