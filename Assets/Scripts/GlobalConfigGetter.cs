using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalConfigGetter : MonoBehaviour
{
    private Toggle t;
    void Start()
    {
        t = GetComponent<Toggle>();

        if (PlayerPrefs.HasKey(t.gameObject.name))
        {
            if (PlayerPrefs.GetInt(t.gameObject.name) == 1)
                t.isOn = true;
            else
                t.isOn = false;
        }
        else
        {
            t.isOn = true;
        }
    }
}
