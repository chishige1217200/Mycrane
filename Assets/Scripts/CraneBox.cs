using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBox : MonoBehaviour
{
    public bool rightMoveFlag = false;
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    public float moveSpeed = 0.001f;
    public bool supportDirectionChanger = false; // true:x-move false:z-move
    GameObject craneBoxSupport;
    GameObject ropeHost;
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    int craneType = -1;

    void Start()
    {
        craneBoxSupport = transform.parent.Find("CraneBoxSupport").gameObject;
        ropeHost = ropeHost = transform.parent.Find("Rope").gameObject;
    }

    void FixedUpdate()
    {
        if (rightMoveFlag) RightMove();
        if (leftMoveFlag) LeftMove();
        if (backMoveFlag) BackMove();
        if (forwardMoveFlag) ForwardMove();
    }

    public void GetManager(int num)
    {
        craneType = num;
        if (craneType == 1)
            _Type1Manager = transform.root.gameObject.GetComponent<Type1Manager>();
        if (craneType == 2)
            _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3)
            _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit")
            leftMoveFlag = false;
        if (collider.tag == "RightLimit")
        {
            rightMoveFlag = false;
            if (craneType == 1)
                if (_Type1Manager.craneStatus == 2) _Type1Manager.craneStatus = 3;
            if (craneType == 2)
                if (_Type2Manager.craneStatus == 2)
                {
                    _Type2Manager.craneStatus = 3;
                    _Type2Manager.buttonFlag = false;
                }
            if (craneType == 3)
                if (_Type3Manager.craneStatus == 2)
                {
                    _Type3Manager.craneStatus = 3;
                    _Type3Manager.buttonFlag = false;
                }
        }

        if (collider.tag == "BackgroundLimit")
        {
            backMoveFlag = false;
            if (craneType == 1)
                if (_Type1Manager.craneStatus == 4) _Type1Manager.craneStatus = 5;
            if (craneType == 2)
            {
                if (_Type2Manager.craneStatus == 4) _Type2Manager.craneStatus = 5;
                _Type2Manager.buttonFlag = false;
            }
            if (craneType == 3)
            {
                if (_Type3Manager.craneStatus == 4) _Type3Manager.craneStatus = 5;
                _Type3Manager.buttonFlag = false;
            }

        }
        if (collider.tag == "ForegroundLimit") forwardMoveFlag = false;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "LeftLimit") leftMoveFlag = false;
        if (collider.tag == "RightLimit") rightMoveFlag = false;
        if (collider.tag == "BackgroundLimit") backMoveFlag = false;
        if (collider.tag == "ForegroundLimit") forwardMoveFlag = false;
    }

    public bool CheckHomePos(int mode) // 1:左手前，2：左奥，3：右手前，4：右奥への復帰確認
    {
        int checker = 0; // 復帰チェック用
        if (mode == 1 || mode == 2)
            if (!leftMoveFlag) checker++;
        if (mode == 2 || mode == 4)
            if (!backMoveFlag) checker++;
        if (mode == 1 || mode == 3)
            if (!forwardMoveFlag) checker++;
        if (mode == 3 || mode == 4)
            if (!rightMoveFlag) checker++;

        if (checker == 2) return true;  // 該当箇所に復帰したとみなす
        else return false;              // 復帰していないとみなす
    }


    void RightMove()
    {
        this.transform.localPosition += new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.localPosition += new Vector3(moveSpeed, 0, 0);
        if (!supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(moveSpeed, 0, 0);
    }

    void LeftMove()
    {
        this.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
        if (!supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
    }

    void BackMove()
    {
        this.transform.localPosition += new Vector3(0, 0, moveSpeed);
        ropeHost.transform.localPosition += new Vector3(0, 0, moveSpeed);
        if (supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(0, 0, moveSpeed);
    }

    void ForwardMove()
    {
        this.transform.localPosition -= new Vector3(0, 0, moveSpeed);
        ropeHost.transform.localPosition -= new Vector3(0, 0, moveSpeed);
        if (supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(0, 0, moveSpeed);
    }
}
