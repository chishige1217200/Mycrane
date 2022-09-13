using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmNail : MonoBehaviour
{
    CraneManager craneManager;
    BaseLifter lifter;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            switch (craneManager.GetCType())
            {
                case 1:
                case 5:
                case 9:
                    if (craneManager.GetStatus() == 6)
                    {
                        Debug.Log("床");
                        lifter.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 4:
                case 12:
                    if (craneManager.GetStatus() == 8)
                    {
                        Debug.Log("床");
                        lifter.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 6:
                    if (craneManager.GetStatus() == 4)
                    {
                        Debug.Log("床");
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

    public void SetLifter(BaseLifter r)
    {
        lifter = r;
    }
}
