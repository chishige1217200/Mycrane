using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneUnitRoter : MonoBehaviour
{
    bool rightRefusedFlag = false; // trueなら、その方向に移動禁止
    bool leftRefusedFlag = false;
    public float[] limit = new float[2];　// Left-Right
    [SerializeField] float rotateSpeed = 0.2f; // 度数法 実際の回転数は0.2f * 60 = 12

    void Update()
    {
        //Debug.Log(transform.localEulerAngles.y);
    }

    void FixedUpdate() // 1秒間に60回呼ばれる
    {

        //if (Input.GetKey(KeyCode.RightArrow)) Right();
        //if (Input.GetKey(KeyCode.LeftArrow)) Left();
    }

    public bool CheckPos(int mode) // 1：左，2：右
    {
        if (mode == 1)
            if (leftRefusedFlag) return true;
        if (mode == 2)
            if (rightRefusedFlag) return true;

        return false; // 復帰していないとみなす
    }

    public void Left()
    {
        if (rightRefusedFlag) rightRefusedFlag = false;
        if (transform.localEulerAngles.y >= limit[0]) leftRefusedFlag = true;
        if (!leftRefusedFlag) transform.Rotate(0, rotateSpeed, 0);
    }

    public void Right()
    {
        if (leftRefusedFlag) leftRefusedFlag = false;
        if (transform.localEulerAngles.y <= limit[1]) rightRefusedFlag = true;
        if (!rightRefusedFlag) transform.Rotate(0, -rotateSpeed, 0);
    }
}
