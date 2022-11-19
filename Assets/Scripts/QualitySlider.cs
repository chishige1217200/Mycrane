using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualitySlider : MonoBehaviour
{
    Slider s;
    [SerializeField] Text QualityText;
    [SerializeField] string[] qualitymessages = new string[6];
    void Start()
    {
        s = GetComponent<Slider>();
        s.value = QualitySettings.GetQualityLevel();
        QualityText.text = qualitymessages[((int)s.value)];
    }
    public void ApplyConfiguration()
    {
        QualityText.text = qualitymessages[((int)s.value)];
        if (s.value != QualitySettings.GetQualityLevel())
            QualitySettings.SetQualityLevel(((int)s.value));
    }
}
