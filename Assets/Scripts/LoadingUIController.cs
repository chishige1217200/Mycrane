using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    [SerializeField] GameObject root;
    CanvasGroup panel;
    Slider slider;

    void Start()
    {
        panel = root.transform.Find("Panel").GetComponent<CanvasGroup>();
        slider = panel.transform.Find("Slider").GetComponent<Slider>();
    }

    public void Show()
    {
        //panel.gameObject.SetActive(true);
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        root.GetComponent<GraphicRaycaster>().enabled = true;
        for (int i = 1; i <= 50; i++)
        {
            panel.alpha = (float)i / 50;
            yield return new WaitForSeconds(1.0f / 50);
        }
    }

    public void SetProgress(float value)
    {
        slider.value = value;
    }
}
