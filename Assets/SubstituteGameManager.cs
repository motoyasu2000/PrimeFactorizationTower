using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstituteGameManager : MonoBehaviour
{
    [SerializeField] GameObject gameManager;

    private void Awake()
    {
        if (GameObject.Find("GameManager") == null) gameManager.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
