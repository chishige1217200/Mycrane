using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    [SerializeField] bool forward = true;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (forward && rb.velocity.z >= -0.05f) rb.AddForce(new Vector3(0, 0, -10f));
        if (!forward && rb.velocity.z <= 0.05f) rb.AddForce(new Vector3(0, 0, 10f));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("PusherStop")) forward = !forward;
    }
}
