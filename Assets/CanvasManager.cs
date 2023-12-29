using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    const int originWidth = 750;
    int nowWidth;
    static int nowScaleFactor;
    public static int NowScaleFactor => nowScaleFactor;
    void Awake()
    {
        nowWidth = Screen.width;
        nowScaleFactor = nowWidth / originWidth;
        GetComponent<Canvas>().scaleFactor = nowScaleFactor;
        GetComponent<Canvas>().scaleFactor = nowScaleFactor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
