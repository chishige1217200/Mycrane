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
    public float upRotation = 0f;
    public float downRotation = 0f;
    public float shrinkSpringPower = 0f;
    public float stretchSpringPower = 0f;
    public int targetAxis = 0; // 0:x, 1:y, 2:z
    public bool upRefusedFlag { get; private set; } = false; // 上昇拒否フラグ trueなら上昇禁止
    public bool downRefusedFlag { get; private set; } = false; // 下降拒否フラグ trueなら下降禁止
    private ConfigurableJoint cj;
    private JointDrive jd;
    private Rigidbody rb;

    void Start()
    {
        rb = bottomTarget.GetComponent<Rigidbody>();
        cj = bottomTarget.GetComponent<ConfigurableJoint>();
        if (targetAxis == 0) jd = cj.angularXDrive;
        else jd = cj.angularYZDrive;
    }

    void Update()
    {
        if (moveDownFlag && Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) >= maxDistance) downRefusedFlag = true;
        if (moveUpFlag && Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) <= minDistance)
        {
            upRefusedFlag = true;
            switch (targetAxis)
            {
                case 0:
                    jd.positionSpring = shrinkSpringPower;
                    cj.angularXDrive = jd;
                    break;
                case 1:
                case 2:
                    jd.positionSpring = shrinkSpringPower;
                    cj.angularYZDrive = jd;
                    break;
            }
        }
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
        switch (targetAxis)
        {
            case 0:
                cj.targetRotation = new Quaternion(upRotation, 0, 0, 1);
                break;
            case 1:
                cj.targetRotation = new Quaternion(0, upRotation, 0, 1);
                break;
            case 2:
                cj.targetRotation = new Quaternion(0, 0, upRotation, 1);
                break;
        }
    }

    void InternalDown()
    {
        if (downRefusedFlag) moveDownFlag = false;
        rb.WakeUp();
        cj.connectedAnchor -= new Vector3(0, downSpeed, 0);
        switch (targetAxis)
        {
            case 0:
                cj.targetRotation = new Quaternion(downRotation, 0, 0, 1);
                jd.positionSpring = stretchSpringPower;
                cj.angularXDrive = jd;
                break;
            case 1:
                cj.targetRotation = new Quaternion(0, downRotation, 0, 1);
                jd.positionSpring = stretchSpringPower;
                cj.angularYZDrive = jd;
                break;
            case 2:
                cj.targetRotation = new Quaternion(0, 0, downRotation, 1);
                jd.positionSpring = stretchSpringPower;
                cj.angularYZDrive = jd;
                break;
        }
    }
}
