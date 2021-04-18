﻿using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmunitRoter : MonoBehaviour
{
    HingeJoint joint;
    JointMotor motor;
    JointLimits limit;
    SEPlayer _SEPlayer;
    public float rotateSpeed = 20f;
    bool rotationFlag = false; //回転中
    [SerializeField] bool rotationDirection = true; //trueなら時計回りに回転
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
    async void Update()
    {
        if (rotationFlag)
        {
            Debug.Log(this.transform.localRotation.z);
            if (this.transform.localEulerAngles.z <= 270 && this.transform.localEulerAngles.z >= 90)
            {
                Debug.Log("Reached limit.");
                if (!instanceFlag)
                {
                    instanceFlag = true;
                    rotationDirection = !rotationDirection;
                    await Task.Delay(500);
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
    }

    public void GetSEPlayer(SEPlayer s)
    {
        _SEPlayer = s;
    }

    public void RotateStart()
    {
        SetLimit(-91, 91);
        rotationDirection = true;
        RotateDirection(rotationDirection);
        rotationFlag = true;
    }

    public void RotateStop()
    {
        rotationFlag = false;
        motor.targetVelocity = 0f;
        joint.motor = motor;
        SetLimit(Mathf.FloorToInt(this.transform.localRotation.z), Mathf.CeilToInt(this.transform.localRotation.z));
        _SEPlayer.StopSE(3);
        _SEPlayer.StopSE(4);
    }

    public void RotateToHome()
    {



        SetLimit(0, 0);
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