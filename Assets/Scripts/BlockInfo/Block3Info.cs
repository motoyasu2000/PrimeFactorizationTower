using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block3Info : BlockInfo
{
    public override void SetSelfPrefab()
    {
        selfPrefab = (GameObject)Resources.Load("ThreeBlock");
    }
    public override void SetMyNumber()
    {
        myNumber = 3;
    }
}
