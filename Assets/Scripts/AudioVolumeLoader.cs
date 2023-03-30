using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeLoader : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("AudioVolume"))
            PlayerPrefs.SetFloat("AudioVolume", 1);
        else
            AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume");
    }
}
