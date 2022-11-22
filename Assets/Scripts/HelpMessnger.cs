using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMessnger : MonoBehaviour
{
    [SerializeField] RayCaster rc; // マシンの選択状況を参照
    [SerializeField] GameObject message;
    [SerializeField] GameObject manuals;
    List<Transform> manualPanels = new List<Transform>();
    [SerializeField] AudioSource manualOpenSE;
    [SerializeField] AudioSource manualCloseSE;
    public bool useUI = false;

    // Start is called before the first frame update
    void Start()
    {
        if (rc == null)
            Debug.LogWarning("RayCasterがセットされていません");
        if (message == null)
            Debug.LogWarning("messageオブジェクトがセットされていません");

        Transform machines = manuals.transform.Find("Machines").transform;

        for (int i = 0; i < machines.childCount; i++)
            manualPanels.Add(machines.GetChild(i));
    }

    // Update is called once per frame
    void Update()
    {
        // マシン選択を促すメッセージの処理
        if (useUI)
        {
            if (rc.host != null)
            {
                if (!rc.host.playable)
                {
                    if (!message.activeSelf) message.SetActive(true);
                }
                else
                {
                    if (message.activeSelf) message.SetActive(false);
                }
            }
            else
            {
                if (!message.activeSelf) message.SetActive(true);
            }
        }

        // 操作方法表示処理
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (!manuals.activeSelf)
                ShowManual();
            else if (manuals.activeSelf)
                DisShowManual();
        }

        if (Input.GetMouseButtonDown(1))
            SelectManual();
    }

    public void ShowManual()
    {
        manuals.SetActive(true);
        if (manualOpenSE != null) manualOpenSE.Play();
        SelectManual();
    }

    void SelectManual()
    {
        if (rc.host != null)
        {
            for (int i = 0; i < manualPanels.Count; i++)
                manualPanels[i].gameObject.SetActive(false);

            if (rc.host.manualCode - 1 < manualPanels.Count)
                manualPanels[rc.host.manualCode - 1].gameObject.SetActive(true);
        }
    }

    public void DisShowManual()
    {
        manuals.SetActive(false);
        if (manualCloseSE != null) manualCloseSE.Play();
    }
}
