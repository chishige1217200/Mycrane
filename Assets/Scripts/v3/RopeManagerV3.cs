using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManagerV3 : BaseLifterV3
{
    [SerializeField] GameObject topTarget;
    [SerializeField] GameObject bottomTarget;
    public float upSpeed = 0.001f; //上昇速度
    public float downSpeed = 0.001f; //下降速度
    public float minDistance = 0.1f;
    public float maxDistance = 2f;
    public float upRotation = 0f;
    public float downRotation = 0f;
    public float shrinkSpringPower = 0f;
    public float stretchSpringPower = 0f;
    public int targetAxis = 0; // 0:x, 1:y, 2:z
    public bool activateRigidbody = false; // rb.WakeUp()を実行するか
    private ConfigurableJoint cj;
    private JointDrive jd;
    private Rigidbody rb;
    // private Coroutine goPositionCoroutine;
    private Coroutine upCoroutine;
    private Coroutine downCoroutine;

    void Start()
    {
        rb = bottomTarget.GetComponent<Rigidbody>();
        cj = bottomTarget.GetComponent<ConfigurableJoint>();
        if (targetAxis == 0) jd = cj.angularXDrive;
        else jd = cj.angularYZDrive;
    }

    void Update()
    {
        if (upCoroutine == null)
        {
            switch (targetAxis)
            {
                case 0:
                    jd.positionSpring = shrinkSpringPower;
                    cj.angularXDrive = jd;
                    break;
                case 1:
                case 2:
                    jd.positionSpring = shrinkSpringPower;
                    cj.angularYZDrive = jd;
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        if (activateRigidbody && rb != null) rb.WakeUp();
    }

    public void SetMoveSpeeds(float upSpeed, float downSpeed)
    {
        this.upSpeed = upSpeed;
        this.downSpeed = downSpeed;
    }

    public void SetLimits(float maxDistance, float minDistance)
    {
        this.maxDistance = maxDistance;
        this.minDistance = minDistance;
    }

    public override void GoPosition(float height)
    {
        Debug.Log("この機能は現在サポートされていません");
        // goPositionCoroutine = StartCoroutine(InternalGoPosition(height));
    }

    public override void CancelGoPosition()
    {
        Debug.Log("この機能は現在サポートされていません");
        // if (goPositionCoroutine != null) StopCoroutine(goPositionCoroutine);
    }

    IEnumerator InternalGoPosition(float height)
    {
        // int checker = 0;
        // while (true)
        // {
        //     checker = 0;
        //     if (Mathf.Abs(Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position)) <= (upSpeed + downSpeed) / 2)
        //     {
        //         checker++;
        //         if (transform.localPosition.y - height != 0)
        //         {
        //             transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
        //         }
        //         goPositionCoroutine = null;
        //         yield break;
        //     }
        //     else
        //     {
        //         if (transform.localPosition.y < height) UpEvent();
        //         else if (transform.localPosition.y > height) DownEvent();
        //     }

        //     yield return new WaitForFixedUpdate();
        // }

        Debug.Log("この機能は現在サポートされていません");
        yield break;
    }

    public override bool CheckPos(int mode) // 1:上，2：下，3：GoPosition用
    {
        int checker = 0; // 復帰チェック用
        if (mode == 1)
        {
            if (Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) <= minDistance) checker++;
        }
        else if (mode == 2)
        {
            if (Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) >= maxDistance) checker++;
        }
        else if (mode == 3)
        {
            Debug.Log("この機能は現在サポートされていません");
            // if (goPositionCoroutine == null) checker++;
        }

        if (checker == 1) return true;    // 該当箇所に復帰したとみなす
        else return false;                // 復帰していないとみなす
    }

    public override void Up(bool flag)
    {
        if (flag)
        {
            Down(false);
            upCoroutine = StartCoroutine(InternalUp());
        }
        else if (!flag && upCoroutine != null)
        {
            StopCoroutine(upCoroutine);
        }
    }

    private void UpEvent()
    {
        cj.connectedAnchor += new Vector3(0, upSpeed, 0);
        switch (targetAxis)
        {
            case 0:
                cj.targetRotation = new Quaternion(upRotation, 0, 0, 1);
                break;
            case 1:
                cj.targetRotation = new Quaternion(0, upRotation, 0, 1);
                break;
            case 2:
                cj.targetRotation = new Quaternion(0, 0, upRotation, 1);
                break;
        }
    }

    IEnumerator InternalUp()
    {
        while (true)
        {
            if (Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) <= minDistance)
            {
                upCoroutine = null;
                yield break;
            }
            UpEvent();

            yield return new WaitForFixedUpdate();
        }
    }

    public override void Down(bool flag)
    {
        if (flag)
        {
            Up(false);
            downCoroutine = StartCoroutine(InternalDown());
        }
        else if (!flag && downCoroutine != null)
        {
            StopCoroutine(downCoroutine);
        }
    }

    private void DownEvent()
    {
        cj.connectedAnchor -= new Vector3(0, downSpeed, 0);
        switch (targetAxis)
        {
            case 0:
                cj.targetRotation = new Quaternion(downRotation, 0, 0, 1);
                jd.positionSpring = stretchSpringPower;
                cj.angularXDrive = jd;
                break;
            case 1:
                cj.targetRotation = new Quaternion(0, downRotation, 0, 1);
                jd.positionSpring = stretchSpringPower;
                cj.angularYZDrive = jd;
                break;
            case 2:
                cj.targetRotation = new Quaternion(0, 0, downRotation, 1);
                jd.positionSpring = stretchSpringPower;
                cj.angularYZDrive = jd;
                break;
        }
    }

    private IEnumerator InternalDown()
    {
        while (true)
        {
            if (Vector3.Distance(topTarget.transform.position, bottomTarget.transform.position) >= maxDistance)
            {
                downCoroutine = null;
                yield break;
            }
            DownEvent();

            yield return new WaitForFixedUpdate();
        }
    }
}
