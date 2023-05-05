using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightSlider : MonoBehaviour
{
    Slider s;
    [SerializeField] Text HeightText;
    [SerializeField] AudioSource sliderSE;
    bool isFirst = true;
    void Start()
    {
        if (!PlayerPrefs.HasKey("FirstPersonAIOHeight"))
        {
            PlayerPrefs.SetFloat("FirstPersonAIOHeight", 1.2f);
            PlayerPrefs.SetFloat("AudioVolume", 1);
        }
        s = GetComponent<Slider>();
        s.value = PlayerPrefs.GetFloat("FirstPersonAIOHeight") * 10f;
        HeightText.text = (s.value / 10f).ToString("F1");
    }

    public void ApplyConfiguration()
    {
        HeightText.text = (s.value / 10f).ToString("F1");
        PlayerPrefs.SetFloat("FirstPersonAIOHeight", s.value / 10f);
        PlayerPrefs.Save();
        if (!isFirst) sliderSE.PlayOneShot(sliderSE.clip);
        isFirst = false;
    }
}
