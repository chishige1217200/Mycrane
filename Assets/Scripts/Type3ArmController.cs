using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type3ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointSpring[] spring;

    void Start()
    {
        int i = 0;

        arm = new GameObject[3];
        joint = new HingeJoint[3];
        spring = new JointSpring[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;

        for (i = 0; i < 3; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            spring[i] = joint[i].spring;
        }
    }

    public void ArmOpen()
    {
        for (int i = 0; i < 3; i++)
        {
            joint[i].useMotor = true;
        }
    }

    public void ArmClose()
    {
        for (int i = 0; i < 3; i++)
            joint[i].useMotor = false;
    }

    public void SpringPower(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            spring[i].spring = 4.5f * power / 100f + 0.5f;
            joint[i].spring = spring[i];
        }
    }
}
