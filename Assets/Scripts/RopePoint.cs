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
    Type4Manager _Type4Manager;
    [SerializeField] int playerNumber = 1;
    int craneType = -1;
    bool upRefusedFlag = false; // 上昇拒否フラグ trueなら上昇禁止

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (moveDownFlag) RopeDown();
        if (moveUpFlag && !upRefusedFlag) RopeUp();
    }

    public void GetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 1) _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2) _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 4) _Type4Manager = transform.root.gameObject.GetComponent<Type4Selecter>().GetManager(playerNumber);
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
                    //Debug.Log("UpFinished.");
                    if (craneType == 1)
                        if (_Type1Manager.craneStatus == 8) _Type1Manager.craneStatus = 9;
                    if (craneType == 2)
                        if (_Type2Manager.craneStatus == 8) _Type2Manager.craneStatus = 9;
                    if (craneType == 3)
                        if (_Type3Manager.craneStatus == 8) _Type3Manager.craneStatus = 9;
                    if (craneType == 4)
                        if (_Type4Manager.craneStatus == 10) _Type4Manager.craneStatus = 11;
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
            {
                upRefusedFlag = true;
                if (last && _Type2Manager.craneStatus == 8) _Type2Manager.craneStatus = 9;
                moveUpFlag = false;
            }
        if (collider.tag == "UpPoint")
            if (moveUpFlag && !upRefusedFlag && !rb.isKinematic)
                rb.isKinematic = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "UpLimit")
            if (moveDownFlag)
                upRefusedFlag = false;

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
                    if (craneType == 4)
                        if (_Type4Manager.craneStatus == 8) _Type4Manager.craneStatus = 9;
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
                rb.AddForce(new Vector3(upSpeed / 2, 0, 0), ForceMode.Impulse);
            if (this.transform.localPosition.x > 0.01f)
                rb.AddForce(new Vector3(-upSpeed / 2, 0, 0), ForceMode.Impulse);
            if (this.transform.localPosition.z < -0.01f)
                rb.AddForce(new Vector3(0, 0, upSpeed / 2), ForceMode.Impulse);
            if (this.transform.localPosition.z > 0.01f)
                rb.AddForce(new Vector3(0, 0, -upSpeed / 2), ForceMode.Impulse);
        }
        if (rb.isKinematic)
        {
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
        this.transform.localPosition -= new Vector3(0, downSpeed, 0);
    }

    public bool KinematicCheck()
    {
        return rb.isKinematic;
    }

    public bool GravityCheck()
    {
        return rb.useGravity;
    }
}
