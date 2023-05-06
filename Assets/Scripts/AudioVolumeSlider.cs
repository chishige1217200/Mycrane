using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : MonoBehaviour
{
    Slider s;
    [SerializeField] Text VolumeText;
    [SerializeField] AudioSource sliderSE;
    bool isFirst = true;
    void Start()
    {
        if (!PlayerPrefs.HasKey("AudioVolume"))
        {
            PlayerPrefs.SetFloat("AudioVolume", 1);
            PlayerPrefs.Save();
        }
        else
            AudioListener.volume = PlayerPrefs.GetFloat("AudioVolume");
        s = GetComponent<Slider>();
        s.value = AudioListener.volume * 100;
        VolumeText.text = (AudioListener.volume * 100).ToString();
    }

    public void ApplyConfiguration()
    {
        AudioListener.volume = s.value / 100f;
        VolumeText.text = (AudioListener.volume * 100).ToString();
        PlayerPrefs.SetFloat("AudioVolume", s.value / 100f);
        PlayerPrefs.Save();
        if (!isFirst) sliderSE.PlayOneShot(sliderSE.clip);
        isFirst = false;
    }
}
