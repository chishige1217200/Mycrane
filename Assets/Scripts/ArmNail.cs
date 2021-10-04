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
    Type6Manager _Type6Manager;
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
                    if (_Type1Manager.GetStatus() == 6)
                    {
                        Debug.Log("床");
                        ropeManager.DownForceStop();
                        _Type1Manager.IncrimentStatus();
                    }
                    break;
                case 4:
                    if (_Type4Manager.GetStatus() == 8)
                    {
                        Debug.Log("床");
                        ropeManager.DownForceStop();
                        _Type4Manager.IncrimentStatus();
                    }
                    break;
                case 5:
                    if (_Type5Manager.GetStatus() == 6)
                    {
                        Debug.Log("床");
                        ropeManager.DownForceStop();
                        _Type5Manager.IncrimentStatus();
                    }
                    break;
                case 6:
                    if (_Type6Manager.GetStatus() == 4)
                    {
                        Debug.Log("床");
                        ropeManager.DownForceStop();
                        _Type6Manager.IncrimentStatus();
                    }
                    break;
            }
        }
    }

    public void SetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 1) _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2) _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 4) _Type4Manager = transform.root.gameObject.GetComponent<Type4Selecter>().GetManager(playerNumber);
        if (craneType == 5) _Type5Manager = transform.root.gameObject.GetComponent<Type5Selecter>().GetManager(playerNumber);
        if (craneType == 6) _Type6Manager = transform.root.gameObject.GetComponent<Type6Selecter>().GetManager(playerNumber);
    }

    public void SetRopeManager(RopeManager r)
    {
        ropeManager = r;
    }
}
