using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetPointV3 : MonoBehaviour
{
    [SerializeField] bool autoDestroy = true; // 自動的に景品を消去する
    private CraneManagerV3 craneManager;
    private PrizePanel panel;
    private Prize prize;
    private MissionMode mission;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    void Start()
    {
        GameObject gb = GameObject.Find("GameManager");
        if (gb.TryGetComponent(out panel)) panel = gb.GetComponent<PrizePanel>();
        if (gb.TryGetComponent(out mission)) mission = gb.GetComponent<MissionMode>();

        if (panel != null && panel.mode3d) autoDestroy = false;
    }

    public void SetManager(CraneManagerV3 c)
    {
        craneManager = c;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "prize")
        {
            craneManager.GetPrize();
            if (collider.gameObject.TryGetComponent(out prize) && autoDestroy)
            {
                if (mission == null || !mission.enabled)
                {
                    panel.SetPrizeName(prize.prizeName);
                    panel.PanelActive(true);
                }
                else
                {
                    mission.GameClear();
                }
                StartCoroutine(DelayCoroutine(5000, () =>
                {
                    if (prize.destroyObject != null) Destroy(prize.destroyObject);
                    else Destroy(collider.gameObject);
                }));
            }
        }
    }
}
