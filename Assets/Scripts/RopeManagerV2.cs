using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManagerV2 : BaseLifter
{
    [SerializeField] GameObject topTarget;
    [SerializeField] GameObject bottomTarget;
    bool moveUpFlag = false; //上昇中か
    bool moveDownFlag = false; //下降中か
    public float upSpeed = 0.001f; //上昇速度
    public float downSpeed = 0.001f; //下降速度
    public float minDistance = 0.1f;
    public float maxDistance = 2f;
    public bool upRefusedFlag { get; private set; } = false; // 上昇拒否フラグ trueなら上昇禁止
    public bool downRefusedFlag { get; private set; } = false; // 下降拒否フラグ trueなら下降禁止
    private ConfigurableJoint cj;
    private Rigidbody rb;

    void Start()
    {
        rb = bottomTarget.GetComponent<Rigidbody>();
        cj = bottomTarget.GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        if (moveDownFlag && Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) >= maxDistance) downRefusedFlag = true;
        if (moveUpFlag && Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) <= minDistance) upRefusedFlag = true;
    }

    void FixedUpdate()
    {
        if (moveDownFlag) InternalDown();
        if (moveUpFlag) InternalUp();
    }

    public override void Down()
    {
        UpForceStop();
        upRefusedFlag = false;
        moveDownFlag = true;
    }

    public override void DownForceStop()
    {
        moveDownFlag = false;
    }

    public override void Up()
    {
        DownForceStop();
        downRefusedFlag = false;
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
        rb.WakeUp();
        cj.connectedAnchor += new Vector3(0, upSpeed, 0);
    }

    void InternalDown()
    {
        if (downRefusedFlag) moveDownFlag = false;
        rb.WakeUp();
        cj.connectedAnchor -= new Vector3(0, downSpeed, 0);
    }
}
