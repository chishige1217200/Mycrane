using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type9ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    JointLimits[] limit;
    [SerializeField] float armAperturesBase = 40f;

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

    public void Open()
    {
        for (int i = 0; i < 2; i++)
        {
            motor[i].targetVelocity = -100f;
            motor[i].force = 100f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void Close(float power)
    {
        for (int i = 0; i < 2; i++)
        {
            motor[i].targetVelocity = power;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void Power(float power, int num)
    {
        if (power > 50)
        {
            motor[num].targetVelocity = 100f;
            motor[num].force = power;
            joint[num].motor = motor[num];
        }
        else
        {
            motor[num].targetVelocity = 0.3f * (power - 50);
            motor[num].force = 1f;
            joint[num].motor = motor[num];
        }
    }

    public void Limit(float armApertures)
    {
        limit[0].min = -armAperturesBase * armApertures / 100;
        limit[1].min = -armAperturesBase * armApertures / 100;
        for (int i = 0; i < 2; i++)
            joint[i].limits = limit[i];
    }
}
