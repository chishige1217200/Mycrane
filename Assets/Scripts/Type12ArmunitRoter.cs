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

    void Start()
    {
        Debug.Log(Quaternion.Euler(75, 0, 180));
        Debug.Log(Quaternion.Euler(75, 180, 0));
    }

    void Update()
    {
        if (rotationFlag)
        {
            Debug.Log(transform.localEulerAngles);
            if (transform.localRotation == Quaternion.Euler(75, 0, 180))
            {
                rotationDirection = false;
                if (useSE)
                {
                    sp.Stop(2);
                    sp.Play(3);
                }
            }
            else if (transform.localRotation == Quaternion.Euler(105, 0, 180))
            {
                rotationDirection = true;
                if (useSE)
                {
                    sp.Stop(3);
                    sp.Play(2);
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
        rotationFlag = true;
    }
}
