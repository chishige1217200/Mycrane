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
    public float upSpeed = 0.001f; //上昇速度
    public float downSpeed = 0.001f; //下降速度
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    int craneType = -1;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (moveDownFlag) RopeDown();
        if (moveUpFlag) RopeUp();
    }

    public void GetManager(int num)
    {
        craneType = num;
        if (craneType == 1)
            _Type1Manager = transform.root.gameObject.GetComponent<Type1Manager>();
        if (craneType == 2)
            _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3)
            _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
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
                if (last)
                {
                    if (craneType == 1)
                        if (_Type1Manager.craneStatus == 8) _Type1Manager.craneStatus = 9;
                    if (craneType == 2)
                        if (_Type2Manager.craneStatus == 8) _Type2Manager.craneStatus = 9;
                    if (craneType == 3)
                        if (_Type3Manager.craneStatus == 8) _Type3Manager.craneStatus = 9;
                }
            }
        }
        else if (collider.tag == "UpPoint")
        {
            if (!parent)
                if (moveUpFlag)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "UpLimit")
            if (moveUpFlag)
                moveUpFlag = false;
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
                    if (craneType == 1)
                        if (_Type1Manager.craneStatus == 6) _Type1Manager.craneStatus = 7;
                    if (craneType == 2)
                        if (_Type2Manager.craneStatus == 6) _Type2Manager.craneStatus = 7;
                    if (craneType == 3)
                        if (_Type3Manager.craneStatus == 6) _Type3Manager.craneStatus = 7;
                }
            }
            if (!parent)
            {
                if (moveDownFlag)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    moveDownFlag = false;
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
