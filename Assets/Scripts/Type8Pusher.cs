using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type8Pusher : MonoBehaviour
{
    [SerializeField] bool forward = true;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!forward && transform.localPosition.z >= 0.25f) forward = true;
        else if (forward && transform.localPosition.z <= 0.11f) forward = false;
    }

    void FixedUpdate()
    {
        if (forward && rb.velocity.z >= -0.05f) rb.AddForce(new Vector3(0, 0, -10f));
        if (!forward && rb.velocity.z <= 0.05f) rb.AddForce(new Vector3(0, 0, 10f));
    }
}
