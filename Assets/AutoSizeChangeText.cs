using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSizeChangeText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float px = gameObject.transform.parent.localScale.x;
        float py = gameObject.transform.parent.localScale.y;
        float pz = gameObject.transform.parent.localScale.z;
        gameObject.transform.localScale = new Vector3(1/px,1/py,1/pz);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
