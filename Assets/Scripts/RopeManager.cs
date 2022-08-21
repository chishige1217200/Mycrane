using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : BaseLifter
{
    public RopePoint[] ropePoint = new RopePoint[30];
    private bool downCheckFlag = false; //降下処理の確認を行うか
    private int ropePointNum; //降下処理用

    void FixedUpdate()
    {
        if (downCheckFlag)
        {
            if (ropePointNum > 0 && !ropePoint[ropePointNum].rb.isKinematic)
            {
                //Debug.Log("Next down." + ropePointNum);
                ropePointNum--;
                ropePoint[ropePointNum].moveDownFlag = true;
            }
            else if (ropePointNum == 0 && !ropePoint[0].moveDownFlag)
            {
                //Debug.Log("Stop!");
                DownForceStop();
            }
        }
    }

    public override void Down()
    {
        UpForceStop();
        ropePointNum = ropePoint.Length - 1;
        downCheckFlag = true;
        ropePoint[ropePointNum].moveDownFlag = true;
    }

    public override void DownForceStop()
    {
        downCheckFlag = false;
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].moveDownFlag = false;
    }

    public override void Up()
    {
        int checker = 0;
        DownForceStop();
        for (int i = 0; i < ropePoint.Length; i++)
        {
            if (checker == 0 && !ropePoint[i].rb.isKinematic)
            {
                ropePoint[i].SetKinematic();
                checker++;
            }
            ropePoint[i].moveUpFlag = true;
        }
    }

    public override void UpForceStop()
    {
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].moveUpFlag = false;
    }

    public override bool DownFinished()
    {
        if (!ropePoint[0].moveDownFlag && !ropePoint[0].upRefusedFlag) return true;
        else return false;
    }

    public override bool UpFinished()
    {
        if (ropePoint[ropePoint.Length - 1].upRefusedFlag && !ropePoint[ropePoint.Length - 1].moveUpFlag) return true;
        else return false;
    }

    public void SetDownSpeed(float f)
    {
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].downSpeed = f;
    }

    public void SetDownSpeed(float f, int target)
    {
        if (target >= ropePoint.Length || target < 0) return;
        ropePoint[target].downSpeed = f;
    }

    public void SetUpSpeed(float f)
    {
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].upSpeed = f;
    }

    public void SetUpSpeed(float f, int target)
    {
        if (target >= ropePoint.Length || target < 0) return;
        ropePoint[target].upSpeed = f;
    }
}
