using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type3CraneUnitMover : MonoBehaviour
{
    GameObject craneBox;
    GameObject craneBoxSupport;
    public bool rightMoveFlag = false;
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    private GameObject ropeHost;
    Transform temp;

    void Start()
    {
        craneBox = this.transform.Find("CraneBox").gameObject;
        craneBoxSupport = this.transform.Find("CraneBoxSupport").gameObject;
        temp = this.transform;
        ropeHost = temp.Find("Rope").gameObject;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit") leftMoveFlag = false;
        if (collider.tag == "RightLimit") rightMoveFlag = false;
        if (collider.tag == "BackgroundLimit") backMoveFlag = false;
        if (collider.tag == "ForegroundLimit") forwardMoveFlag = false;
    }
}
