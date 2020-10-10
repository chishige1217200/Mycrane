using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditSystem : MonoBehaviour
{
    public static int credit = 0; //残クレジット
    private static int nowpaid = 0; //投入金額（開始時リセット）
    public static int nowpaidSum = 0; //投入金額合計
    private int[,] rateSet = new int[2, 2];  //100円1PLAY，500円6PLAYなどのプリセット?

    private Type2Manager craneGameManager; //クレーンゲーム親スクリプト

    //For test-----------------------------------------

    public Text nowPaid;
    public Text Credit;

    //-------------------------------------------------
    void Start()
    {
        craneGameManager = GameObject.Find("CraneGame").GetComponent<Type2Manager>();

        rateSet[0, 0] = 200;
        rateSet[0, 1] = 1;
        rateSet[1, 0] = 500;
        rateSet[1, 1] = 3;

        if ((float)rateSet[0, 0] / rateSet[0, 1] < (float)rateSet[1, 0] / rateSet[1, 1])
        {
            Debug.Log("rateSet value error."); //高額のレートになるとコストが多くなる設定エラーのとき
        }
    }

    void Update()
    {
            nowPaid.text = nowpaid.ToString();
            Credit.text = credit.ToString();
    }

    public void GetPayment(int cost)
    {
        nowpaid += cost;
        nowpaidSum += cost;
        if (nowpaid % rateSet[1, 0] == 0)
        {
            credit = credit + (nowpaid / rateSet[1, 0] * rateSet[1, 1] - nowpaid / rateSet[0, 0] * rateSet[0, 1]);
        }
        else if (nowpaid % rateSet[0, 0] == 0)
        {
            credit = credit + rateSet[0, 1];
        }

        if(credit > 0) Type2Manager.craneStatus = 1;
    }

    public static void ResetNowPayment()
    {
        nowpaid = 0;
    }
}