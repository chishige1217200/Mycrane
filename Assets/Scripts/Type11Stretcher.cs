using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11Stretcher : MonoBehaviour
{
    public bool stretchRefusedFlag = false;
    public bool shrinkRefusedFlag = false;

    public bool CheckPos(int mode) // 1:奥 2:手前
    {
        if (mode == 1 && stretchRefusedFlag) return true;
        if (mode == 2 && shrinkRefusedFlag) return true;
        return false;
    }

    public void Stretch()
    {
        if (!stretchRefusedFlag)
        {
            shrinkRefusedFlag = false;
            if (transform.localPosition.z < 1f) transform.localPosition += new Vector3(0, 0, 0.012f);
            else stretchRefusedFlag = true;
        }
    }

    public void Shrink()
    {
        if (!shrinkRefusedFlag)
        {
            stretchRefusedFlag = false;
            if (transform.localPosition.z > 0.1f) transform.localPosition -= new Vector3(0, 0, 0.012f);
            else shrinkRefusedFlag = true;
        }
    }
}
