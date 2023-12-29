using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitScreenSize : MonoBehaviour
{
    RectTransform rectTransform;
    float width;
    float canvasHeight;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        canvasHeight = transform.parent.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
        rectTransform.sizeDelta = new Vector2(0, -canvasHeight + width / 4.2f); //-canvasHeightが上端の位置でそこから横幅/4.2分だけ下がる この処理によりUIのサイズをスマホの画面幅に限らず一定にする。
        //Debug.Log(canvasHeight + " " + width);

        //この大枠の処理を行ったのち、子要素のリサイジングを行う。
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "DoneText") continue;
            child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
