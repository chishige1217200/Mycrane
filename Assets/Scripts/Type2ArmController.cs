﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type2ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;

    void Start()
    {
        arm = new GameObject[3];
        joint = new HingeJoint[3];
        motor = new JointMotor[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;

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
            motor[i].targetVelocity = 100f;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void ArmClose()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -100f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void MotorPower(float power)
    {
        //Debug.Log("Change Spring Power " + power);
        for (int i = 0; i < 3; i++)
        {
            if (power > 50)
            {
                motor[i].force = power;
                joint[i].motor = motor[i];
            }
            else
            {
                motor[i].targetVelocity = power - 50f;
                motor[i].force = 1f;
                joint[i].motor = motor[i];
            }
        }
    }
}
