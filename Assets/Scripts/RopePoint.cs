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
            moveUpFlag = false;
            this.transform.position = new Vector3(0, this.transform.position.y, 0);
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else if (collider.tag == "UpPoint")
        {
            if (!parent)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
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
                    moveDownFlag = false;
            }
            else
            {
                if (moveDownFlag)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    moveUpFlag = false;
                }
            }
        }
    }

    void RopeUp()
    {
        this.transform.position += new Vector3(0, 0.2f, 0);
        if (this.transform.position.x < -0.5f)
            rb.AddForce(new Vector3(0.001f, 0, 0), ForceMode.Impulse);
        if (this.transform.position.x > 0.5f)
            rb.AddForce(new Vector3(-0.001f, 0, 0), ForceMode.Impulse);
        if (this.transform.position.z < -0.5f)
            rb.AddForce(new Vector3(0, 0, 0.001f), ForceMode.Impulse);
        if (this.transform.position.x > 0.5f)
            rb.AddForce(new Vector3(0, 0, -0.001f), ForceMode.Impulse);
    }

    void RopeDown()
    {
        this.transform.position -= new Vector3(0, 0.2f, 0);
    }
}
