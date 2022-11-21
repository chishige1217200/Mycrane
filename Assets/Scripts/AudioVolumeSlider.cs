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
        s = GetComponent<Slider>();
        s.value = AudioListener.volume * 100;
        VolumeText.text = (AudioListener.volume * 100).ToString();
    }

    public void ApplyConfiguration()
    {
        if (s.value != AudioListener.volume)
            AudioListener.volume = s.value / 100f;
        VolumeText.text = (AudioListener.volume * 100).ToString();
        if (!isFirst) sliderSE.PlayOneShot(sliderSE.clip);
        isFirst = false;
    }
}
