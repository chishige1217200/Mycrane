using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnarcYmCoord : MonoBehaviour
{
    Type3ManagerV2 m;
    float[] unitCoordinate = new float[2];
    bool[] isPushed = new bool[2];
    public bool pushBeforeStart { get; private set; } = false;

    void Start()
    {
        m = GetComponent<Type3ManagerV2>();
    }

    void Update()
    {
        if (m.host.playable)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                PushButton(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                PushButton(1);
            if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
                ReleaseButton(0);
            if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
                ReleaseButton(1);
            Debug.Log(unitCoordinate[0].ToString() + ", " + unitCoordinate[1].ToString());
        }
    }

    void FixedUpdate()
    {
        if (m.GetStatus() >= 0)
        {
            switch (m.GetStatus())
            {
                case 0:
                case 1:
                case 3:
                case 12:
                    if (m.romVer != 2.1f && isPushed[0]) // 横ズレ(2.1以外)
                        unitCoordinate[0] += m.craneBox.moveSpeed;
                    if (m.romVer != 2.1f && isPushed[1]) // 縦ズレ(2.1以外)
                        unitCoordinate[1] += m.craneBox.moveSpeed;
                    break;
                case 2:
                    if (isPushed[0])
                        unitCoordinate[0] += m.craneBox.moveSpeed;
                    if (m.romVer != 2.1f && isPushed[1])// 縦ズレ(2.1以外)
                        unitCoordinate[1] += m.craneBox.moveSpeed;
                    break;
                case 4:
                    if (m.romVer != 2.1f && isPushed[0]) // 横ズレ(2.1以外)
                        unitCoordinate[0] += m.craneBox.moveSpeed;
                    if (isPushed[1])
                        unitCoordinate[1] += m.craneBox.moveSpeed;
                    break;
                case 10:
                    unitCoordinate[0] -= m.craneBox.moveSpeed;
                    unitCoordinate[1] -= m.craneBox.moveSpeed;
                    break;
            }

            if (m.GetStatus() == 0 || m.GetStatus() == 1 || m.GetStatus() == 12) // 4以上対策コード
                if (m.romVer >= 4.0f && (isPushed[0] || isPushed[1]))
                    pushBeforeStart = true;
        }
    }

    public void SetManager(Type3ManagerV2 manager)
    {
        m = manager;
    }

    public void ResetBeforePush()
    {
        pushBeforeStart = false;
    }

    public void ResetCoordinate()
    {
        unitCoordinate[0] = 0f;
        unitCoordinate[1] = 0f;
    }

    public void PushButton(int num)
    {
        isPushed[num] = true;
    }

    public void ReleaseButton(int num)
    {
        isPushed[num] = false;
        if (unitCoordinate[0] > 0.31f) unitCoordinate[0] = 0.312f;
        if (unitCoordinate[1] > 0.26f) unitCoordinate[0] = 0.26f;
    }

    public bool isOnHome()
    {
        if (unitCoordinate[0] <= 0.16f && unitCoordinate[1] <= 0.15f) // 特定座標より小さいときは獲得口上部付近にある
            return true;
        return false;
    }
}
