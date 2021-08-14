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
    public float upSpeed = 0.001f; //上昇速度
    public float downSpeed = 0.001f; //下降速度
    public bool upRefusedFlag { get; private set; } = false; // 上昇拒否フラグ trueなら上昇禁止
    public bool downRefusedFlag { get; private set; } = false; // 下降拒否フラグ trueなら下降禁止

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (moveDownFlag && !downRefusedFlag) RopeDown();
        if (moveUpFlag && !upRefusedFlag) RopeUp();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {
            if (moveUpFlag)
            {
                moveUpFlag = false;
                upRefusedFlag = true;
                this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);
                this.transform.localRotation = new Quaternion(0, 0, 0, 0);
            }
        }

        if (collider.tag == "UpPoint")
        {
            if (!parent)
            {
                if (moveUpFlag)
                    rb.isKinematic = true;
            }
            else
                downRefusedFlag = false;
        }
    }

    /*void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "UpLimit")
            if (moveUpFlag)
            {
                upRefusedFlag = true;
                moveUpFlag = false;
            }
        if (collider.tag == "UpPoint")
            if (moveUpFlag && !upRefusedFlag && !rb.isKinematic)
                rb.isKinematic = true;
    }*/

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "UpLimit")
            if (moveDownFlag)
                upRefusedFlag = false;

        if (collider.tag == "UpPoint")
        {
            if (moveDownFlag)
            {
                if (!parent) rb.isKinematic = false;
                else downRefusedFlag = true;
                moveDownFlag = false;
            }
        }
    }

    void RopeUp()
    {
        if (upRefusedFlag) moveUpFlag = false;
        if (!rb.isKinematic)
        {
            if (this.transform.localPosition.x < -0.01f)
                rb.AddForce(new Vector3(upSpeed / 2, 0, 0), ForceMode.Impulse);
            if (this.transform.localPosition.x > 0.01f)
                rb.AddForce(new Vector3(-upSpeed / 2, 0, 0), ForceMode.Impulse);
            if (this.transform.localPosition.z < -0.01f)
                rb.AddForce(new Vector3(0, 0, upSpeed / 2), ForceMode.Impulse);
            if (this.transform.localPosition.z > 0.01f)
                rb.AddForce(new Vector3(0, 0, -upSpeed / 2), ForceMode.Impulse);
        }
        else
        {
            this.transform.localPosition += new Vector3(0, upSpeed, 0);
            if (this.transform.localPosition.x < -0.5f)
                this.transform.localPosition += new Vector3(0.5f, 0, 0);
            else if (this.transform.localPosition.x > 0.5f)
                this.transform.localPosition -= new Vector3(0.5f, 0, 0);
            if (this.transform.localPosition.z < -0.5f)
                this.transform.localPosition += new Vector3(0, 0, 0.5f);
            else if (this.transform.localPosition.z > 0.5f)
                this.transform.localPosition -= new Vector3(0, 0, 0.5f);
        }
    }

    void RopeDown()
    {
        if (downRefusedFlag) moveDownFlag = false;
        if (rb.isKinematic) this.transform.localPosition -= new Vector3(0, downSpeed, 0);
    }

    public bool IsKinematic()
    {
        return rb.isKinematic;
    }

    public void SetKinematic()
    {
        rb.isKinematic = true;
    }
}
