using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    public bool parent = false; //一番上の質点かどうか
    public bool last = false; //下から二番目の質点かどうか
    Rigidbody rb; //Rigidbody情報
    public bool moveUpFlag = false; //上昇中か
    public bool moveDownFlag = false; //下降中か
    public bool upCompleteFlag = false; //上昇終了時
    public bool downCompleteFlag = false; //下降終了時
    public float upSpeed = 0.03f; //上昇速度
    public float downSpeed = 0.03f; //下降速度

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
                this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);
                this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                //Debug.Log("enter UpLimit");
                if (last)
                    upCompleteFlag = true;
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
                    //Debug.Log("!parent enter UpPoint");
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
                    downCompleteFlag = true;
                    //Debug.Log("parent exit DownStopPoint");
                }
            }
            if (!parent)
            {
                if (moveDownFlag)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    moveDownFlag = false;
                    //Debug.Log("!parent exit DownStopPoint");
                }
            }
        }
    }

    void RopeUp()
    {
        this.transform.localPosition += new Vector3(0, upSpeed, 0);
        if (!rb.isKinematic)
        {
            if (this.transform.localPosition.x < -0.01f)
                rb.AddForce(new Vector3(0.001f, 0, 0), ForceMode.Impulse);
            if (this.transform.localPosition.x > 0.01f)
                rb.AddForce(new Vector3(-0.001f, 0, 0), ForceMode.Impulse);
            if (this.transform.localPosition.z < -0.01f)
                rb.AddForce(new Vector3(0, 0, 0.001f), ForceMode.Impulse);
            if (this.transform.localPosition.x > 0.01f)
                rb.AddForce(new Vector3(0, 0, -0.001f), ForceMode.Impulse);
        }
        if (rb.isKinematic)
        {
            if (this.transform.localPosition.x < -0.05f)
                this.transform.localPosition += new Vector3(0.05f, 0, 0);
            if (this.transform.localPosition.x > 0.05f)
                this.transform.localPosition -= new Vector3(0.05f, 0, 0);
            if (this.transform.localPosition.z < -0.05f)
                this.transform.localPosition += new Vector3(0, 0, 0.05f);
            if (this.transform.localPosition.x > 0.05f)
                this.transform.localPosition -= new Vector3(0, 0, 0.05f);
        }
    }

    void RopeDown()
    {
        this.transform.localPosition -= new Vector3(0, downSpeed, 0);
    }
}
