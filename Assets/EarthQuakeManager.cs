using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//�f�����������ԈႦ���ꍇ�ɁA�y�i���e�B�Ƃ��Ēn�k�𔭐�������N���X�B
public class EarthQuakeManager : MonoBehaviour
{
    static readonly float earthQuakeScale = 1f; //�n�k�̐U���̃X�P�[��
    static readonly float timeScale = 3f; //�n�k�̔g���̃X�P�[��
    static readonly float earthQuakeTime = 1f; //�n�k�̒���

    bool isEarthquakeHappening = false; //���n�k���N���Ă��邩
    int magnitude = 1; //�n�k�̑傫���A�~�X���邲�Ƃɑ傫���Ȃ��Ă����B���̐��l�������A�w���֐��I�Ɏ��g���傫���Ȃ�
    float elapsedEarthQuakeTime = 0; //�n�k�̌o�ߎ���

    GameObject ground;
    Rigidbody2D groundRb;

    //�����I�Ȕg���X�P�[���B2�΂������邱�ƂŁAearthQuakeTime*timeScale�������l�ł���Έړ��ʒu�Ŏ~�܂�悤�ɂ���B
    float ComprehensiveTimeScale => timeScale * Mathf.PI * 2;

    //���s���Ƃɉ��Z�����magnitude��2�悷�邱�ƂŁA�y�i���e�B�̃��X�N�����߂�B
    float ComprehensiveEarthQuakeScale => earthQuakeScale * magnitude * magnitude;

    void Start()
    {
        ground = GameObject.Find("GroundGenerator");
        groundRb = ground.GetComponent<Rigidbody2D>();
    }

    //�n�k�𔭐������郁�\�b�h
    public void TriggerEarthQuake()
    {
        isEarthquakeHappening = true;
        StartCoroutine(SwayUpAndDown());
        StartCoroutine(UpdateElapsedIime());
    }

    //�n�k�I����ɗl�X�ȏ��������s�����\�b�h
    void InitializeEarthQuake()
    {
        isEarthquakeHappening = false;
        elapsedEarthQuakeTime = 0;
        groundRb.velocity = Vector2.zero;
        groundRb.rotation = 0;
        magnitude++;
    }

    //�n�ʂ��㉺�ɗh�炷
    IEnumerator SwayUpAndDown()
    {
        while (isEarthquakeHappening)
        {
            float moveY = Mathf.Sin(elapsedEarthQuakeTime * ComprehensiveTimeScale) * ComprehensiveEarthQuakeScale;
            groundRb.velocity = new Vector2(0, moveY);
            yield return new WaitForEndOfFrame();
        }
    }

    //�n�k���Ԃ��Ǘ�
    IEnumerator UpdateElapsedIime()
    {
        while (isEarthquakeHappening)
        {
            elapsedEarthQuakeTime += Time.deltaTime;
            if (elapsedEarthQuakeTime > earthQuakeTime) InitializeEarthQuake();
            yield return new WaitForEndOfFrame();
        }
    }

}
