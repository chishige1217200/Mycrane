using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBox : MonoBehaviour
{
    public bool leftMoveFlag = false;
    public bool rightMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;
    public bool rightRefusedFlag = false; // trueなら、その方向に移動禁止
    public bool leftRefusedFlag = false;
    public bool backRefusedFlag = false;
    public bool forwardRefusedFlag = false;
    public float moveSpeed = 0.001f;
    [SerializeField] bool supportDirectionChanger = false; // true:x-move false:z-move
    GameObject craneBoxSupport;
    GameObject ropeHost;
    public Vector2 goPoint; // GoPosition関数の目的地
    public bool goPositionFlag = false; // GoPosition関数の実行フラグ
    public bool limitIgnoreFlag = false; // trueのとき，refusedFlagの影響を受けない

    void Start()
    {
        craneBoxSupport = transform.parent.Find("CraneBoxSupport").gameObject;
        ropeHost = ropeHost = transform.parent.Find("Rope").gameObject;
    }

    void FixedUpdate()
    {
        if (goPositionFlag) GoPosition();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit") leftRefusedFlag = true;
        if (collider.tag == "RightLimit") rightRefusedFlag = true;
        if (collider.tag == "BackgroundLimit") backRefusedFlag = true;
        if (collider.tag == "ForegroundLimit") forwardRefusedFlag = true;
    }

    void OnTriggerExit(Collider collider) // 限界に達すると，RefusedFlagをTrueに
    {
        if (collider.tag == "LeftLimit") leftRefusedFlag = false;
        if (collider.tag == "RightLimit") rightRefusedFlag = false;
        if (collider.tag == "BackgroundLimit") backRefusedFlag = false;
        if (collider.tag == "ForegroundLimit") forwardRefusedFlag = false;
    }

    public bool CheckPos(int mode) // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：GoPosition用
    {
        int checker = 0; // 復帰チェック用
        if (mode == 1 || mode == 2 || mode == 5)
            if (leftRefusedFlag) checker++;
        if (mode == 2 || mode == 4 || mode == 8)
            if (backRefusedFlag) checker++;
        if (mode == 1 || mode == 3 || mode == 6)
            if (forwardRefusedFlag) checker++;
        if (mode == 3 || mode == 4 || mode == 7)
            if (rightRefusedFlag) checker++;
        if (mode == 9)
            if (!goPositionFlag) checker++; //Bad?

        if (mode <= 4 && checker == 2) return true;         // 該当箇所に復帰したとみなす
        else if (mode >= 5 && checker == 1) return true;    // 該当箇所に復帰したとみなす
        else return false;                                  // 復帰していないとみなす
    }

    void GoPosition()
    {
        int checker = 0;
        if (Mathf.Abs(this.transform.localPosition.x - goPoint.x) <= moveSpeed)
        {
            checker++;
            if (this.transform.localPosition.x - goPoint.x != 0)
                this.transform.localPosition = new Vector3(goPoint.x, this.transform.localPosition.y, this.transform.localPosition.z);
        }
        else
        {
            if (this.transform.localPosition.x < goPoint.x) Right();
            else if (this.transform.localPosition.x > goPoint.x) Left();
        }
        if (Mathf.Abs(this.transform.localPosition.z - goPoint.y) <= moveSpeed)
        {
            checker++;
            if (this.transform.localPosition.z - goPoint.y != 0)
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, goPoint.y);
        }
        else
        {
            if (this.transform.localPosition.z < goPoint.y) Back();
            else if (this.transform.localPosition.z > goPoint.y) Forward();
        }

        Debug.Log(Mathf.Abs(this.transform.localPosition.x - goPoint.x) + "," + Mathf.Abs(this.transform.localPosition.z - goPoint.y));

        if (checker == 2) goPositionFlag = false;
    }

    public void Right()
    {
        if (!rightRefusedFlag || limitIgnoreFlag)
        {
            this.transform.localPosition += new Vector3(moveSpeed, 0, 0);
            ropeHost.transform.localPosition += new Vector3(moveSpeed, 0, 0);
            if (!supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(moveSpeed, 0, 0);
        }
    }

    public void Left()
    {
        if (!leftRefusedFlag || limitIgnoreFlag)
        {
            this.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
            ropeHost.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
            if (!supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
        }
    }

    public void Back()
    {
        if (!backRefusedFlag || limitIgnoreFlag)
        {
            this.transform.localPosition += new Vector3(0, 0, moveSpeed);
            ropeHost.transform.localPosition += new Vector3(0, 0, moveSpeed);
            if (supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(0, 0, moveSpeed);
        }
    }

    public void Forward()
    {
        if (!forwardRefusedFlag || limitIgnoreFlag)
        {
            this.transform.localPosition -= new Vector3(0, 0, moveSpeed);
            ropeHost.transform.localPosition -= new Vector3(0, 0, moveSpeed);
            if (supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(0, 0, moveSpeed);
        }
    }
}
