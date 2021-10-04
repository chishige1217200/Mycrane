using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type3ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    ArmControllerSupport support;
    Type3Manager _Type3Manager;
    Type7Manager _Type7Manager;
    public bool autoPower = true; //trueなら強制射出
    int craneType = 3;
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

    public void Open()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 150f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void Close()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -50f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    /*public void ArmFinalClose() // 景品排出後に閉じる時
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 0f;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }*/

    public async void Release()
    {
        if (craneType == 3) _Type3Manager.armPower = 0f;
        else if (craneType == 7) _Type7Manager.armPower = 0f;
        if (autoPower)
        {
            for (int i = 0; i < 3; i++)
            {
                motor[i].targetVelocity = 50f;
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
    }

    public void MotorPower(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 50f - power;
            //motor[i].force = 1f;
            joint[i].motor = motor[i];
        }
    }

    public void SetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 7) _Type7Manager = transform.root.gameObject.GetComponent<Type7Manager>();
        support = transform.Find("Head").Find("Hat").GetComponent<ArmControllerSupport>();
        support.SetArmController(craneType);
    }
}
