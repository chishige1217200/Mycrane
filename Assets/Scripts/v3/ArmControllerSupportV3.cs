using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArmControllerSupportV3 : MonoBehaviour
{
    Type3ArmControllerV3 _Type3ArmController;
    CraneManagerV3 craneManager;
    BaseLifterV3 lifter;
    public int pushTime = 0;
    public int prizeCount = 0; // プライズがアームにいくつ検知されているか
    public bool isShieldcollis = false; // アームがShieldに衝突しているかどうか
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ReleaseCheck")
        {
            switch (craneManager.GetCraneType())
            {
                case 3:
                case 10:
                    if (!craneManager.GetProbability() && craneManager.craneStatus >= 8 && prizeCount > 0)
                    {
                        Debug.Log("Released.");
                        _Type3ArmController.Release();
                    }
                    break;
                case 7:
                    if (!craneManager.GetProbability() && prizeCount > 0)
                    {
                        Debug.Log("Released.");
                        _Type3ArmController.Release();
                    }
                    break;
            }
        }
        if (collider.tag == "DownLimit")
        {
            switch (craneManager.GetCraneType())
            {
                case 1:
                case 3:
                case 5:
                    if (craneManager.craneStatus == 6)
                    {
                        Debug.Log("下降制限に接触");
                        lifter.Down(false);
                        craneManager.craneStatus = 7;
                    }
                    break;
            }
        }
        if (collider.tag == "prize")
        {
            Debug.Log("prize inTrigger");
            prizeCount++;
            if (craneManager == null) return;
            switch (craneManager.GetCraneType())
            {
                case 3:
                case 10:
                    if (craneManager.craneStatus == 6)
                    {
                        if (craneManager.GetCraneType() == 3)
                        {
                            StartCoroutine(DelayCoroutine(700, () =>
                            {
                                if (craneManager.craneStatus == 6)
                                {
                                    lifter.Down(false);
                                    craneManager.craneStatus = 7;
                                }
                            }));
                        }
                        if (craneManager.GetCraneType() == 10)
                        {
                            StartCoroutine(DelayCoroutine(500, () =>
                            {
                                if (craneManager.craneStatus == 6)
                                {
                                    lifter.Down(false);
                                    craneManager.craneStatus = 7;
                                }
                            }));
                        }
                    }
                    break;
            }
        }
        if (collider.tag == "Shield")
        {
            switch (craneManager.GetCraneType())
            {
                case 3:
                case 10:
                    if (craneManager.craneStatus == 6)
                    {
                        if (craneManager.GetCraneType() == 3)
                        {
                            StartCoroutine(DelayCoroutine(1000, () =>
                            {
                                if (craneManager.craneStatus == 6)
                                {
                                    lifter.Down(false);
                                    craneManager.craneStatus = 7;
                                }
                            }));
                        }
                        else if (craneManager.GetCraneType() == 10)
                        {
                            StartCoroutine(DelayCoroutine(300, () =>
                            {
                                if (craneManager.craneStatus == 6)
                                {
                                    lifter.Down(false);
                                    craneManager.craneStatus = 7;
                                }
                            }));
                        }
                    }
                    break;
            }
            isShieldcollis = true;
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
        if (collider.tag == "Shield")
        {
            isShieldcollis = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "prize")
        {
            Debug.Log("景品に接触");
            StartCoroutine(DelayCoroutine(pushTime, () =>
            {
                switch (craneManager.GetCraneType())
                {
                    case 5:
                        if (craneManager.craneStatus == 6)
                        {
                            lifter.Down(false);
                            craneManager.craneStatus = 7;
                        }
                        break;
                }
            }));
        }
    }

    public void SetManager(CraneManagerV3 c)
    {
        craneManager = c;
    }

    public void SetArmController(int num) // Type3ArmControllerから呼ばれる
    {
        if (num == 3 || num == 7 || num == 10)
            _Type3ArmController = transform.parent.parent.gameObject.GetComponent<Type3ArmControllerV3>();
    }

    public void SetLifter(BaseLifterV3 r)
    {
        lifter = r;
    }
}
