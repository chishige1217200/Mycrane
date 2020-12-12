using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    public bool parent = false; //一番上の質点かどうか
    public bool last = false; //下から二番目の質点かどうか
    Rigidbody rb; //Rigidbody情報
    public bool moveUpFlag = false; //上昇または下降をしているか
    public bool moveDownFlag = false;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (moveDownFlag) RopeDown();
        if (moveUpFlag) RopeUp();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {
            if (moveUpFlag)
            {
                moveUpFlag = false;
                this.transform.position = new Vector3(0, this.transform.position.y, 0);
                this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                Debug.Log("enter UpLimit");
            }
        }
        else if (collider.tag == "UpPoint")
        {
            if (!parent)
            {
                if (moveUpFlag)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    Debug.Log("!parent enter UpPoint");
                }
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "DownStopPoint")
        {
            if (parent)
            {
                if (moveDownFlag)
                {
                    moveDownFlag = false;
                    Debug.Log("parent exit DownStopPoint");
                }
            }
            if (!parent)
            {
                if (moveDownFlag)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    moveDownFlag = false;
                    Debug.Log("!parent exit DownStopPoint");
                }
            }
        }
    }

    void RopeUp()
    {
        this.transform.position += new Vector3(0, 0.2f, 0);
        if (!rb.isKinematic)
        {
            if (this.transform.position.x < -0.01f)
                rb.AddForce(new Vector3(0.001f, 0, 0), ForceMode.Impulse);
            if (this.transform.position.x > 0.01f)
                rb.AddForce(new Vector3(-0.001f, 0, 0), ForceMode.Impulse);
            if (this.transform.position.z < -0.01f)
                rb.AddForce(new Vector3(0, 0, 0.001f), ForceMode.Impulse);
            if (this.transform.position.x > 0.01f)
                rb.AddForce(new Vector3(0, 0, -0.001f), ForceMode.Impulse);
        }
        if (rb.isKinematic)
        {
            if (this.transform.position.x < -0.01f)
                this.transform.position += new Vector3(0.01f, 0, 0);
            if (this.transform.position.x > 0.01f)
                this.transform.position -= new Vector3(0.01f, 0, 0);
            if (this.transform.position.z < -0.01f)
                this.transform.position += new Vector3(0, 0, 0.01f);
            if (this.transform.position.x > 0.01f)
                this.transform.position -= new Vector3(0, 0, 0.01f);
        }
    }

    void RopeDown()
    {
        this.transform.position -= new Vector3(0, 0.2f, 0);
    }
}
