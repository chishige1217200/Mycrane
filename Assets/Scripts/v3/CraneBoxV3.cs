using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBoxV3 : MonoBehaviour
{
    public CraneBoxSound cbs; // モーター駆動音用SEPlayer
    [SerializeField] float moveSpeedX = 0.001f; // 原則SetMoveSpeeds()で与える
    [SerializeField] float moveSpeedZ = 0.001f; // 原則SetMoveSpeeds()で与える
    [SerializeField] float rightLimit = 1f; // 原則SetLimits()で与える
    [SerializeField] float leftLimit = -1f; // 原則SetLimits()で与える
    [SerializeField] float backLimit = 1f; // 原則SetLimits()で与える
    [SerializeField] float frontLimit = -1f; // 原則SetLimits()で与える
    [SerializeField] bool supportDirectionChanger = false; // true:x-move false:z-move
    [SerializeField] GameObject ropeHost;
    private GameObject craneBoxSupport;
    private Coroutine goPositionCoroutine; // 座標指定移動のコルーチン
    private Coroutine rightCoroutine;
    private Coroutine leftCoroutine;
    private Coroutine backCoroutine;
    private Coroutine frontCoroutine;
    private bool[] moveFlags = new bool[4]; // right, left, back, front

    // TODO:Type8用サポートはそのうち

    void Start()
    {
        craneBoxSupport = transform.parent.Find("CraneBoxSupport").gameObject;
    }

    public void SetMoveSpeeds(float moveSpeedX, float moveSpeedZ)
    {
        this.moveSpeedX = moveSpeedX;
        this.moveSpeedZ = moveSpeedZ;
    }

    public void SetLimits(float rightLimit, float leftLimit, float backLimit, float frontLimit)
    {
        this.rightLimit = rightLimit;
        this.leftLimit = leftLimit;
        this.backLimit = backLimit;
        this.frontLimit = frontLimit;
    }

    public bool CheckPos(int mode) // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：GoPosition用
    {
        int checker = 0; // 復帰チェック用
        if (mode == 1 || mode == 2 || mode == 5)
            if (transform.localPosition.x <= leftLimit) checker++;
        if (mode == 2 || mode == 4 || mode == 8)
            if (transform.localPosition.z >= backLimit) checker++;
        if (mode == 1 || mode == 3 || mode == 6)
            if (transform.localPosition.z <= frontLimit) checker++;
        if (mode == 3 || mode == 4 || mode == 7)
            if (transform.localPosition.x >= rightLimit) checker++;
        if (mode == 9)
            if (goPositionCoroutine == null) checker++;

        if (mode <= 4 && checker == 2) return true;         // 該当箇所に復帰したとみなす
        else if (mode >= 5 && checker == 1) return true;    // 該当箇所に復帰したとみなす
        else return false;                                  // 復帰していないとみなす
    }

    public void GoPosition(Vector2 goPoint)
    {
        goPositionCoroutine = StartCoroutine(InternalGoPosition(goPoint));
    }

    public void CancelGoPosition()
    {
        if (goPositionCoroutine != null) StopCoroutine(goPositionCoroutine);
    }

    IEnumerator InternalGoPosition(Vector2 goPoint)
    {
        int checker = 0;
        while (true)
        {
            checker = 0;
            if (Mathf.Abs(transform.localPosition.x - goPoint.x) <= moveSpeedX)
            {
                checker++;
                if (transform.localPosition.x - goPoint.x != 0)
                {
                    moveFlags[0] = false;
                    moveFlags[1] = false;
                    transform.localPosition = new Vector3(goPoint.x, transform.localPosition.y, transform.localPosition.z);
                }
            }
            else
            {
                if (transform.localPosition.x < goPoint.x) RightEvent();
                else if (transform.localPosition.x > goPoint.x) LeftEvent();
            }
            if (Mathf.Abs(transform.localPosition.z - goPoint.y) <= moveSpeedZ)
            {
                checker++;
                if (transform.localPosition.z - goPoint.y != 0)
                {
                    moveFlags[2] = false;
                    moveFlags[3] = false;
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, goPoint.y);
                }
            }
            else
            {
                if (transform.localPosition.z < goPoint.y) BackEvent();
                else if (transform.localPosition.z > goPoint.y) ForwardEvent();
            }

            SendMoveSoundFlag();

            if (checker == 2)
            {
                goPositionCoroutine = null;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    void SendMoveSoundFlag()
    {
        int checker = 0;
        for (int i = 0; i < moveFlags.Length; i++)
        {
            if (moveFlags[i]) checker++;
        }

        if (checker > 0) cbs.MoveSound(true);
        else cbs.MoveSound(false);
    }

    public void Right(bool flag) // CraneBox.csのものとは仕様が異なる．動かすときにtrue，止めるときにfalseで呼び出すこと．
    {
        if (flag) rightCoroutine = StartCoroutine(InternalRight());
        else if (!flag && rightCoroutine != null)
        {
            moveFlags[0] = false;
            SendMoveSoundFlag();
            StopCoroutine(rightCoroutine);
        }
    }

    private void RightEvent()
    {
        if (leftCoroutine != null) StopCoroutine(leftCoroutine);
        if (transform.localPosition.x < rightLimit)
        {
            moveFlags[0] = true;
            transform.localPosition += new Vector3(moveSpeedX, 0, 0);
            if (ropeHost != null) ropeHost.transform.localPosition += new Vector3(moveSpeedX, 0, 0);
            if (!supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(moveSpeedX, 0, 0);
        }
    }

    IEnumerator InternalRight()
    {
        while (true)
        {
            if (transform.localPosition.x > rightLimit)
            {
                moveFlags[0] = false;
                SendMoveSoundFlag();
                rightCoroutine = null;
                yield break;
            }
            RightEvent();
            SendMoveSoundFlag();

            yield return new WaitForFixedUpdate();
        }
    }

    public void Left(bool flag) // CraneBox.csのものとは仕様が異なる．動かすときにtrue，止めるときにfalseで呼び出すこと．
    {
        if (flag) leftCoroutine = StartCoroutine(InternalLeft());
        else if (!flag && leftCoroutine != null)
        {
            moveFlags[1] = false;
            SendMoveSoundFlag();
            StopCoroutine(leftCoroutine);
        }
    }

    private void LeftEvent()
    {
        if (rightCoroutine != null) StopCoroutine(rightCoroutine);
        if (transform.localPosition.x > leftLimit)
        {
            moveFlags[1] = true;
            transform.localPosition -= new Vector3(moveSpeedX, 0, 0);
            if (ropeHost != null) ropeHost.transform.localPosition -= new Vector3(moveSpeedX, 0, 0);
            if (!supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(moveSpeedX, 0, 0);
        }
    }

    IEnumerator InternalLeft()
    {
        while (true)
        {
            if (transform.localPosition.x < leftLimit)
            {
                moveFlags[1] = false;
                SendMoveSoundFlag();
                leftCoroutine = null;
                yield break;
            }
            LeftEvent();
            SendMoveSoundFlag();

            yield return new WaitForFixedUpdate();
        }
    }

    public void Back(bool flag) // CraneBox.csのものとは仕様が異なる．動かすときにtrue，止めるときにfalseで呼び出すこと．
    {
        if (flag) backCoroutine = StartCoroutine(InternalBack());
        else if (!flag && backCoroutine != null)
        {
            moveFlags[2] = false;
            SendMoveSoundFlag();
            StopCoroutine(backCoroutine);
        }
    }

    private void BackEvent()
    {
        if (frontCoroutine != null) StopCoroutine(frontCoroutine);
        if (transform.localPosition.z < backLimit)
        {
            moveFlags[2] = true;
            transform.localPosition += new Vector3(0, 0, moveSpeedZ);
            if (ropeHost != null) ropeHost.transform.localPosition += new Vector3(0, 0, moveSpeedZ);
            if (supportDirectionChanger) craneBoxSupport.transform.localPosition += new Vector3(0, 0, moveSpeedZ);
        }
    }

    IEnumerator InternalBack()
    {
        while (true)
        {
            if (transform.localPosition.z > backLimit)
            {
                moveFlags[2] = false;
                SendMoveSoundFlag();
                backCoroutine = null;
                yield break;
            }
            BackEvent();
            SendMoveSoundFlag();

            yield return new WaitForFixedUpdate();
        }
    }

    public void Forward(bool flag) // CraneBox.csのものとは仕様が異なる．動かすときにtrue，止めるときにfalseで呼び出すこと．
    {
        if (flag) frontCoroutine = StartCoroutine(InternalForward());
        else if (!flag && frontCoroutine != null)
        {
            moveFlags[3] = false;
            SendMoveSoundFlag();
            StopCoroutine(frontCoroutine);
        }
    }

    private void ForwardEvent()
    {
        if (backCoroutine != null) StopCoroutine(backCoroutine);
        if (transform.localPosition.z > frontLimit)
        {
            moveFlags[3] = true;
            transform.localPosition -= new Vector3(0, 0, moveSpeedZ);
            if (ropeHost != null) ropeHost.transform.localPosition -= new Vector3(0, 0, moveSpeedZ);
            if (supportDirectionChanger) craneBoxSupport.transform.localPosition -= new Vector3(0, 0, moveSpeedZ);
        }
    }

    IEnumerator InternalForward()
    {
        while (true)
        {
            if (transform.localPosition.z < frontLimit)
            {
                moveFlags[3] = false;
                SendMoveSoundFlag();
                frontCoroutine = null;
                yield break;
            }
            ForwardEvent();
            SendMoveSoundFlag();

            yield return new WaitForFixedUpdate();
        }
    }

    public void Up(bool flag)
    {
        if (flag) backCoroutine = StartCoroutine(InternalUp());
        else if (!flag && backCoroutine != null)
        {
            moveFlags[2] = false;
            SendMoveSoundFlag();
            StopCoroutine(backCoroutine);
        }
    }

    private void UpEvent()
    {
        if (transform.localPosition.y < backLimit)
        {
            moveFlags[2] = true;
            transform.localPosition += new Vector3(0, moveSpeedZ, 0);
        }
    }

    IEnumerator InternalUp()
    {
        while (true)
        {
            if (transform.localPosition.y > backLimit)
            {
                moveFlags[2] = false;
                SendMoveSoundFlag();
                backCoroutine = null;
                yield break;
            }
            UpEvent();
            SendMoveSoundFlag();

            yield return new WaitForFixedUpdate();
        }
    }

    public void Down(bool flag)
    {
        if (flag) frontCoroutine = StartCoroutine(InternalDown());
        else if (!flag && frontCoroutine != null)
        {
            moveFlags[3] = false;
            SendMoveSoundFlag();
            StopCoroutine(frontCoroutine);
        }
    }

    private void DownEvent()
    {
        if (transform.localPosition.y > frontLimit)
        {
            moveFlags[3] = true;
            transform.localPosition -= new Vector3(0, moveSpeedZ, 0);
        }
    }

    IEnumerator InternalDown()
    {
        while (true)
        {
            if (transform.localPosition.y < frontLimit)
            {
                moveFlags[3] = false;
                SendMoveSoundFlag();
                frontCoroutine = null;
                yield break;
            }
            DownEvent();
            SendMoveSoundFlag();

            yield return new WaitForFixedUpdate();
        }
    }
}
