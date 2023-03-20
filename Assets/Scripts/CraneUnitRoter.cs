using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneUnitRoter : MonoBehaviour
{
    bool rightRefusedFlag = false; // trueなら、その方向に移動禁止
    bool leftRefusedFlag = false;
    public float[] limit = new float[2];　// Left-Right
    [SerializeField] float rotateSpeed = 0.2f; // 度数法 実際の回転数は0.2f * 60 = 12
    float targetEulerAngles = 0f; // 指定角度
    private bool goPositionFlag = false; // true時指定角度に回転完了

    void Update()
    {
        //Debug.Log(transform.localEulerAngles.y);
    }

    void FixedUpdate() // 1秒間に60回呼ばれる
    {

        //if (Input.GetKey(KeyCode.RightArrow)) Right();
        //if (Input.GetKey(KeyCode.LeftArrow)) Left();
    }

    public void RotateToTarget(float targetAngles)
    {
        targetEulerAngles = targetAngles;
        goPositionFlag = true;
        StartCoroutine(RotateToTargetEuler());
    }

    private IEnumerator RotateToTargetEuler()
    {
        while (Mathf.Abs(transform.localEulerAngles.y - targetEulerAngles) >= rotateSpeed)
        {
            if (transform.localEulerAngles.y - targetEulerAngles > 0) Right();
            else if (transform.localEulerAngles.y - targetEulerAngles < 0) Left();
            yield return new WaitForFixedUpdate();
        }
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetEulerAngles, transform.localEulerAngles.z);
        //Debug.Log("Rotate Finish.");
        goPositionFlag = false;
    }

    public bool CheckPos(int mode) // 1：左，2：右，3：指定角度
    {
        if (mode == 1)
            if (leftRefusedFlag) return true;
        if (mode == 2)
            if (rightRefusedFlag) return true;
        if (mode == 3)
            return !goPositionFlag;

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
