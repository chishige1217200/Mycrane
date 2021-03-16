using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPoint : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    [SerializeField] int playerNumber = 1;
    int craneType = -1;

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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "prize")
        {
            Debug.Log("prize");
            if (craneType == 1) _Type1Manager.GetPrize();
            if (craneType == 2) _Type2Manager.GetPrize();
            if (craneType == 3) _Type3Manager.GetPrize();
        }
    }
}
