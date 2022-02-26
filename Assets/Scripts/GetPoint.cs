using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GetPoint : MonoBehaviour
{
    CraneManager craneManager;
    [SerializeField] bool autoDestroy = false; // 自動的に景品を消去する
    PrizePanel panel;
    Prize prize;
    MissionMode mission;

    void Start()
    {
        GameObject gb = GameObject.Find("GameManager");
        if (gb.TryGetComponent(out panel)) panel = gb.GetComponent<PrizePanel>();
        if (gb.TryGetComponent(out mission)) mission = gb.GetComponent<MissionMode>();
    }

    public void SetManager(CraneManager c)
    {
        craneManager = c;
    }

    async void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "prize")
        {
            craneManager.GetPrize();
            if (collider.gameObject.TryGetComponent(out prize) && autoDestroy)
            {
                if (mission == null)
                {
                    panel.SetPrizeName(prize.prizeName);
                    panel.PanelActive(true);
                }
                else
                {
                    mission.GameClear();
                }
                await Task.Delay(5000);
                if (prize.destroyObject != null) Destroy(prize.destroyObject);
                else Destroy(collider.gameObject);
            }
        }
    }
}
