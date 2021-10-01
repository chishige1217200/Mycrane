using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GetPoint : MonoBehaviour
{
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    Type4Manager _Type4Manager;
    Type5Manager _Type5Manager;
    Type6Manager _Type6Manager;
    Type7Manager _Type7Manager;
    CraneManager craneManager;
    [SerializeField] int playerNumber = 1;
    int craneType = -1;
    [SerializeField] bool autoDestroy = false; // 自動的に景品を消去する
    PrizePanel panel;
    Prize prize;

    void Start()
    {
        GameObject gb = GameObject.Find("GameManager");
        if (gb.TryGetComponent(out panel)) panel = gb.GetComponent<PrizePanel>();
    }

    public void SetManager(int num) // 筐体のマネージャー情報取得
    {
        craneType = num;
        if (craneType == -1) craneManager = transform.parent.parent.gameObject.GetComponent<CraneManager>();
        if (craneType == 1) _Type1Manager = transform.root.gameObject.GetComponent<Type1Selecter>().GetManager(playerNumber);
        if (craneType == 2) _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3) _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
        if (craneType == 4) _Type4Manager = transform.root.gameObject.GetComponent<Type4Selecter>().GetManager(playerNumber);
        if (craneType == 5) _Type5Manager = transform.root.gameObject.GetComponent<Type5Selecter>().GetManager(playerNumber);
        if (craneType == 6) _Type6Manager = transform.root.gameObject.GetComponent<Type6Selecter>().GetManager(playerNumber);
        if (craneType == 7) _Type7Manager = transform.root.gameObject.GetComponent<Type7Manager>();
    }

    async void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "prize")
        {
            //Debug.Log("prize");
            if (craneType == -1) craneManager.GetPrize();
            if (craneType == 1) _Type1Manager.GetPrize();
            if (craneType == 2) _Type2Manager.GetPrize();
            if (craneType == 3) _Type3Manager.GetPrize();
            if (craneType == 4) _Type4Manager.GetPrize();
            if (craneType == 5) _Type5Manager.GetPrize();
            if (craneType == 6) _Type6Manager.GetPrize();
            if (craneType == 7) _Type7Manager.GetPrize();
            if (collider.gameObject.TryGetComponent(out prize) && autoDestroy)
            {
                panel.SetPrizeName(prize.prizeName);
                panel.PanelActive(true);
                await Task.Delay(5000);
                if (prize.destroyObject != null) Destroy(prize.destroyObject);
                else Destroy(collider.gameObject);
            }
        }
    }
}
