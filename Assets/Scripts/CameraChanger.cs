using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    GameObject upCamera;
    GameObject downCamera;
    List<GameObject> downCameras;
    List<GameObject> upCameras; // *downCamerasと同じ数存在する必要あり
    bool cameraUpFlag = false; // trueなら上方向からのカメラ
    int cameraStatus = 0;

    //呼び出し時に実行される関数
    void Start()
    {
        downCameras = new List<GameObject>();
        upCameras = new List<GameObject>();
        downCamera = this.transform.Find("Camera").Find("DownCamera").gameObject;
        upCamera = this.transform.Find("Camera").Find("UpCamera").gameObject;

        foreach (Transform child in downCamera.transform)
        {
            downCameras.Add(child.gameObject);
        }

        foreach (Transform child in upCamera.transform)
        {
            upCameras.Add(child.gameObject);
        }

        for (int i = 0; i < downCameras.Count; i++)
        {
            if (i == 0) downCameras[i].SetActive(true);
            else downCameras[i].SetActive(false);
        }

        for (int i = 0; i < upCameras.Count; i++)
        {
            if (i == 0) upCameras[i].SetActive(true);
            else upCameras[i].SetActive(false);
        }

        upCamera.SetActive(cameraUpFlag);

        if (downCameras.Count != upCameras.Count) Debug.Log("downCamerasとupCamerasの個数が一致しません");
    }

    //単位時間ごとに実行される関数
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) ChangeCameraStatus(1);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) ChangeCameraStatus(-1);
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) ChangeCameraStatus(0); // downCameras，upCameras切り替え
    }

    public void ChangeCameraStatus(int num)
    {
        if (num == 0) cameraUpFlag = !cameraUpFlag; // downCameras，upCameras切り替え
        else
        {
            cameraStatus = cameraStatus + num;
            if (cameraStatus < 0) cameraStatus = downCameras.Count - 1;
            if (cameraStatus >= downCameras.Count) cameraStatus = 0;
        }
        ActiveCamera();
    }

    void ActiveCamera()
    {
        for (int i = 0; i < downCameras.Count; i++)
        {
            if (i == cameraStatus) downCameras[i].SetActive(true);
            else downCameras[i].SetActive(false);
        }

        for (int i = 0; i < upCameras.Count; i++)
        {
            if (i == cameraStatus) upCameras[i].SetActive(true);
            else upCameras[i].SetActive(false);
        }

        upCamera.SetActive(cameraUpFlag); // downCameras，upCameras切り替え
    }
}
