using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBox : MonoBehaviour
{
    public bool rightMoveFlag = false; // trueのとき，その方向に移動
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    public bool rightRefusedFlag = false; // trueなら、その方向に移動禁止
    public bool leftRefusedFlag = false;
    public bool backRefusedFlag = false;
    public bool forwardRefusedFlag = false;
    public float moveSpeed = 0.001f;
    [SerializeField] bool supportDirectionChanger = false; // true:x-move false:z-move
    [SerializeField] int playerNumber = 1;
    GameObject craneBoxSupport;
    GameObject ropeHost;
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type4Manager _Type4Manager;
    public Vector2 goPoint; // GoPosition関数の目的地
    public bool goPositionFlag = false; // GoPosition関数の実行フラグ
    int craneType = -1;

    void Start()
    {
        craneBoxSupport = transform.parent.Find("CraneBoxSupport").gameObject;
        ropeHost = ropeHost = transform.parent.Find("Rope").gameObject;
    }

    void FixedUpdate()
    {
        if (rightMoveFlag && !rightRefusedFlag) RightMove();
        if (leftMoveFlag && !leftRefusedFlag) LeftMove();
        if (backMoveFlag && !backRefusedFlag) BackMove();
        if (forwardMoveFlag && !forwardRefusedFlag) ForwardMove();
        if (goPositionFlag) GoPosition();
        if (!leftMoveFlag && !backMoveFlag && !forwardMoveFlag && !rightMoveFlag) goPositionFlag = false;
    }

    public void GetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 1) _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2) _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 4) _Type4Manager = transform.root.gameObject.GetComponent<Type4Selecter>().GetManager(playerNumber);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit")
        {
            leftMoveFlag = false;
            leftRefusedFlag = true;
            if (craneType == 1)
            {
                if (_Type1Manager.craneStatus == 2) _Type1Manager.craneStatus = 3;
                _Type1Manager.buttonFlag = false;
            }
        }
        if (collider.tag == "RightLimit")
        {
            rightMoveFlag = false;
            rightRefusedFlag = true;
            if (craneType == 1)
                if (_Type1Manager.craneStatus == 2)
                {
                    _Type1Manager.craneStatus = 3;
                    _Type1Manager.buttonFlag = false;
                }
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
            if (craneType == 4)
                if (_Type4Manager.craneStatus == 2)
                {
                    _Type4Manager.craneStatus = 3;
                    _Type4Manager.buttonFlag = false;
                }
        }
        if (collider.tag == "BackgroundLimit")
        {
            backMoveFlag = false;
            backRefusedFlag = true;
            if (craneType == 1)
            {
                if (_Type1Manager.craneStatus == 4) _Type1Manager.craneStatus = 5;
                _Type1Manager.buttonFlag = false;
            }
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
            if (craneType == 4)
            {
                if (_Type4Manager.craneStatus == 4) _Type4Manager.craneStatus = 5;
                _Type4Manager.buttonFlag = false;
            }
        }
        if (collider.tag == "ForegroundLimit")
        {
            forwardMoveFlag = false;
            forwardRefusedFlag = true;
        }
    }

    void OnTriggerExit(Collider collider) // 限界に達すると，RefusedFlagをTrueに
    {
        if (collider.tag == "LeftLimit") leftRefusedFlag = false;
        if (collider.tag == "RightLimit") rightRefusedFlag = false;
        if (collider.tag == "BackgroundLimit") backRefusedFlag = false;
        if (collider.tag == "ForegroundLimit") forwardRefusedFlag = false;
    }

    public bool CheckPos(int mode) // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：停止状態の移動確認
    {
        int checker = 0; // 復帰チェック用
        if (mode == 1 || mode == 2 || mode == 5)
            if (!leftMoveFlag || leftRefusedFlag) checker++;
        if (mode == 2 || mode == 4 || mode == 8)
            if (!backMoveFlag || backRefusedFlag) checker++;
        if (mode == 1 || mode == 3 || mode == 6)
            if (!forwardMoveFlag || forwardRefusedFlag) checker++;
        if (mode == 3 || mode == 4 || mode == 7)
            if (!rightMoveFlag || rightRefusedFlag) checker++;
        if (mode == 9)
            if (!leftMoveFlag && !backMoveFlag && !forwardMoveFlag && !rightMoveFlag)
            {
                checker++;
                goPositionFlag = false;
            }

        if (mode <= 4 && checker == 2) return true;         // 該当箇所に復帰したとみなす
        else if (mode >= 5 && checker == 1) return true;    // 該当箇所に復帰したとみなす
        else return false;                                  // 復帰していないとみなす
    }

    void GoPosition()
    {
        if (this.transform.position.x < goPoint.x)
        {
            leftMoveFlag = false;
            rightMoveFlag = true;
        }
        if (this.transform.position.x > goPoint.x)
        {
            rightMoveFlag = false;
            leftMoveFlag = true;
        }
        if (this.transform.position.z < goPoint.y)
        {
            forwardMoveFlag = false;
            backMoveFlag = true;
        }
        if (this.transform.position.z > goPoint.y)
        {
            backMoveFlag = false;
            forwardMoveFlag = true;
        }

        if (Mathf.Abs(this.transform.position.x - goPoint.x) <= moveSpeed)
        {
            if (this.transform.position.x - goPoint.x != 0)
                this.transform.position = new Vector3(goPoint.x, this.transform.position.y, this.transform.position.z);
            leftMoveFlag = false;
            rightMoveFlag = false;
        }
        if (Mathf.Abs(this.transform.position.z - goPoint.y) <= moveSpeed)
        {
            if (this.transform.position.z - goPoint.y != 0)
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, goPoint.y);
            backMoveFlag = false;
            forwardMoveFlag = false;
        }
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
