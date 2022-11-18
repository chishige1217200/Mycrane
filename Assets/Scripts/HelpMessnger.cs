using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMessnger : MonoBehaviour
{
    [SerializeField] RayCaster rc; // マシンの選択状況を参照
    [SerializeField] GameObject message;
    public bool useUI = false;

    // Start is called before the first frame update
    void Start()
    {
        if (rc == null)
            Debug.LogWarning("RayCasterがセットされていません");
        if (message == null)
            Debug.LogWarning("messageオブジェクトがセットされていません");
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
