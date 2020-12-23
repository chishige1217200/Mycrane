using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBox : MonoBehaviour
{
    public bool rightMoveFlag = false;
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    GameObject craneBoxSupport;

    void Start()
    {
        craneBoxSupport = transform.parent.Find("CraneBoxSupport").gameObject;
    }

    public void GetManager(int num)
    {
        if (num == 1) Debug.Log("1");
        if (num == 2) Debug.Log("2");
        if (num == 3) Debug.Log("3");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit")
        {
            Debug.Log("Hit");
            leftMoveFlag = false;
        }

        if (collider.tag == "RightLimit") rightMoveFlag = false;
        if (collider.tag == "BackgroundLimit") backMoveFlag = false;
        if (collider.tag == "ForegroundLimit") forwardMoveFlag = false;
    }
}
