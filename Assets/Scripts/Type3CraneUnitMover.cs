using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type3CraneUnitMover : MonoBehaviour
{
    GameObject craneBox;
    //Rigidbody craneBoxRb;
    GameObject craneBoxSupport;
    //Rigidbody craneBoxSupportRb;
    public float moveSpeed = 0.1f;
    public bool rightMoveFlag = false;
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    private GameObject ropeHost;
    Transform temp;

    void Start()
    {

        craneBox = this.transform.Find("CraneBox").gameObject;
        //craneBoxRb = craneBox.GetComponent<Rigidbody>();
        craneBoxSupport = this.transform.Find("CraneBoxSupport").gameObject;
        //craneBoxSupportRb = craneBoxSupport.GetComponent<Rigidbody>();
        temp = this.transform;
        ropeHost = temp.Find("Rope").gameObject;
    }

    void Update()
    {
        /*if (rightMoveFlag) RightMove();
        if (leftMoveFlag) LeftMove();
        if (backMoveFlag) BackMove();
        if (forwardMoveFlag) ForwardMove();*/
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

    /*void RightMove()
    {
        craneBox.transform.position += new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.position += new Vector3(moveSpeed, 0, 0);
    }

    void LeftMove()
    {
        craneBox.transform.position -= new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.position -= new Vector3(moveSpeed, 0, 0);
    }

    void BackMove()
    {
        craneBox.transform.position += new Vector3(0, 0, moveSpeed);
        ropeHost.transform.position += new Vector3(0, 0, moveSpeed);
        craneBoxSupport.transform.position += new Vector3(0, 0, moveSpeed);
    }

    void ForwardMove()
    {
        craneBox.transform.position -= new Vector3(0, 0, moveSpeed);
        ropeHost.transform.position -= new Vector3(0, 0, moveSpeed);
        craneBoxSupport.transform.position -= new Vector3(0, 0, moveSpeed);
    }*/
}
