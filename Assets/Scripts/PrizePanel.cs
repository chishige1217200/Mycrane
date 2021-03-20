using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizePanel : MonoBehaviour
{
    GameObject canvas;
    Text prizeText;
    void Start()
    {
        canvas = this.transform.Find("Canvas").gameObject;
        prizeText = this.transform.Find("Canvas").Find("Panel").Find("PrizeText").GetComponent<Text>();
    }

    public void SetPrizeName(string name)
    {
        prizeText.text = "「" + name + "」\nを獲得しました！";
    }


    public void PanelActive(bool t)
    {
        if (!canvas.activeSelf && t) canvas.SetActive(true);
        if (canvas.activeSelf && !t) canvas.SetActive(false);
    }
}
