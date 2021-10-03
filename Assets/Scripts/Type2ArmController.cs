using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type2ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    bool isOpen = true;
    Transform root;
    ArmControllerSupport support;
    Type2Manager _Type2Manager;

    void Start()
    {
        arm = new GameObject[3];
        joint = new HingeJoint[3];
        motor = new JointMotor[3];
        arm[0] = transform.Find("Arm1").gameObject;
        arm[1] = transform.Find("Arm2").gameObject;
        arm[2] = transform.Find("Arm3").gameObject;
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
                root.localPosition -= new Vector3(0, 0.01f, 0);
            }
        }
        else if (!isOpen)
        {
            if (root.localPosition.y < 0.15f)
            {
                if (root.localPosition.y > 0.15f) root.localPosition = new Vector3(root.localPosition.x, 0.15f, root.localPosition.z);
                root.localPosition += new Vector3(0, 0.01f, 0);
            }
        }
    }

    public void Open()
    {
        for (int i = 0; i < 3; i++)
        {
            isOpen = true;
            motor[i].targetVelocity = 100f;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public async void Release()
    {
        _Type2Manager.armPower = 0f;

        isOpen = true;
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 20f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
        }
        while (support.prizeCount > 0)
            await Task.Delay(50);
        isOpen = false;
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -60f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
        }
    }

    public void Close()
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

    public void SetManager() // 筐体のマネージャー情報取得
    {
        _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        support = transform.Find("Body").GetComponent<ArmControllerSupport>();
    }
}
