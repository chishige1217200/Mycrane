using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMessenger2D : MonoBehaviour
{
    [SerializeField] GameObject manuals;
    [SerializeField] AudioSource manualOpenSE;
    [SerializeField] AudioSource manualCloseSE;

    // Update is called once per frame
    void Update()
    {
        // 操作方法表示処理
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (!manuals.activeSelf)
                ShowManual();
            else if (manuals.activeSelf)
                DisShowManual();
        }
    }

        public void ShowManual()
    {
        manuals.SetActive(true);
        if (manualOpenSE != null) manualOpenSE.Play();
    }

    public void DisShowManual()
    {
        manuals.SetActive(false);
        if (manualCloseSE != null) manualCloseSE.Play();
    }
}
