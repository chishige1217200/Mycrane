using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalConfigSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("useUI"))
            PlayerPrefs.SetInt("useUI", 1);
        if (!PlayerPrefs.HasKey("useCentralIcon"))
            PlayerPrefs.SetInt("useCentralIcon", 1);

        PlayerPrefs.Save();
    }

    public void SetTogglePrefs(Toggle t)
    {
        if (t.isOn)
            PlayerPrefs.SetInt(t.gameObject.name, 1);
        else
            PlayerPrefs.SetInt(t.gameObject.name, 0);
        PlayerPrefs.Save();
    }

}
