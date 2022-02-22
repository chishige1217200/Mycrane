using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnProbabilityChecker : MonoBehaviour
{
    LockOnProbability l;
    bool inJudge = false;

    public void ResetJudge()
    {
        inJudge = false;
    }

    public bool GetInJudge()
    {
        if (inJudge) Debug.Log("Machine will slip!");
        return inJudge;
    }

    public void IncrimentTarget()
    {
        if (l != null) l.IncrimentTarget();
        else Debug.Log("No Probability Zone");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("JudgePoint")) inJudge = true;
        if (collider.CompareTag("Flag")) l = collider.GetComponent<LockOnProbability>();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("JudgePoint")) inJudge = false;
        if (collider.CompareTag("Flag")) l = null;
    }
}
