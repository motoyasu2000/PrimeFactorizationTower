using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    const int originWidth = 750;
    int nowWidth;
    void Awake()
    {
        nowWidth = Screen.width;
        GetComponent<Canvas>().scaleFactor = nowWidth / originWidth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
