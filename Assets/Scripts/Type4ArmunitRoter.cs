using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type4ArmunitRoter : MonoBehaviour
{
    HingeJoint joint;
    JointMotor motor;
    JointLimits limit;
    SEPlayer _SEPlayer;
    public float rotateSpeed = 20f;
    bool rotationFlag = false; //回転中
    bool rotationDirection = true; //trueなら時計回りに回転
    bool rotateInitialFlag = false; //trueなら初期化確認
    bool instanceFlag = false; //1回だけ実行するのに使用

    // Start is called before the first frame update
    void Start()
    {
        joint = this.gameObject.GetComponent<HingeJoint>();
        motor = joint.motor;
        limit = joint.limits;

        //SetLimit(-90, 90);
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationFlag)
        {
            if (this.transform.localEulerAngles.z <= 270 && this.transform.localEulerAngles.z >= 90)
            {
                if (!instanceFlag)
                {
                    instanceFlag = true;
                    rotationDirection = !rotationDirection;
                    RotateDirection(rotationDirection);
                }
            }
            else instanceFlag = false;
            if (rotationDirection)
            {
                _SEPlayer.StopSE(4);
                _SEPlayer.PlaySE(3, 2147483647);
            }
            else
            {
                _SEPlayer.StopSE(3);
                _SEPlayer.PlaySE(4, 2147483647);
            }
        }
        //if()
    }

    public void GetSEPlayer(SEPlayer s)
    {
        _SEPlayer = s;
    }

    public void RotateStart()
    {
        SetLimit(-90, 90);
        rotationDirection = true; //時計回りに回転
        RotateDirection(rotationDirection);
        rotationFlag = true;
    }

    public void RotateStop()
    {
        int low; //角度の小さい側
        rotationFlag = false;
        motor.targetVelocity = 0f;
        joint.motor = motor;
        low = Mathf.FloorToInt(this.transform.localEulerAngles.z);
        if (low > 180) low -= 360;
        SetLimit(low, low + 1);
        _SEPlayer.StopSE(3);
        _SEPlayer.StopSE(4);
    }

    public void RotateToHome() //0度に向かって回転
    {
        if (this.transform.localEulerAngles.z > 1)
            rotationDirection = true; //時計回りに回転
        else if (this.transform.localEulerAngles.z < -1)
            rotationDirection = false;
        else
        {
            SetLimit(-1, 1);
            return;
        }
        RotateDirection(rotationDirection);
        rotateInitialFlag = true; //初期化用Updateの実行
    }

    void RotateDirection(bool direction)
    {
        if (direction) motor.targetVelocity = -rotateSpeed;
        else motor.targetVelocity = rotateSpeed;
        joint.motor = motor;
    }

    void SetLimit(int min, int max)
    {
        limit.min = min;
        limit.max = max;
        joint.limits = limit;
    }
}
