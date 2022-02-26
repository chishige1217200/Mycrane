using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CraneBox : MonoBehaviour
{
    bool rightRefusedFlag = false; // trueなら、その方向に移動禁止
    bool leftRefusedFlag = false;
    bool backRefusedFlag = false;
    bool forwardRefusedFlag = false;
    public float moveSpeed = 0.001f;
    [SerializeField] bool supportDirectionChanger = false; // true:x-move false:z-move
    GameObject craneBoxSupport;
    [SerializeField] GameObject ropeHost;
    //[SerializeField] GameObject tubePoint;
    public Vector2 goPoint; // GoPosition関数の目的地
    public bool goPositionFlag = false; // GoPosition関数の実行フラグ
    public bool limitIgnoreFlag = false; // trueのとき，refusedFlagの影響を受けない
    bool leftDummyFlag = false; // 特定の左移動制限を無視
    bool useLeftDummy = false; // 特定の左移動制限無視機能の使用
    public int dummyEnableTime = 100; // ミリ秒

    void Start()
    {
        craneBoxSupport = transform.parent.Find("CraneBoxSupport").gameObject;
        //transform.Find("Rope").TryGetComponent<GameObject>(out ropeHost);
        //ropeHost = transform.parent.Find("Rope").gameObject; //要対策
        //transform.Find("ArmUnit").Find("TubePoint").TryGetComponent<GameObject>(out tubePoint);
        //tubePoint = transform.Find("ArmUnit").Find("TubePoint").gameObject;
    }

    void FixedUpdate()
    {
        if (goPositionFlag) GoPosition();
    }

    void OnTriggerEnter(Collider collider) // 限界に達すると，RefusedFlagをTrueに
    {
        if (collider.CompareTag("LeftLimit")) leftRefusedFlag = true;
        if (collider.CompareTag("RightLimit")) rightRefusedFlag = true;
        if (collider.CompareTag("BackgroundLimit")) backRefusedFlag = true;
        if (collider.CompareTag("ForegroundLimit")) forwardRefusedFlag = true;
        if (collider.CompareTag("LeftLimitDummy"))
        {
            leftRefusedFlag = true;
            useLeftDummy = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("LeftLimit")) leftRefusedFlag = false;
        if (collider.CompareTag("RightLimit")) rightRefusedFlag = false;
        if (collider.CompareTag("BackgroundLimit")) backRefusedFlag = false;
        if (collider.CompareTag("ForegroundLimit")) forwardRefusedFlag = false;
        if (collider.CompareTag("LeftLimitDummy"))
        {
            leftRefusedFlag = false;
            useLeftDummy = false;
        }
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
        if (!limitIgnoreFlag) limitIgnoreFlag = true;
        if (Mathf.Abs(transform.localPosition.x - goPoint.x) <= moveSpeed)
        {
            checker++;
            if (transform.localPosition.x - goPoint.x != 0)
            {
                transform.localPosition = new Vector3(goPoint.x, transform.localPosition.y, transform.localPosition.z);
                //if (tubePoint != null) tubePoint.transform.localPosition = new Vector3(goPoint.x, transform.localPosition.y, transform.localPosition.z);
            }
        }
        else
        {
            if (transform.localPosition.x < goPoint.x) Right();
            else if (transform.localPosition.x > goPoint.x) Left();
        }
        if (Mathf.Abs(transform.localPosition.z - goPoint.y) <= moveSpeed)
        {
            checker++;
            if (transform.localPosition.z - goPoint.y != 0)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, goPoint.y);
                //if (tubePoint != null) tubePoint.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, goPoint.y);
            }
        }
        else
        {
            if (transform.localPosition.z < goPoint.y) Back();
            else if (transform.localPosition.z > goPoint.y) Forward();
        }

        //Debug.Log(Mathf.Abs(this.transform.localPosition.x - goPoint.x) + "," + Mathf.Abs(this.transform.localPosition.z - goPoint.y));

        if (checker == 2)
        {
            goPositionFlag = false;
            if (limitIgnoreFlag) limitIgnoreFlag = false;
        }
    }

    public void Right()
    {
        if (useLeftDummy && !leftDummyFlag && !Input.GetKey(KeyCode.F)) EnableLeftDummy();
        if (!rightRefusedFlag || limitIgnoreFlag)
        {
            transform.localPosition += new Vector3(moveSpeed, 0, 0);
            if (ropeHost != null) ropeHost.transform.localPosition += new Vector3(moveSpeed, 0, 0);
            //if (tubePoint != null) tubePoint.transform.localPosition += new Vector3(moveSpeed, 0, 0);
            if (!supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(moveSpeed, 0, 0);
        }
    }

    public void Left()
    {
        if (!leftRefusedFlag || limitIgnoreFlag || leftDummyFlag)
        {
            transform.localPosition -= new Vector3(moveSpeed, 0, 0);
            if (ropeHost != null) ropeHost.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
            //if (tubePoint != null) tubePoint.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
            if (!supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(moveSpeed, 0, 0);
        }
    }

    public void Back()
    {
        if (!backRefusedFlag || limitIgnoreFlag)
        {
            transform.localPosition += new Vector3(0, 0, moveSpeed);
            if (ropeHost != null) ropeHost.transform.localPosition += new Vector3(0, 0, moveSpeed);
            //if (tubePoint != null) tubePoint.transform.localPosition += new Vector3(0, 0, moveSpeed);
            if (supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(0, 0, moveSpeed);
        }
    }

    public void Forward()
    {
        if (!forwardRefusedFlag || limitIgnoreFlag)
        {
            transform.localPosition -= new Vector3(0, 0, moveSpeed);
            if (ropeHost != null) ropeHost.transform.localPosition -= new Vector3(0, 0, moveSpeed);
            //if (tubePoint != null) tubePoint.transform.localPosition -= new Vector3(0, 0, moveSpeed);
            if (supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(0, 0, moveSpeed);
        }
    }

    public void Up()
    {
        if (!backRefusedFlag || limitIgnoreFlag)
        {
            transform.localPosition += new Vector3(0, moveSpeed, 0);
        }
    }

    public void Down()
    {
        if (!forwardRefusedFlag || limitIgnoreFlag)
        {
            transform.localPosition -= new Vector3(0, moveSpeed, 0);
        }
    }

    async void EnableLeftDummy()
    {
        leftDummyFlag = true;
        await Task.Delay(dummyEnableTime);
        leftDummyFlag = false;
    }
}
