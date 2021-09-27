using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type6ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    JointLimits[] limit;
    ArmControllerSupport support;
    Type6Manager _Type6Manager;
    [SerializeField] int playerNumber = 1;
    [SerializeField] float armAperturesBase = 35f;

    void Start()
    {
        arm = new GameObject[3];
        joint = new HingeJoint[3];
        motor = new JointMotor[3];
        limit = new JointLimits[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;

        for (int i = 0; i < 3; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            motor[i] = joint[i].motor;
            limit[i] = joint[i].limits;
        }
    }

    public void ArmOpen()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -100f;
            motor[i].force = 100f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public async void Release()
    {
        _Type6Manager.armPower = 0f;

        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -80f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
        }
        while (support.prizeCount > 0)
            await Task.Delay(50);
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 0f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
        }
    }

    public void ArmClose(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = power;
            motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void MotorPower(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            if (power > 50)
            {
                motor[i].targetVelocity = 100f;
                motor[i].force = power;
                joint[i].motor = motor[i];
            }
            else
            {
                motor[i].targetVelocity = 0.3f * (power - 50);
                motor[i].force = 1f;
                joint[i].motor = motor[i];
            }
        }
    }

    public void ArmLimit(float armApertures)
    {
        limit[0].max = armAperturesBase * armApertures / 100;
        limit[1].max = armAperturesBase * armApertures / 100;
        limit[2].max = armAperturesBase * armApertures / 100;
        for (int i = 0; i < 3; i++)
            joint[i].limits = limit[i];
    }

    public void GetManager() // 筐体のマネージャー情報取得
    {
        _Type6Manager = transform.root.gameObject.GetComponent<Type6Selecter>().GetManager(playerNumber);
        support = this.transform.Find("Main").GetComponent<ArmControllerSupport>();
        support.GetManager(6);
    }
}
