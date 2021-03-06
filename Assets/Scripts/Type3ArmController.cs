﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type3ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    ArmControllerSupport support;

    void Start()
    {
        arm = new GameObject[3];
        joint = new HingeJoint[3];
        motor = new JointMotor[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;
        support = this.transform.Find("Hat").GetComponent<ArmControllerSupport>();
        support.GetManager(3);
        support.GetArmController(3);

        for (int i = 0; i < 3; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            motor[i] = joint[i].motor;
        }
    }

    public void ArmOpen()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -130f;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void ArmClose()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 50f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void ArmFinalClose() // 景品排出後に閉じる時
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -10f;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void Release()
    {
        for (int i = 0; i < 3; i++)
        {
            joint[i].useMotor = false;
        }
    }

    public void MotorPower(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].force = power;
            joint[i].motor = motor[i];
        }
    }
}
