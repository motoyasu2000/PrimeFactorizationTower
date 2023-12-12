using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block7Info : BlockInfo
{
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("SevenBlock");
    }
    public override void SetMyNumber()
    {
        myNumber = 7;
    }
}
