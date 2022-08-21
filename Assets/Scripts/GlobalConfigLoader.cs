using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigLoader : MonoBehaviour
{
    [SerializeField] GameObject backButton;
    void Start()
    {
        if (PlayerPrefs.GetInt("useUI") == 1)
            CraneManager.useUI = true;
        else
        {
            CraneManager.useUI = false;
            backButton.SetActive(false);
        }

        if (PlayerPrefs.GetInt("useCentralIcon") == 0)
            GameObject.Find("FirstPerson-AIO").GetComponent<FirstPersonAIO>().autoCrosshair = false;
    }
}
