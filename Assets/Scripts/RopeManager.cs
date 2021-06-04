using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{

    public RopePoint[] ropePoint = new RopePoint[30];
    private bool checkFlag = false; //マネージャーがセットされたかを確認
    private bool downCheckFlag = false; //降下処理の確認を行うか
    private int ropePointNum; //降下処理用

    void FixedUpdate()
    {
        if (checkFlag && downCheckFlag)
        {
            if (ropePointNum > 0 && !ropePoint[ropePointNum].KinematicCheck())
            {
                //Debug.Log("Next down." + ropePointNum);
                ropePointNum--;
                ropePoint[ropePointNum].moveDownFlag = true;
            }
            else if (ropePointNum == 0 && !ropePoint[0].moveDownFlag)
            {
                //Debug.Log("Stop!");
                ArmUnitDownForceStop();
            }
        }
    }

    public void SetManagerToPoint(int num) //RopePointにマネージャーのインスタンスをセット
    {
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].GetManager(num);
        checkFlag = true;
    }

    public void ArmUnitDown()
    {
        ArmUnitUpForceStop();
        ropePointNum = ropePoint.Length - 1;
        downCheckFlag = true;
        ropePoint[ropePointNum].moveDownFlag = true;
    }

    public void ArmUnitDownForceStop()
    {
        downCheckFlag = false;
        for (int i = 0; i < ropePoint.Length; i++)
        {
            ropePoint[i].moveDownFlag = false;
        }
    }

    public void ArmUnitUp()
    {
        ArmUnitDownForceStop();
        for (int i = 0; i < ropePoint.Length; i++)
            ropePoint[i].moveUpFlag = true;
    }
    public void ArmUnitUpForceStop()
    {
        for (int i = 0; i < ropePoint.Length; i++)
        {
            ropePoint[i].moveUpFlag = false;
        }
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
