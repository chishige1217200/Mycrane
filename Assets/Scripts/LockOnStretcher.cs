using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnStretcher : MonoBehaviour
{
    public bool stretchRefusedFlag = false;
    public bool shrinkRefusedFlag = false;
    LockOnManager l;
    public float stretchLimit = 0.95f;
    public float shrinkLimit = 0;
    public float moveSpeed = 0.01f;

    void Start()
    {
        l = transform.parent.parent.parent.GetComponent<LockOnManager>();

        if(shrinkLimit >= stretchLimit) Debug.LogWarning("正常に伸び縮みしない可能性があります");
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
            if (transform.localPosition.z < stretchLimit) transform.localPosition += new Vector3(0, 0, moveSpeed);
            else stretchRefusedFlag = true;
        }
    }

    public void Shrink()
    {
        if (!shrinkRefusedFlag)
        {
            stretchRefusedFlag = false;
            if (transform.localPosition.z > shrinkLimit) transform.localPosition -= new Vector3(0, 0, moveSpeed);
            else shrinkRefusedFlag = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("prize")) l.GetPrize();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Shield")) stretchRefusedFlag = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Shield")) stretchRefusedFlag = false;
    }
}
