using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualitySlider : MonoBehaviour
{
    Slider s;
    [SerializeField] Text QualityText;
    [SerializeField] AudioSource sliderSE;
    bool isFirst = true;
    [SerializeField] string[] qualitymessages = new string[6];
    [SerializeField] VSyncCountSlider vscs;
    void Start()
    {
        s = GetComponent<Slider>();
        s.value = QualitySettings.GetQualityLevel();
        QualityText.text = qualitymessages[(int)s.value];
    }
    public void ApplyConfiguration()
    {
        if (!isFirst) sliderSE.PlayOneShot(sliderSE.clip);
        QualitySettings.SetQualityLevel(((int)s.value));
        QualityText.text = qualitymessages[(int)s.value];
        vscs.RefreshSlider();
        isFirst = false;
    }
}
