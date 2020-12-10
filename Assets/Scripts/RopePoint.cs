using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {

        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {

        }
    }
}
