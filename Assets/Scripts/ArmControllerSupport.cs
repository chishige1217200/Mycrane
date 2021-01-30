using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControllerSupport : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type3ArmController _Type3ArmController;
    int craneType = -1;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ReleaseCheck")
        {
            switch (craneType)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    if (!_Type3Manager.probability && _Type3Manager.craneStatus >= 8)
                    {
                        Debug.Log("Released.");
                        _Type3ArmController.Release();
                    }
                    break;
            }
        }
    }

    public void GetManager(int num)
    {
        craneType = num;
        if (craneType == 1)
            _Type1Manager = transform.root.gameObject.GetComponent<Type1Manager>();
        if (craneType == 2)
            _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3)
            _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
    }

    public void GetArmController(int num)
    {
        /*if (num == 1)
            _Type1Manager = transform.parent.gameObject.GetComponent<Type1Manager>();
        if (num == 2)
            _Type2Manager = transform.parent.gameObject.GetComponent<Type2Manager>();*/
        if (num == 3)
            _Type3ArmController = transform.parent.gameObject.GetComponent<Type3ArmController>();
    }
}
