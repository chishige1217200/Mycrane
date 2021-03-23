using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    private GameObject mainCamera;      //メインカメラ格納用(0)
    private GameObject rightCamera;     //右カメラ格納用(1)
    private GameObject leftCamera;      //左カメラ格納用(2)
    private int cameraStatus = 0;       //カメラ選択の管理用

    //呼び出し時に実行される関数
    void Start()
    {
        //メインカメラとサブカメラをそれぞれ取得
        mainCamera = GameObject.Find("Main Camera");
        rightCamera = GameObject.Find("Right Camera");
        leftCamera = GameObject.Find("Left Camera");

        //サブカメラを非アクティブにする
        rightCamera.SetActive(false);
        leftCamera.SetActive(false);
    }

    //単位時間ごとに実行される関数
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) ChangeCameraStatus(1);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) ChangeCameraStatus(-1);
        if (cameraStatus == 0) //メインカメラ
        {
            mainCamera.SetActive(true);
            rightCamera.SetActive(false);
            leftCamera.SetActive(false);
        }
        else if (cameraStatus == 1) //右カメラ
        {
            rightCamera.SetActive(true);
            mainCamera.SetActive(false);
            leftCamera.SetActive(false);
        }
        else if (cameraStatus == 2) //左カメラ
        {
            leftCamera.SetActive(true);
            mainCamera.SetActive(false);
            rightCamera.SetActive(false);
        }
    }

    public void ChangeCameraStatus(int num)
    {
        cameraStatus = cameraStatus + num;
        if (cameraStatus >= 3) cameraStatus = 0;
        if (cameraStatus <= -1) cameraStatus = 2;
    }
}
