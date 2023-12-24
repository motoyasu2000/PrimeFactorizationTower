using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    Vector3 defo; //�����̃J�����̍��W
    float startHeight = 6; //�J�����̈ړ����J�n���鍂��
    public float StartHeight => startHeight;
    float newCameraHeight;
    public float NewCameraHeight => newCameraHeight;
    void Start()
    {
        defo = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreManager.MaxHeight < startHeight) return;
        Camera.main.orthographicSize = scoreManager.MaxHeight - startHeight + 10; //scoreManager.MaxHeight - startHeight�͕ω��ʁA10�͏����l
        newCameraHeight = defo.y + (scoreManager.MaxHeight - startHeight) * 0.3f; //��ʂ̉�30���������Œ肵�ăJ�������g�� //�{��startHeight�����݂��Ȃ������ꍇ���l���Astartheight���������ꍇ�ɂǂ̂悤�ɋt�Z�ł��邩���l�����maxheight�ɂ�0.3���|�����Ă��闝�R���킩��B
        Camera.main.transform.position = new Vector3(defo.x,newCameraHeight, defo.z);
    }
}
