using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type3ArmControllerV3 : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] motor;
    ArmControllerSupportV3 support;
    Type3Manager _Type3Manager;
    Type7Manager _Type7Manager;
    Type10ManagerV2 _Type10Manager;
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
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void Close()
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = -50f;
            joint[i].motor = motor[i];
            joint[i].useMotor = true;
        }
    }

    public void Release()
    {
        if (autoPower)
        {
            if (craneType == 3) _Type3Manager.armPower = 0f;
            else if (craneType == 7) _Type7Manager.armPower = 0f;
            else _Type10Manager.armPower = 0f;
            for (int i = 0; i < 3; i++)
            {
                motor[i].targetVelocity = 50f;
                joint[i].motor = motor[i];
            }
            StartCoroutine(InternalRelease());
        }
    }

    IEnumerator InternalRelease()
    {
        while (support.prizeCount > 0)
            yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 0f;
            joint[i].motor = motor[i];
        }
    }

    public void MotorPower(float power)
    {
        for (int i = 0; i < 3; i++)
        {
            motor[i].targetVelocity = 50f - power;
            joint[i].motor = motor[i];
        }
    }

    public void SetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 7) _Type7Manager = transform.root.gameObject.GetComponent<Type7Manager>();
        if (craneType == 10) _Type10Manager = transform.parent.parent.gameObject.GetComponent<Type10ManagerV2>();
        support = transform.Find("Head").Find("Hat").GetComponent<ArmControllerSupportV3>();
        support.SetArmController(craneType);
    }
}
