using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11RopeDowner : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] float downSpeed = 0.0015f;
    bool isTouched = false;

    void FixedUpdate()
    {
        if (isTouched)
            root.transform.localPosition -= new Vector3(0, downSpeed, 0);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("PrizeJudge"))
        {
            isTouched = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("PrizeJudge"))
        {
            isTouched = false;
        }
    }
}
