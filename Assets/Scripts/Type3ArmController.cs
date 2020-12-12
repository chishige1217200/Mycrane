using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type3ArmController : MonoBehaviour
{
    GameObject[] arm;
    HingeJoint[] joint;
    JointMotor[] jointmotor;
    JointLimits[] limit;

    async void Start()
    {
        int i = 0;

        arm = new GameObject[3];
        joint = new HingeJoint[3];
        jointmotor = new JointMotor[3];
        limit = new JointLimits[3];
        arm[0] = this.transform.Find("Arm1").gameObject;
        arm[1] = this.transform.Find("Arm2").gameObject;
        arm[2] = this.transform.Find("Arm3").gameObject;

        for (i = 0; i < 3; i++)
        {
            joint[i] = arm[i].GetComponent<HingeJoint>();
            jointmotor[i] = joint[i].motor;
            limit[i] = joint[i].limits;
        }

        /*await Task.Delay(5000);
        motor_on();
        await Task.Delay(5000);
        motor_off();*/


    }

    public void motor_on()
    {
        int i = 0;

        Debug.Log("Motor Activated.");
        for (i = 0; i < 3; i++)
        {
            joint[i].useMotor = true;
            jointmotor[i].force = -100;
            jointmotor[i].targetVelocity = -115;
            //limit[i].max = 180;
        }
    }

    public void motor_off()
    {
        int i = 0;

        Debug.Log("Motor not Activated.");
        for (i = 0; i < 3; i++)
            joint[i].useMotor = false;
    }
}
