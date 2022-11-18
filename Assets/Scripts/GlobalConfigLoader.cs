using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigLoader : MonoBehaviour
{
    [SerializeField] GameObject backButton;
    [SerializeField] HelpMessnger hm;
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
        }

        if (PlayerPrefs.GetInt("useCentralIcon") == 0)
            GameObject.Find("FirstPerson-AIO").GetComponent<FirstPersonAIO>().autoCrosshair = false;

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
