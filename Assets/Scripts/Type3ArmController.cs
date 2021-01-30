using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type3ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointSpring[] spring;
    ArmControllerSupport support;

    void Start()
    {
        int i = 0;

        arm = new GameObject[3];
        joint = new HingeJoint[3];
        spring = new JointSpring[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;
        support = this.transform.Find("Hat").GetComponent<ArmControllerSupport>();
        support.GetManager(3);
        support.GetArmController(3);

        for (i = 0; i < 3; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            spring[i] = joint[i].spring;
        }
    }

    public void ArmOpen()
    {
        for (int i = 0; i < 3; i++)
            joint[i].useMotor = true;
    }

    public void ArmClose()
    {
        for (int i = 0; i < 3; i++)
        {
            joint[i].useMotor = false;
            joint[i].useSpring = true;
        }
    }

    public void Release()
    {
        for (int i = 0; i < 2; i++)
            joint[i].useSpring = false;
    }

    /*public void SpringPower(float power)
    {
        Debug.Log("Change Spring Power " + power);
        for (int i = 0; i < 3; i++)
        {
            spring[i].spring = 100f * power / 100f;
            joint[i].spring = spring[i];
        }
    }*/
}
