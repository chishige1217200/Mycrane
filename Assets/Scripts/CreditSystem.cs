using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditSystem : MonoBehaviour
{
    public int creditNew = 0; //新規クレジット
    public int creditAll = 0; //全クレジット
    public int creditDisplayed = 0; //筐体に表示されるクレジット
    private int nowpaid = 0; //投入金額（開始時リセット）
    public int nowpaidSum = 0; //投入金額合計
    private int[,] rateSet = new int[2, 2];  //100円1PLAY，500円6PLAYなどのプリセット?

    //For test-----------------------------------------

    public Text nowPaid;
    public Text Credit;

    //-------------------------------------------------
    void Start()
    {
        rateSet[0, 0] = 200;
        rateSet[0, 1] = 3;
        rateSet[1, 0] = 500;
        rateSet[1, 1] = 1;

        if ((float)rateSet[0, 0] / rateSet[0, 1] < (float)rateSet[1, 0] / rateSet[1, 1])
        {
            Debug.Log("rateSet value error."); //高額のレートになるとコストが多くなる設定エラーのとき
        }
    }

    void Update()
    {
        nowPaid.text = nowpaid.ToString();
        Credit.text = creditDisplayed.ToString();
    }

    public void GetPayment(int cost)
    {
        nowpaid += cost;
        nowpaidSum += cost;
        if (nowpaid % rateSet[1, 0] == 0 && creditNew < nowpaid / rateSet[1, 0] * rateSet[1, 1])
        {
            creditNew = nowpaid / rateSet[1, 0] * rateSet[1, 1];
        }
        else if (nowpaid % rateSet[0, 0] == 0 && creditNew < nowpaid / rateSet[0, 0] * rateSet[0, 1])
        {
            creditNew = nowpaid / rateSet[0, 0] * rateSet[0, 1];
        }
        else
        {
            Debug.Log("あなたは損または保留をしています");
        }
        creditDisplayed = creditAll + creditNew;
        SEPlayer.PlaySE(0, 1);
    }

    public void ResetNowPayment()
    {
        if (nowpaid % rateSet[1, 0] == 0 || nowpaid % rateSet[0, 0] == 0) nowpaid = 0;
        else nowpaid = nowpaid % rateSet[0, 0];
        creditAll = creditAll + creditNew;
        creditNew = 0;
        creditDisplayed = creditAll + creditNew;
    }

    public void ServiceButton()
    {
        creditAll++;
        creditDisplayed = creditAll + creditNew;
    }
}