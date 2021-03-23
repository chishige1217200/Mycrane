using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizePanel : MonoBehaviour
{
    GameObject canvas;
    Text prizeText;
    Text dateText;
    void Start()
    {
        canvas = this.transform.Find("Canvas").gameObject;
        prizeText = this.transform.Find("Canvas").Find("Panel").Find("PrizeText").GetComponent<Text>();
        dateText = this.transform.Find("Canvas").Find("Panel").Find("DateText").GetComponent<Text>();
    }

    public void SetPrizeName(string name)
    {
        prizeText.text = "「" + name + "」\nを獲得しました！";
        dateText.text = "獲得日時:" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
    }

    public void PanelActive(bool t)
    {
        if (!canvas.activeSelf && t) canvas.SetActive(true);
        if (canvas.activeSelf && !t) canvas.SetActive(false);
    }
}
