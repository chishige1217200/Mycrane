using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWheelZoomer : MonoBehaviour
{
    Camera playerCamera;
    [SerializeField] FirstPersonAIO fpaio;
    float FOVKickAmountBackup = 0f;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (fpaio == null)
            Debug.LogWarning("FirstPersonAIOがないため，細かい調整機能は無効になります");
        else
            FOVKickAmountBackup = fpaio.advanced.FOVKickAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (fpaio == null) playerCamera.fieldOfView = 60f;
            else fpaio.advanced.FOVKickAmount = FOVKickAmountBackup;
        }


        float wh = Input.GetAxis("Mouse ScrollWheel");
        if (wh > 0)
        {
            if (fpaio != null) fpaio.advanced.FOVKickAmount = 0f;
            if (playerCamera.fieldOfView > 35)
                playerCamera.fieldOfView -= 1;
        }
        else if (wh < 0)
        {
            if (fpaio != null) fpaio.advanced.FOVKickAmount = 0f;
            if (playerCamera.fieldOfView < 85)
                playerCamera.fieldOfView += 1;
        }
        // +ならズームイン，-ならズームアウトするようにする
        // マウスボタンでズームリセットできるとよさそう
        // FOV標準60で10-80くらいに調整したいね
    }
}
