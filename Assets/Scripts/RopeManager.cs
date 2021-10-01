using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    [SerializeField] RopePoint[] ropePoint = new RopePoint[30];
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

    public void Down()
    {
        UpForceStop();
        ropePointNum = ropePoint.Length - 1;
        downCheckFlag = true;
        ropePoint[ropePointNum].moveDownFlag = true;
    }

    public void DownForceStop()
    {
        downCheckFlag = false;
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].moveDownFlag = false;
    }

    public void Up()
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

    public void UpForceStop()
    {
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].moveUpFlag = false;
    }

    public bool DownFinished()
    {
        if (!ropePoint[0].moveDownFlag && !ropePoint[0].upRefusedFlag) return true;
        else return false;
    }

    public bool UpFinished()
    {
        if (ropePoint[ropePoint.Length - 1].upRefusedFlag && !ropePoint[ropePoint.Length - 1].moveUpFlag) return true;
        else return false;
    }
}
