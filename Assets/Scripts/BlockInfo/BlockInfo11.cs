using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block11Info : BlockInfo
{
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("ElevenBlock");
    }
    public override void SetMyNumber()
    {
        myNumber = 11;
    }
}
