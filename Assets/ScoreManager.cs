using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] GameObject blockField;
    GameObject afterField;
    GameObject completedField;
    void Awake()
    {
        blockField = GameObject.Find("BlockField");
        afterField = blockField.transform.Find("AfterField").gameObject;
        completedField = blockField.transform.Find("CompletedField").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�S�Q�[���I�u�W�F�N�g�̒��_����ł��������_��y���W��Ԃ����\�b�h
    public float CalculateAllVerticesHeight()
    {
        float max = 0;
        List<Vector3> allVertices = new List<Vector3>();
        foreach(Transform block in afterField.transform)
        {
            max = Mathf.Max(max, CaluculateGameObjectHeight(block.gameObject)); //���݌��Ă���Q�[���I�u�W�F�N�g�̍ł��������_��max�̔�r
            //Debug.Log(block.gameObject.name);
        }
        foreach(Transform block in completedField.transform)
        {
            max = Mathf.Max(max, CaluculateGameObjectHeight(block.gameObject)); //���݌��Ă���Q�[���I�u�W�F�N�g�̍ł��������_��max�̔�r
            //Debug.Log(block.gameObject.name);
        }
        return max;
    }

    //�^����ꂽ�Q�[���I�u�W�F�N�g�̑S���_�̓��A�ł��������_��Ԃ����\�b�h
    float CaluculateGameObjectHeight(GameObject block)
    {
        Vector2[] vertices = block.GetComponent<SpriteRenderer>().sprite.vertices;
        float max = 0;
        foreach(Vector2 vartex in vertices)
        {
            Vector2 worldPoint = block.transform.TransformPoint(vartex);
            max = Mathf.Max(max, worldPoint.y);
            //Debug.Log(worldPoint.y);
        }
        return max;
    }
}
