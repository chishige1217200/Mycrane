using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ArmControllerSupport : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type4Manager _Type4Manager;
    Type5Manager _Type5Manager;
    Type7Manager _Type7Manager;
    Type3ArmController _Type3ArmController;
    [SerializeField] int playerNumber = 1;
    RopeManager ropeManager;
    int craneType = -1;
    public int pushTime = 0;
    public bool prizeFlag = false;
    public bool isShieldcollis = false; // アームがShieldに衝突しているかどうか

    async void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ReleaseCheck")
        {
            switch (craneType)
            {
                case 3:
                    if (!_Type3Manager.probability && _Type3Manager.craneStatus >= 8 && prizeFlag)
                    {
                        Debug.Log("Released.");
                        _Type3ArmController.Release();
                    }
                    break;
                case 7:
                    if (!_Type7Manager.probability && prizeFlag)
                    {
                        Debug.Log("Released.");
                        _Type3ArmController.Release();
                    }
                    break;
            }
        }
        if (collider.tag == "DownLimit")
        {
            switch (craneType)
            {
                case 1:
                    if (_Type1Manager.craneStatus == 6)
                    {
                        Debug.Log("下降制限に接触");
                        ropeManager.ArmUnitDownForceStop();
                        _Type1Manager.craneStatus = 7;
                    }
                    break;
                case 4:
                    if (_Type4Manager.craneStatus == 8)
                    {
                        Debug.Log("下降制限に接触");
                        ropeManager.ArmUnitDownForceStop();
                        _Type4Manager.craneStatus = 9;
                    }
                    break;
            }
        }
        if (collider.tag == "prize")
        {
            Debug.Log("prize inTrigger");
            prizeFlag = true;
            switch (craneType)
            {
                case 3:
                    if (_Type3Manager.craneStatus == 6)
                    {
                        await Task.Delay(700);
                        ropeManager.ArmUnitDownForceStop();
                        _Type3Manager.craneStatus = 7;
                    }
                    break;
            }
        }
        if (collider.tag == "Shield")
        {
            switch (craneType)
            {
                case 3:
                    if (_Type3Manager.craneStatus == 6)
                    {
                        await Task.Delay(1000);
                        ropeManager.ArmUnitDownForceStop();
                        _Type3Manager.craneStatus = 7;
                    }
                    break;
            }
            isShieldcollis = true;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "prize")
            prizeFlag = true;
        if (collider.tag == "ReleaseCheck")
        {
            switch (craneType)
            {
                case 3:
                    if (!_Type3Manager.probability && _Type3Manager.craneStatus >= 8 && prizeFlag)
                    {
                        _Type3ArmController.Release();
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
            prizeFlag = false;
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
            switch (craneType)
            {

                case 1:
                    if (_Type1Manager.craneStatus == 6)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        _Type1Manager.craneStatus = 7;
                    }
                    break;
                case 4:
                    if (_Type4Manager.craneStatus == 8)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        _Type4Manager.craneStatus = 9;
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
        if (craneType == 7) _Type7Manager = transform.root.gameObject.GetComponent<Type7Manager>();
    }

    public void GetArmController(int num)
    {
        /*if (num == 1)
            _Type1Manager = transform.parent.gameObject.GetComponent<Type1Manager>();
        if (num == 2)
            _Type2Manager = transform.parent.gameObject.GetComponent<Type2Manager>();*/
        if (num == 3 || num == 7)
            _Type3ArmController = transform.parent.parent.gameObject.GetComponent<Type3ArmController>();
    }
    public void GetRopeManager(RopeManager r)
    {
        ropeManager = r;
    }
}
