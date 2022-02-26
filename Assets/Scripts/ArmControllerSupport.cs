using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ArmControllerSupport : MonoBehaviour
{
    Type3ArmController _Type3ArmController;
    CraneManager craneManager;
    BaseLifter lifter;
    public int pushTime = 0;
    public int prizeCount = 0; // プライズがアームにいくつ検知されているか
    public bool isShieldcollis = false; // アームがShieldに衝突しているかどうか

    async void OnTriggerEnter(Collider collider)
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
                case 5:
                    if (craneManager.GetStatus() == 6)
                    {
                        Debug.Log("下降制限に接触");
                        lifter.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 4:
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
                            await Task.Delay(700);
                        if (craneManager.GetCType() == 10)
                            await Task.Delay(500);
                        if (craneManager.GetStatus() == 6)
                        {
                            lifter.DownForceStop();
                            craneManager.IncrimentStatus();
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
                        if (craneManager.GetCType() == 3) await Task.Delay(1000);
                        else if (craneManager.GetCType() == 10) await Task.Delay(300);
                        if (craneManager.GetStatus() == 6)
                        {
                            lifter.DownForceStop();
                            craneManager.IncrimentStatus();
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
    async void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "prize")
        {
            Debug.Log("景品に接触");
            await Task.Delay(pushTime); //押し込みパワーの調整
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
