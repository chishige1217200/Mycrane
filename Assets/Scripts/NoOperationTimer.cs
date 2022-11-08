using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoOperationTimer : MonoBehaviour
{
    SceneChanger sc;
    float startTime;
    [SerializeField] float thresholdSeconds = 60f;
    // Start is called before the first frame update
    void Start()
    {
        sc = GetComponent<SceneChanger>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) startTime = Time.time;
        if (Time.time - startTime >= 60f) sc.SceneTransition();
    }
}
