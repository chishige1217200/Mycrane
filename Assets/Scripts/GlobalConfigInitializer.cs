using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("useUI"))
            PlayerPrefs.SetInt("useUI", 1);
        if (!PlayerPrefs.HasKey("useCentralIcon"))
            PlayerPrefs.SetInt("useCentralIcon", 1);
        if (!PlayerPrefs.HasKey("doAutoPlay"))
            PlayerPrefs.SetInt("doAutoPlay", 1);

        PlayerPrefs.Save();
    }
}