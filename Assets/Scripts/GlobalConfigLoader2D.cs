using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigLoader2D : MonoBehaviour
{
    void Start()
    {
        CraneManager.useUI = true;
        CraneManagerV2.useUI = true;
        AutoControllerFlag.doAutoPlay = true;
    }
}
