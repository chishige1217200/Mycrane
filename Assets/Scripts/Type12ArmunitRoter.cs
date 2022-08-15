using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type12ArmunitRoter : MonoBehaviour
{
    SEPlayer sp;
    public float rotateEulerSpeed = 0.3f;
    bool useSE = false;
    bool rotationFlag = false; //回転中
    public bool rotationDirection = true; //trueなら下向きに回転
    bool rotateInitialFlag = false; //trueなら初期化確認
    Type12Manager manager;

    void Update()
    {
        if (rotationFlag)
        {
            if (transform.localEulerAngles.x <= 75)
            {
                if (rotationDirection && transform.localEulerAngles.z == 180)
                {
                    rotationDirection = false;
                    if (useSE)
                    {
                        sp.Stop(2);
                        manager.Pattern(11);
                        sp.Play(3);
                    }
                }
                else if (!rotationDirection && transform.localEulerAngles.y == 180)
                {
                    rotationDirection = true;
                    if (useSE)
                    {
                        sp.Stop(3);
                        manager.Pattern(10);
                        sp.Play(2);
                    }
                }
            }
        }
        if (rotateInitialFlag && transform.localEulerAngles.x == 90) //位置初期化確認
        {
            RotateStop();
            rotateInitialFlag = false;
        }
    }

    void FixedUpdate()
    {
        if (rotationFlag)
        {
            if (rotationDirection)
                transform.Rotate(rotateEulerSpeed, 0, 0);
            else
                transform.Rotate(-rotateEulerSpeed, 0, 0);
        }
    }

    public void SetSEPlayer(SEPlayer s)
    {
        sp = s;
    }

    public void RotateStart(bool b)
    {
        useSE = b;
        rotationFlag = true;
        rotationDirection = true;
        if (useSE)
        {
            manager.Pattern(10);
            sp.Play(2);
        }
    }

    public void RotateStop()
    {
        rotationFlag = false;
    }

    public void RotateToHome() //0度に向かって回転
    {
        if (transform.localEulerAngles.z == 180)
            rotationDirection = false;
        else if (transform.localEulerAngles.y == 180)
            rotationDirection = true;
        else if (transform.localEulerAngles.x == 90) return;

        rotateInitialFlag = true; //初期化用Updateの実行
        useSE = false;
        rotationFlag = true;
    }

    public void SetManager(Type12Manager m)
    {
        manager = m;
    }
}
