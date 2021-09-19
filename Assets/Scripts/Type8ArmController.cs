using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type8ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    bool isOpen = false;
    Transform root;
    void Start()
    {
        arm = new GameObject[3];
        joint = new HingeJoint[3];
        motor = new JointMotor[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;
        root = transform.Find("Body").Find("piston");

        for (int i = 0; i < 3; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            motor[i] = joint[i].motor;
        }
    }

    void FixedUpdate()
    {
        if (isOpen)
        {
            if (root.localPosition.y > 0f)
            {
                if (root.localPosition.y < 0f) root.localPosition = new Vector3(root.localPosition.x, 0, root.localPosition.z);
                root.localPosition -= new Vector3(0, 0.001f, 0);
            }
        }
        else if (!isOpen)
        {
            if (root.localPosition.y < 0.01f)
            {
                if (root.localPosition.y > 0.01f) root.localPosition = new Vector3(root.localPosition.x, 0.01f, root.localPosition.z);
                root.localPosition += new Vector3(0, 0.001f, 0);
            }
        }
    }

    public void ArmOpen()
    {
        isOpen = true;
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 200f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void ArmClose()
    {
        isOpen = false;
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -100f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void MotorPower(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -2f * power;
            joint[i].motor = motor[i];
        }
    }
}
