using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmUnitLifter : BaseLifter
{
    bool moveUpFlag = false; //上昇中か
    bool moveDownFlag = false; //下降中か
    public float upSpeed = 0.001f; //上昇速度
    public float downSpeed = 0.001f; //下降速度
    public bool upRefusedFlag { get; private set; } = false; // 上昇拒否フラグ trueなら上昇禁止
    public bool downRefusedFlag { get; private set; } = false; // 下降拒否フラグ trueなら下降禁止

    void FixedUpdate()
    {
        if (moveDownFlag) InternalDown();
        if (moveUpFlag) InternalUp();
    }

    public override void Down()
    {
        UpForceStop();
        moveDownFlag = true;
    }

    public override void DownForceStop()
    {
        moveDownFlag = false;
    }

    public override void Up()
    {
        DownForceStop();
        moveUpFlag = true;
    }

    public override void UpForceStop()
    {
        moveUpFlag = false;
    }

    public override bool DownFinished()
    {
        return downRefusedFlag;
    }

    public override bool UpFinished()
    {
        return upRefusedFlag;
    }

    void InternalUp()
    {
        if (upRefusedFlag) moveUpFlag = false;
        //if (transform.localPosition.y >= 0.02f) transform.localPosition = new Vector3(0, 0, 0);
        transform.localPosition += new Vector3(0, upSpeed, 0);
    }

    void InternalDown()
    {
        if (downRefusedFlag) moveDownFlag = false;
        transform.localPosition -= new Vector3(0, downSpeed, 0);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Tube"))
        {
            if (collider.GetComponent<Tube>().parent)
            {
                //Debug.Log("Top Stop");
                upRefusedFlag = true;
            }
            else if (collider.GetComponent<Tube>().last)
                downRefusedFlag = false;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Tube"))
        {
            if (collider.GetComponent<Tube>().parent)
                upRefusedFlag = false;
            else if (collider.GetComponent<Tube>().last)
                downRefusedFlag = true;
        }
    }
}
