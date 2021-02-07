using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    public int ropePointCount = 0; //RopePointの数
    public RopePoint[] ropePoint = new RopePoint[10];
    private bool checkFlag = false; //マネージャーがセットされたかを確認
    private bool downCheckFlag = false; //降下処理の確認を行うか
    private int ropePointNum; //降下処理用

    void Update()
    {
        if (checkFlag && downCheckFlag)
        {
            if (ropePointNum > 0 && !ropePoint[ropePointNum].KinematicCheck())
            {
                Debug.Log("Next down." + ropePointNum);
                ropePointNum--;
                ropePoint[ropePointNum].moveDownFlag = true;
            }
            else if (ropePointNum == 0 && !ropePoint[0].moveDownFlag)
            {
                Debug.Log("Stop!");
                ArmUnitDownForceStop();
            }
        }
    }

    public void SetManagerToPoint(int num) //RopePointにマネージャーのインスタンスをセット
    {
        for (int i = 0; i < ropePointCount; i++)
            ropePoint[i].GetManager(num);
        checkFlag = true;
    }

    public void ArmUnitDown()
    {
        ropePointNum = ropePointCount - 1;
        downCheckFlag = true;
        ropePoint[ropePointNum].moveDownFlag = true;
    }

    public void ArmUnitDownForceStop()
    {
        downCheckFlag = false;
        for (int i = 0; i < ropePointCount; i++)
        {
            ropePoint[i].moveDownFlag = false;
        }
    }

    public void ArmUnitUp()
    {
        for (int i = 0; i < ropePointCount; i++)
        {
            ropePoint[i].moveUpFlag = true;
        }
    }
}
