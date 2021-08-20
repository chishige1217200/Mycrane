using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmNail : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type4Manager _Type4Manager;
    Type5Manager _Type5Manager;
    [SerializeField] int playerNumber = 1;
    RopeManager ropeManager;
    int craneType = -1;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            switch (craneType)
            {
                case 1:
                    if (_Type1Manager.craneStatus == 6)
                    {
                        Debug.Log("床");
                        ropeManager.ArmUnitDownForceStop();
                        _Type1Manager.craneStatus = 7;
                    }
                    break;
                case 4:
                    if (_Type4Manager.craneStatus == 8)
                    {
                        Debug.Log("床");
                        ropeManager.ArmUnitDownForceStop();
                        _Type4Manager.craneStatus = 9;
                    }
                    break;
                case 5:
                    if (_Type5Manager.craneStatus == 6)
                    {
                        Debug.Log("床");
                        ropeManager.ArmUnitDownForceStop();
                        _Type5Manager.craneStatus = 7;
                    }
                    break;
            }
        }
    }

    public void GetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 1) _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2) _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 4) _Type4Manager = transform.root.gameObject.GetComponent<Type4Selecter>().GetManager(playerNumber);
        if (craneType == 5) _Type5Manager = transform.root.gameObject.GetComponent<Type5Selecter>().GetManager(playerNumber);
    }

    public void GetRopeManager(RopeManager r)
    {
        ropeManager = r;
    }
}
