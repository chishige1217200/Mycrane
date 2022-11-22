using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] AudioSource closeSE; // 異なるゲームオブジェクトにアタッチしておくこと
    // Start is called before the first frame update
    void Start()
    {
        if (panel == null) Debug.LogWarning("パネル要素が指定されていません");
    }

    // Update is called once per frame
    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.B)) DisShow();
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void DisShow()
    {
        if (closeSE != null) closeSE.PlayOneShot(closeSE.clip);
        panel.SetActive(false);
    }
}
