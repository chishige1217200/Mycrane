using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnStretcher : MonoBehaviour
{
    public bool stretchRefusedFlag = false;
    public bool shrinkRefusedFlag = false;
    LockOnManager l;

    void Start()
    {
        l = transform.parent.parent.parent.GetComponent<LockOnManager>();
    }

    public bool CheckPos(int mode) // 1:奥 2:手前
    {
        if (mode == 1 && stretchRefusedFlag) return true;
        if (mode == 2 && shrinkRefusedFlag) return true;
        return false;
    }

    public void Stretch()
    {
        if (!stretchRefusedFlag)
        {
            shrinkRefusedFlag = false;
            if (transform.localPosition.z < 1) transform.localPosition += new Vector3(0, 0, 0.01f);
            else stretchRefusedFlag = true;
        }
    }

    public void Shrink()
    {
        if (!shrinkRefusedFlag)
        {
            stretchRefusedFlag = false;
            if (transform.localPosition.z > 0) transform.localPosition -= new Vector3(0, 0, 0.01f);
            else shrinkRefusedFlag = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("prize")) l.GetPrize();
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collision Enter");
        Debug.Log(collider.tag);
        if (collider.CompareTag("Shield")) stretchRefusedFlag = true;
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("Collision End");
        if (collider.CompareTag("Shield")) stretchRefusedFlag = false;
    }
}
