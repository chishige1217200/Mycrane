using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11MeshRenActivator : MonoBehaviour
{
    MeshRenderer mr;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("RopeMRActive"))
            mr.enabled = true;
    }
}
