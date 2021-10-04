using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ArmControllerSupport : MonoBehaviour
{
    /*Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type4Manager _Type4Manager;
    Type5Manager _Type5Manager;
    Type6Manager _Type6Manager;
    Type7Manager _Type7Manager;*/
    Type3ArmController _Type3ArmController;
    CraneManager craneManager;
    //[SerializeField] int playerNumber = 1;
    RopeManager ropeManager;
    //int craneType = -1;
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
                        ropeManager.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 4:
                    if (craneManager.GetStatus() == 8)
                    {
                        Debug.Log("下降制限に接触");
                        ropeManager.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 6:
                    if (craneManager.GetStatus() == 4)
                    {
                        Debug.Log("下降制限に接触");
                        ropeManager.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
            }
        }
        if (collider.tag == "prize")
        {
            Debug.Log("prize inTrigger");
            prizeCount++;
            switch (craneManager.GetCType())
            {
                case 3:
                    if (craneManager.GetStatus() == 6)
                    {
                        await Task.Delay(700);
                        if (craneManager.GetStatus() == 6)
                        {
                            ropeManager.DownForceStop();
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
                    if (craneManager.GetStatus() == 6)
                    {
                        await Task.Delay(1000);
                        if (craneManager.GetStatus() == 6)
                        {
                            ropeManager.DownForceStop();
                            craneManager.IncrimentStatus();
                        }
                    }
                    break;
            }
            isShieldcollis = true;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        /*if (collider.tag == "prize")
            prizeFlag = true;*/
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
                    if (craneManager.GetStatus() == 6)
                    {
                        ropeManager.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 4:
                    if (craneManager.GetStatus() == 8)
                    {
                        ropeManager.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
                case 6:
                    if (craneManager.GetStatus() == 4)
                    {
                        ropeManager.DownForceStop();
                        craneManager.IncrimentStatus();
                    }
                    break;
            }
        }
    }

    /*public void SetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == 1) _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2) _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 4) _Type4Manager = transform.root.gameObject.GetComponent<Type4Selecter>().GetManager(playerNumber);
        if (craneType == 5) _Type5Manager = transform.root.gameObject.GetComponent<Type5Selecter>().GetManager(playerNumber);
        if (craneType == 6) _Type6Manager = transform.root.gameObject.GetComponent<Type6Selecter>().GetManager(playerNumber);
        if (craneType == 7) _Type7Manager = transform.root.gameObject.GetComponent<Type7Manager>();
    }*/

    public void SetManager(CraneManager c)
    {
        craneManager = c;
    }

    public void SetArmController(int num)
    {
        /*if (num == 1)
            _Type1Manager = transform.parent.gameObject.GetComponent<Type1Manager>();
        if (num == 2)
            _Type2Manager = transform.parent.gameObject.GetComponent<Type2Manager>();*/
        if (num == 3 || num == 7)
            _Type3ArmController = transform.parent.parent.gameObject.GetComponent<Type3ArmController>();
    }
    public void SetRopeManager(RopeManager r)
    {
        ropeManager = r;
    }
}
