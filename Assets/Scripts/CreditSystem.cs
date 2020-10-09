using System;
using UnityEngine;

public class CreditSystem : MonoBehaviour {
    public int credit = 0; //残クレジット
    private static int nowpaid = 0; //投入金額（開始時リセット）
    public static int nowpaidSum = 0; //投入金額合計
    private int[][] rateSet = new int[2][2]; //100円1PLAY，500円6PLAYなどのプリセット?
    void Start () {
        rateSet[0][0] = 200;
        rateSet[0][1] = 1;
        rateSet[1][0] = 500;
        rateSet[1][1] = 3;

        if ((float) rateSet[0][0] / rateSet[0][1] < (float) rateSet[1][0] / rateSet[1][1]) {
            Debug.Log ("rateSet value error."); //高額のレートになるとコストが多くなる設定エラーのとき
        }
    }

    void Update () {

    }

    void GetPayment (int cost) {
        nowpaid += cost;
        nowpaidSum += cost;
        if (nowpaid == rateSet[1][0]) {
            credit = rateSet[1][1];
        }
    }

}