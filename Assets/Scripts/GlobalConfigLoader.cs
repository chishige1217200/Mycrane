using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigLoader : MonoBehaviour
{
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject manualButton;
    [SerializeField] HelpMessnger hm;
    [SerializeField] FirstPersonAIO faio;
    void Start()
    {
        if (PlayerPrefs.GetInt("useUI") == 1)
        {
            CraneManager.useUI = true;
            CraneManagerV2.useUI = true;
        }
        else
        {
            CraneManager.useUI = false;
            CraneManagerV2.useUI = false;
            backButton.SetActive(false);
            manualButton.SetActive(false);
        }

        if (faio == null)
            Debug.LogWarning("FirstPersonAIOがセットされていません");
        else
        {
            if (PlayerPrefs.GetInt("useCentralIcon") == 1)
                faio.autoCrosshair = true;
            else
                faio.autoCrosshair = false;
        }

        if (PlayerPrefs.GetInt("doAutoPlay") == 1)
            AutoControllerFlag.doAutoPlay = true;
        else
            AutoControllerFlag.doAutoPlay = false;

        if (hm == null)
            Debug.LogWarning("HelpMessengerがセットされていません");
        else
        {
            if (PlayerPrefs.GetInt("useUI") == 1)
            {
                hm.useUI = true;
            }
        }
    }
}
