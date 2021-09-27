using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type2ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
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

    public async void Release()
    {
        _Type2Manager.armPower = 0f;

        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 80f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
        }
        while (support.prizeCount > 0)
            await Task.Delay(50);
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -100f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
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

    public void GetManager() // 筐体のマネージャー情報取得
    {
        _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        support = transform.Find("Main").GetComponent<ArmControllerSupport>();
    }
}
