using System;
using UnityEngine;
using UnityEngine.UI;

public class Type2Manager : MonoBehaviour
{
    public CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    int craneStatus = -1; //-1:初期化動作，0:待機状態
    double catchArmpower; //掴むときのアームパワー(%，未確率時)
    double upArmpower; //上昇時のアームパワー(%，未確率時)
    double backArmpower; //獲得口移動時のアームパワー(%，未確率時)
    double catchArmpowersuccess; //同確率時
    double upArmpowersuccess; //同確率時
    double backArmpowersuccess; //同確率時
    int operationType = 1; //0:ボタン式，1:レバー式
    int limitTime = 60; //レバー式の場合
    int soundType = 0; //DECACRE:0, DECACRE Alpha:1
    bool resetFlag = false; //投入金額リセットは1プレイにつき1度のみ実行

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
    }

    void Update()
    {
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            BGMPlayer.StopBGM(2 * soundType);
            //クレーン位置初期化動作; DECACRE・CARINOタイプは不要
            //コイン投入無効化;
        }

        if (craneStatus == 0)
        {
            BGMPlayer.StopBGM(1 + 2 * soundType);
            BGMPlayer.PlayBGM(2 * soundType);
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
        }

        if (operationType == 0)
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                BGMPlayer.StopBGM(2 * soundType);
                BGMPlayer.PlayBGM(1 + 2 * soundType);
                //右移動ボタン有効化;
            }

            if (craneStatus == 2)
            { //右移動中
                //コイン投入無効化;
                if (resetFlag == false)
                {
                    resetFlag = true;
                    creditSystem.ResetNowPayment();
                }
                //クレーン右移動;
                //右移動効果音ループ再生;
            }

            if (craneStatus == 3)
            {
                //右移動効果音ループ再生停止;
                //奥移動ボタン有効化;
            }

            if (craneStatus == 4)
            { //奥移動中
                //クレーン奥移動;
                //奥移動効果音ループ再生;
            }

            if (craneStatus == 5)
            {
                //奥移動効果音ループ再生停止;
                //アーム開く音再生;
                //アーム開く;
            }
        }

        if (operationType == 1)
        {
            if (craneStatus == 1)
            {
                BGMPlayer.StopBGM(2 * soundType);
                BGMPlayer.PlayBGM(1 + 2 * soundType);
                limitTime = 60;
                //レバー操作有効化;
                //降下ボタン有効化;
            }

            if (craneStatus == 6)
            {
                if (resetFlag == false)
                {
                    resetFlag = true;
                    creditSystem.ResetNowPayment();
                }
                switch (soundType)
                {
                    case 0:
                        SEPlayer.PlaySE(1, 2147483647);
                        break;
                    case 1:
                        SEPlayer.PlaySE(8, 2147483647);
                        break;
                }
                //アーム下降音再生
                //アーム下降;
            }

            if (craneStatus == 7)
            {
                switch (soundType)
                {
                    case 0:
                        SEPlayer.StopSE(1); //アーム下降音再生停止;
                        SEPlayer.PlaySE(2, 1); //アーム掴む音再生;
                        break;
                    case 1:
                        SEPlayer.StopSE(8);
                        break;
                }
                //アーム掴む;
            }

            if (craneStatus == 8)
            {
                if (soundType == 1) SEPlayer.PlaySE(9, 2147483647);
                //アーム上昇音再生;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {
                switch (soundType)
                {
                    case 0:
                        SEPlayer.StopSE(2);
                        SEPlayer.PlaySE(3, 1); //アーム上昇停止音再生;
                        break;
                    case 1:
                        SEPlayer.StopSE(9);
                        break;
                }

                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
                //アーム獲得口ポジション移動音再生;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                if (soundType == 0) SEPlayer.PlaySE(4, 1); //アーム開く音再生;
                //アーム開く;
                //1秒待機;
            }

            if (craneStatus == 12)
            {
                //アーム閉じる音再生;
                //アーム閉じる;
                //1秒待機;
                resetFlag = false;
                if (creditSystem.creditDisplayed > 0)
                    craneStatus = 1;
                else
                    craneStatus = 0;
            }
        }
    }

    public void Testadder()
    {
        Debug.Log("Clicked.");
        craneStatus++;
    }

    public void TestSubber()
    {
        Debug.Log("Clicked.");
        craneStatus--;
    }
}