using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    GameObject groundToken;
    private void Start()
    {
        groundToken = (GameObject)Resources.Load("GroundToken");

        for(int i=-3; i<=3; i++)
        {
            GameObject newGround = Instantiate(groundToken, new Vector3(i, 0, 0), Quaternion.identity); //¶¬
            newGround.transform.localScale = new Vector3(1, Random.Range(0.5f, 1.5f), 1); //•ÏŒ`
            newGround.transform.Rotate(new Vector3(0, 0, Random.Range(-10f, 10f)));
            newGround.transform.parent = transform;
        }
    }
}
