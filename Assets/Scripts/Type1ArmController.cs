using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type1ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    JointLimits[] limit;

    void Start()
    {
        arm = new GameObject[2];
        joint = new HingeJoint[2];
        motor = new JointMotor[2];
        limit = new JointLimits[2];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;

        for (int i = 0; i < 2; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            motor[i] = joint[i].motor;
            limit[i] = joint[i].limits;
        }
    }

    public void ArmOpen()
    {
        for (int i = 0; i < 2; i++)
        {
            motor[i].targetVelocity = 100f;
            if (i == 0) motor[i].targetVelocity *= -1;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void ArmClose()
    {
        for (int i = 0; i < 2; i++)
        {
            motor[i].targetVelocity = -100f;
            if (i == 0) motor[i].targetVelocity *= -1;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void ArmFinalClose() // 景品排出後に閉じる時
    {
        for (int i = 0; i < 2; i++)
        {
            motor[i].targetVelocity = -1f;
            if (i == 0) motor[i].targetVelocity *= -1;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void MotorPower(float power)
    {
        //Debug.Log("Change Spring Power " + power);
        for (int i = 0; i < 2; i++)
        {
            motor[i].force = power;
            joint[i].motor = motor[i];
        }
    }

    public void ArmLimit(float armApertures)
    {
        limit[0].min = -40 * armApertures / 100;
        limit[1].max = 40 * armApertures / 100;
        for (int i = 0; i < 2; i++)
            joint[i].limits = limit[i];
    }
}
