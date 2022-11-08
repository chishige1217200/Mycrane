using System;
using System.Collections;
using UnityEngine;

public class ArmControllerSupport : MonoBehaviour
{
    Type3ArmController _Type3ArmController;
    CraneManager craneManager;
    BaseLifter lifter;
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
            switch (craneManager.GetCType())
            {
                case 3:
                case 10:
                    if (!craneManager.GetProbability() && craneManager.GetStatus() >= 8 && prizeCount > 0)
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
            switch (craneManager.GetCType())
            {
                case 1:
                case 3:
                case 5:
                    if (craneManager.GetStatus() == 6)
                    {
                        Debug.Log("下降制限に接触");
                        lifter.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 4:
                case 12:
                    if (craneManager.GetStatus() == 8)
                    {
                        Debug.Log("下降制限に接触");
                        lifter.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 6:
                    if (craneManager.GetStatus() == 4)
                    {
                        Debug.Log("下降制限に接触");
                        lifter.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
            }
        }
        if (collider.tag == "prize")
        {
            Debug.Log("prize inTrigger");
            prizeCount++;
            if (craneManager == null) return;
            switch (craneManager.GetCType())
            {
                case 3:
                case 10:
                    if (craneManager.GetStatus() == 6)
                    {
                        if (craneManager.GetCType() == 3)
                        {
                            StartCoroutine(DelayCoroutine(700, () =>
                            {
                                if (craneManager.GetStatus() == 6)
                                {
                                    lifter.DownForceStop();
                                    craneManager.IncrimentStatus();
                                }
                            }));
                        }
                        if (craneManager.GetCType() == 10)
                        {
                            StartCoroutine(DelayCoroutine(500, () =>
                            {
                                if (craneManager.GetStatus() == 6)
                                {
                                    lifter.DownForceStop();
                                    craneManager.IncrimentStatus();
                                }
                            }));
                        }
                    }
                    break;
            }
        }
        if (collider.tag == "Shield")
        {
            switch (craneManager.GetCType())
            {
                case 3:
                case 10:
                    if (craneManager.GetStatus() == 6)
                    {
                        if (craneManager.GetCType() == 3)
                        {
                            StartCoroutine(DelayCoroutine(1000, () =>
                            {
                                if (craneManager.GetStatus() == 6)
                                {
                                    lifter.DownForceStop();
                                    craneManager.IncrimentStatus();
                                }
                            }));
                        }
                        else if (craneManager.GetCType() == 10)
                        {
                            StartCoroutine(DelayCoroutine(300, () =>
                            {
                                if (craneManager.GetStatus() == 6)
                                {
                                    lifter.DownForceStop();
                                    craneManager.IncrimentStatus();
                                }
                            }));
                        }
                    }
                    break;
            }
            isShieldcollis = true;
        }
    }

    /*void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "ReleaseCheck")
        {
            switch (craneManager.GetCType())
            {
                case 3:
                case 7:
                    if (!craneManager.GetProbability() && craneManager.GetStatus() >= 8 && prizeCount > 0)
                    {
                        _Type3ArmController.Release();
                    }
                    break;
            }
        }
    }*/

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
                switch (craneManager.GetCType())
                {

                    case 1:
                    case 5:
                    case 9:
                        if (craneManager.GetStatus() == 6)
                        {
                            lifter.DownForceStop();
                            craneManager.IncrimentStatus();
                        }
                        break;
                    case 4:
                    case 12:
                        if (craneManager.GetStatus() == 8)
                        {
                            lifter.DownForceStop();
                            craneManager.IncrimentStatus();
                        }
                        break;
                    case 6:
                        if (craneManager.GetStatus() == 4)
                        {
                            lifter.DownForceStop();
                            craneManager.IncrimentStatus();
                        }
                        break;
                }
            }));
        }
    }

    public void SetManager(CraneManager c)
    {
        craneManager = c;
    }

    public void SetArmController(int num) // Type3ArmControllerから呼ばれる
    {
        if (num == 3 || num == 7 || num == 10)
            _Type3ArmController = transform.parent.parent.gameObject.GetComponent<Type3ArmController>();
    }
    public void SetLifter(BaseLifter r)
    {
        lifter = r;
    }
}
