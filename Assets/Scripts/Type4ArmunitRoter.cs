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
    public bool rotationDirection = true; //trueなら時計回りに回転
    bool rotateInitialFlag = false; //trueなら初期化確認

    void Start()
    {
        joint = gameObject.GetComponent<HingeJoint>();
        motor = joint.motor;
        limit = joint.limits;

        SetLimit(-1, 1);
    }

    void Update()
    {
        if (rotationFlag)
        {
            //Debug.Log(transform.localEulerAngles.z);
            if (transform.localEulerAngles.z <= 270 && transform.localEulerAngles.z >= 265 && rotationDirection)
            {
                rotationDirection = false;
                RotateDirection(rotationDirection);
            }
            else if (transform.localEulerAngles.z >= 90 && transform.localEulerAngles.z <= 95 && !rotationDirection)
            {
                rotationDirection = true;
                RotateDirection(rotationDirection);
            }
            if (rotationDirection)
            {
                _SEPlayer.Stop(4);
                _SEPlayer.Play(3, 2147483647);
            }
            else
            {
                _SEPlayer.Stop(3);
                _SEPlayer.Play(4, 2147483647);
            }
        }
        if (rotateInitialFlag) //位置初期化確認
        {
            if (transform.localEulerAngles.z >= 359.7 || transform.localEulerAngles.z <= 0.3)
            {
                rotateInitialFlag = false;
                motor.targetVelocity = 0f;
                joint.motor = motor;
                SetLimit(-1, 1);
            }
        }
    }

    public void SetSEPlayer(SEPlayer s)
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
        low = Mathf.FloorToInt(transform.localEulerAngles.z); //下方向に丸める
        if (low > 180) low -= 360; //Limitの角度が-180,180でしか設定できないため
        SetLimit(low, low + 1);
    }

    public void RotateToHome() //0度に向かって回転
    {
        if (transform.localEulerAngles.z > 0.3 && transform.localEulerAngles.z <= 91)
        {
            rotationDirection = true; //時計回りに回転
            SetLimit(0, 90);
        }

        else if (transform.localEulerAngles.z < 359.7 && transform.localEulerAngles.z >= 269)
        {
            rotationDirection = false;
            SetLimit(-90, 0);
        }
        else
        {
            SetLimit(-1, 1);
            return;
        }
        RotateDirection(rotationDirection);
        rotateInitialFlag = true; //初期化用Updateの実行
    }

    void RotateDirection(bool direction) //回転指令を実行
    {
        if (direction) motor.targetVelocity = -rotateSpeed;
        else motor.targetVelocity = rotateSpeed;
        joint.motor = motor;
    }

    void SetLimit(int min, int max) //回転領域の制限
    {
        limit.min = min;
        limit.max = max;
        joint.limits = limit;
    }
}
