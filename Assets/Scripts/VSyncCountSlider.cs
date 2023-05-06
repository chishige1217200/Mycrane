using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VSyncCountSlider : MonoBehaviour
{
    Slider s;
    [SerializeField] Text VSyncText;
    [SerializeField] AudioSource sliderSE;
    bool isFirst = true;
    [SerializeField] string[] qualitymessages = new string[3];
    void Start()
    {
        s = GetComponent<Slider>();
        RefreshSlider();
    }

    public void RefreshSlider()
    {
        s.value = vSyncCount2Slider(QualitySettings.vSyncCount);
        VSyncText.text = qualitymessages[(int)s.value];
    }

    public void ApplyConfiguration()
    {
        if (!isFirst) sliderSE.PlayOneShot(sliderSE.clip);
        int tempframe = Slider2vSyncCount((int)s.value);
        // VSyncの数とスライダーの値が合っていないので修正
        QualitySettings.vSyncCount = tempframe;
        VSyncText.text = qualitymessages[(int)s.value];
        isFirst = false;
    }

    int Slider2vSyncCount(int sval)
    {
        switch (sval)
        {
            case 1:
                return 2;
            case 2:
                return 1;
            default:
                return 0;
        }
    }

    int vSyncCount2Slider(int vsc)
    {
        switch (vsc)
        {
            case 1:
                return 2;
            case 2:
                return 1;
            default:
                return 0;
        }
    }
}
