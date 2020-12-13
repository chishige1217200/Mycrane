using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type3CraneUnitMover : MonoBehaviour
{
    GameObject craneBox;
    Rigidbody craneBoxRb;
    GameObject craneBoxSupport;
    Rigidbody craneBoxSupportRb;
    public float moveSpeed = 0.1f;
    public bool rightMoveFlag = false;
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    private Rigidbody[] ropePointRb;
    private GameObject[] ropePointGO;
    private GameObject ropeHost;
    Transform temp;

    void Start()
    {
        ropePointRb = new Rigidbody[9];
        ropePointGO = new GameObject[9];
        craneBox = this.transform.Find("CraneBox").gameObject;
        craneBoxRb = craneBox.GetComponent<Rigidbody>();
        craneBoxSupport = this.transform.Find("CraneBoxSupport").gameObject;
        craneBoxSupportRb = craneBoxSupport.GetComponent<Rigidbody>();
        temp = this.transform;
        ropeHost = temp.Find("Rope").gameObject;
        /*ropePointGO[0] = temp.Find("Rope").Find("Sphere (0)").gameObject;
        ropePointGO[1] = temp.Find("Rope").Find("Sphere (1)").gameObject;
        ropePointGO[2] = temp.Find("Rope").Find("Sphere (2)").gameObject;
        ropePointGO[3] = temp.Find("Rope").Find("Sphere (3)").gameObject;
        ropePointGO[4] = temp.Find("Rope").Find("Sphere (4)").gameObject;
        ropePointGO[5] = temp.Find("Rope").Find("Sphere (5)").gameObject;
        ropePointGO[6] = temp.Find("Rope").Find("Sphere (6)").gameObject;
        ropePointGO[7] = temp.Find("Rope").Find("Sphere (7)").gameObject;
        ropePointGO[8] = temp.Find("Rope").Find("Sphere (8)").gameObject;

        ropePointRb[0] = temp.Find("Rope").Find("Sphere (0)").GetComponent<Rigidbody>();
        ropePointRb[1] = temp.Find("Rope").Find("Sphere (1)").GetComponent<Rigidbody>();
        ropePointRb[2] = temp.Find("Rope").Find("Sphere (2)").GetComponent<Rigidbody>();
        ropePointRb[3] = temp.Find("Rope").Find("Sphere (3)").GetComponent<Rigidbody>();
        ropePointRb[4] = temp.Find("Rope").Find("Sphere (4)").GetComponent<Rigidbody>();
        ropePointRb[5] = temp.Find("Rope").Find("Sphere (5)").GetComponent<Rigidbody>();
        ropePointRb[6] = temp.Find("Rope").Find("Sphere (6)").GetComponent<Rigidbody>();
        ropePointRb[7] = temp.Find("Rope").Find("Sphere (7)").GetComponent<Rigidbody>();
        ropePointRb[8] = temp.Find("Rope").Find("Sphere (8)").GetComponent<Rigidbody>();*/
    }

    void Update()
    {
        if (rightMoveFlag) RightMove();
        if (leftMoveFlag) LeftMove();
        if (backMoveFlag) BackMove();
        if (forwardMoveFlag) ForwardMove();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit") leftMoveFlag = false;
        if (collider.tag == "RightLimit") rightMoveFlag = false;
        if (collider.tag == "BackgroundLimit") backMoveFlag = false;
        if (collider.tag == "ForegroundLimit") forwardMoveFlag = false;
    }

    void RightMove()
    {
        craneBox.transform.position += new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.position += new Vector3(moveSpeed, 0, 0);
        //craneBoxRb.AddForce(new Vector3(moveSpeed, 0, 0));
        /*for (int i = 0; i <= 8; i++)
        {
            if (ropePointRb[i].isKinematic)
            {
                ropePointGO[i].transform.localPosition += new Vector3(moveSpeed / 60, 0, 0);
            }
        }*/
    }

    void LeftMove()
    {
        craneBox.transform.position -= new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.position -= new Vector3(moveSpeed, 0, 0);
        //craneBoxRb.AddForce(new Vector3(-moveSpeed, 0, 0));
        /*for (int i = 0; i <= 8; i++)
        {
            if (ropePointRb[i].isKinematic)
            {
                ropePointGO[i].transform.localPosition -= new Vector3(moveSpeed / 60, 0, 0);
            }
        }*/
    }

    void BackMove()
    {
        craneBox.transform.position += new Vector3(0, 0, moveSpeed);
        ropeHost.transform.position += new Vector3(0, 0, moveSpeed);
        craneBoxSupport.transform.position += new Vector3(0, 0, moveSpeed);
        //craneBoxRb.AddForce(new Vector3(0, 0, moveSpeed));
        //craneBoxSupportRb.AddForce(new Vector3(0, 0, moveSpeed));

        /*for (int i = 0; i <= 8; i++)
        {
            if (ropePointRb[i].isKinematic)
            {
                ropePointGO[i].transform.localPosition += new Vector3(0, 0, moveSpeed / 60);
            }
        }*/
    }

    void ForwardMove()
    {
        craneBox.transform.position -= new Vector3(0, 0, moveSpeed);
        ropeHost.transform.position -= new Vector3(0, 0, moveSpeed);
        craneBoxSupport.transform.position -= new Vector3(0, 0, moveSpeed);
        //craneBoxRb.AddForce(new Vector3(0, 0, -moveSpeed));
        //craneBoxSupportRb.AddForce(new Vector3(0, 0, -moveSpeed));

        /*for (int i = 0; i <= 8; i++)
        {
            if (ropePointRb[i].isKinematic)
            {
                ropePointGO[i].transform.localPosition -= new Vector3(0, 0, moveSpeed / 60);
            }
        }*/
    }
}
