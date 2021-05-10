using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPoint : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type4Manager _Type4Manager;
    Type5Manager _Type5Manager;
    [SerializeField] int playerNumber = 1;
    int craneType = -1;
    PrizePanel panel;
    Prize prize;

    void Start()
    {
        GameObject gb = GameObject.Find("GameManager");
        if (gb.TryGetComponent(out panel)) panel = gb.GetComponent<PrizePanel>();
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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "prize")
        {
            Debug.Log("prize");
            if (craneType == 1) _Type1Manager.GetPrize();
            if (craneType == 2) _Type2Manager.GetPrize();
            if (craneType == 3) _Type3Manager.GetPrize();
            if (craneType == 4) _Type4Manager.GetPrize();
            if (collider.gameObject.TryGetComponent(out prize))
            {
                panel.SetPrizeName(prize.prizeName);
                panel.PanelActive(true);
            }
        }
    }
}
