using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11RopeRoot : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("RopeMRActive"))
            rb.isKinematic = false;
    }
}
