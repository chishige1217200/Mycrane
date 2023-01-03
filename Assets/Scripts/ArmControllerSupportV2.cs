using System;
using System.Collections;
using UnityEngine;

public class ArmControllerSupportV2 : MonoBehaviour
{
    CraneManagerV2 craneManager;
    BaseLifter lifter;
    public int pushTime = 0;
    public int prizeCount = 0; // プライズがアームにいくつ検知されているか
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "DownLimit")
        {
            switch (craneManager.GetCType())
            {
                case 5:
                    if (craneManager.craneStatus == 6)
                    {
                        Debug.Log("下降制限に接触");
                        lifter.DownForceStop();
                        craneManager.craneStatus = 7;
                    }
                    break;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "prize")
        {
            Debug.Log("prize outTrigger");
            prizeCount--;
            if (prizeCount < 0) prizeCount = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "prize")
        {
            Debug.Log("景品に接触");
            StartCoroutine(DelayCoroutine(pushTime, () =>
            {
                switch (craneManager.GetCType())
                {
                    case 5:
                        if (craneManager.craneStatus == 6)
                        {
                            lifter.DownForceStop();
                            craneManager.craneStatus = 7;
                        }
                        break;
                }
            }));
        }
    }

    public void SetManager(CraneManagerV2 c)
    {
        craneManager = c;
    }

    public void SetLifter(BaseLifter r)
    {
        lifter = r;
    }
}
