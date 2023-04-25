using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmNailV3 : MonoBehaviour
{
    CraneManagerV3 craneManager;
    BaseLifter lifter;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            switch (craneManager.GetCraneType())
            {
                case 5:
                    if (craneManager.craneStatus == 6)
                    {
                        Debug.Log("床");
                        lifter.DownForceStop();
                        craneManager.craneStatus = 7;
                    }
                    break;
            }
        }
    }

    public void SetManager(CraneManagerV3 c)
    {
        craneManager = c;
    }

    public void SetLifter(BaseLifter r)
    {
        lifter = r;
    }
}
