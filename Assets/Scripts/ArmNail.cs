using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmNail : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    [SerializeField] int playerNumber = 1;
    RopeManager ropeManager;
    int craneType = -1;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "DownLimit")
        {
            Debug.Log("下降制限");
            switch (craneType)
            {
                case 1:
                    if (_Type1Manager.craneStatus == 6)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        _Type1Manager.craneStatus = 7;
                    }
                    break;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Debug.Log("床に接触");
            switch (craneType)
            {
                case 1:
                    if (_Type1Manager.craneStatus == 6)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        _Type1Manager.craneStatus = 7;
                    }
                    break;
            }
        }
    }

    public void GetManager(int num)
    {
        craneType = num;
        if (craneType == 1)
            _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2)
            _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3)
            _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
    }

    public void GetRopeManager(RopeManager r)
    {
        ropeManager = r;
    }
}
