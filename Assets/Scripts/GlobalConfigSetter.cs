using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalConfigSetter : MonoBehaviour
{
    public void SetTogglePrefs(Toggle t)
    {
        if (t.isOn)
            PlayerPrefs.SetInt(t.gameObject.name, 1);
        else
            PlayerPrefs.SetInt(t.gameObject.name, 0);
        PlayerPrefs.Save();
    }
}
