using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalConfigSetter : MonoBehaviour
{
    [SerializeField] AudioSource checkSE;
    bool useSE = false;

    void Start()
    {
        Invoke("EnableSE", 0.1f);
    }

    void EnableSE()
    {
        useSE = true;
    }

    public void SetTogglePrefs(Toggle t)
    {
        if (useSE)
            checkSE.PlayOneShot(checkSE.clip);
        if (t.isOn)
            PlayerPrefs.SetInt(t.gameObject.name, 1);
        else
            PlayerPrefs.SetInt(t.gameObject.name, 0);
        PlayerPrefs.Save();
    }
}
