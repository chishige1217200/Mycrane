using System;
using UnityEngine;
using UnityEngine.UI;

public class Type3Manager : MonoBehaviour
{
    public CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    int craneStatus = -1; //-1:初期化動作，0:待機状態
    double catchArmpower; //掴むときのアームパワー(%，未確率時)
    double upArmpower; //上昇時のアームパワー(%，未確率時)
    double backArmpower; //獲得口移動時のアームパワー(%，未確率時)
    double catchArmpowersuccess; //同確率時
    double upArmpowersuccess; //同確率時
    double backArmpowersuccess; //同確率時
    int soundType = 3; //0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    bool resetFlag = false; //投入金額リセットは1プレイにつき1度のみ実行

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(13);
        if (soundType == 3) creditSystem.SetCreditSound(-1);
    }

    void Update()
    {
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            BGMPlayer.StopBGM(soundType);
            //コイン投入無効化;
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            switch (soundType)
            {
                case 0:
                    BGMPlayer.PlayBGM(0);
                    break;
                case 1:
                    BGMPlayer.PlayBGM(1);
                    break;
                case 2:
                    BGMPlayer.PlayBGM(2);
                    break;
                case 3:
                    BGMPlayer.StopBGM(4);
                    BGMPlayer.PlayBGM(3);
                    break;
            }
        }


        if (craneStatus == 1)
        {
            //コイン投入有効化;
            BGMPlayer.StopBGM(soundType);
            switch (soundType)
            {
                case 0:
                    SEPlayer.PlaySE(1, 2147483647);
                    break;
                case 1:
                    SEPlayer.PlaySE(7, 2147483647);
                    break;
                case 3:
                    BGMPlayer.PlayBGM(4);
                    break;
            }
            //右移動ボタン有効化;
        }

        if (craneStatus == 2)
        { //右移動中
          //コイン投入無効化;
            if(resetFlag == false)
            {
               resetFlag = true;
               creditSystem.ResetNowPayment();
            }
          //クレーン右移動;
            switch (soundType)
            {
                case 0:
                    SEPlayer.PlaySE(1, 2147483647);
                    break;
                case 1:
                    SEPlayer.StopSE(7);
                    SEPlayer.PlaySE(8, 2147483647);
                    break;
                case 2:
                    SEPlayer.PlaySE(14, 2147483647);
                    break;
                case 3:
                    SEPlayer.PlaySE(18, 2147483647);
                    break;
            }
            //右移動効果音ループ再生;
        }

        if (craneStatus == 3)
        {
            switch (soundType)
            {
                case 2:
                    SEPlayer.StopSE(14);
                    break;
                case 3:
                    SEPlayer.StopSE(18);
                    break;
            }
            //右移動効果音ループ再生停止;
            //奥移動ボタン有効化;
        }

        if (craneStatus == 4)
        { //奥移動中
          //クレーン奥移動;
            switch (soundType)
            {
                case 0:
                    SEPlayer.StopSE(1);
                    SEPlayer.PlaySE(2, 2147483647);
                    break;
                case 1:
                    SEPlayer.StopSE(8);
                    SEPlayer.PlaySE(9, 2147483647);
                    break;
                case 2:
                    SEPlayer.PlaySE(14, 2147483647);
                    break;
                case 3:
                    SEPlayer.PlaySE(19, 2147483647);
                    break;
            }
            //奥移動効果音ループ再生;
        }

        if (craneStatus == 5)
        {
            switch (soundType)
            {
                case 0:
                    SEPlayer.StopSE(2);
                    SEPlayer.PlaySE(3, 2147483647);
                    break;
                case 1:
                    SEPlayer.StopSE(9);
                    SEPlayer.PlaySE(10, 2147483647);
                    break;
                case 2:
                    SEPlayer.StopSE(14);
                    break;
                case 3:
                    SEPlayer.StopSE(19);
                    SEPlayer.PlaySE(20, 1);
                    break;
            }
            //奥移動効果音ループ再生停止;
            //アーム開く音再生;
            //アーム開く;
        }

        if (craneStatus == 6)
        {
            switch (soundType)
            {
                case 2:
                    SEPlayer.PlaySE(15, 2147483647);
                    break;
                case 3:
                    SEPlayer.PlaySE(21, 2147483647);
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
                    SEPlayer.StopSE(3);
                    SEPlayer.PlaySE(4, 2147483647);
                    break;
                case 1:
                    SEPlayer.StopSE(10);
                    SEPlayer.PlaySE(11, 2147483647);
                    break;
                case 3:
                    SEPlayer.StopSE(21);
                    break;
            }
            //アーム下降音再生停止;
            //アーム掴む音再生;
            //アーム掴む;
        }

        if (craneStatus == 8)
        {
            switch (soundType)
            {
                case 3:
                    SEPlayer.PlaySE(22, 2147483647);
                    break;
            }
            //アーム上昇音再生;
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            switch (soundType)
            {
                case 3:
                    SEPlayer.StopSE(22);
                    break;
            }
            //アーム上昇停止音再生;
            //アーム上昇停止;
        }

        if (craneStatus == 10)
        {
            switch (soundType)
            {
                case 0:
                    SEPlayer.StopSE(4);
                    SEPlayer.PlaySE(1, 2147483647);
                    break;
                case 2:
                    SEPlayer.StopSE(15);
                    SEPlayer.PlaySE(14, 2147483647);
                    break;
                case 3:
                    SEPlayer.PlaySE(23, 2147483647);
                    break;
            }
            //アーム獲得口ポジション移動音再生;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            switch (soundType)
            {
                case 0:
                    SEPlayer.StopSE(1);
                    break;
                case 1:
                    SEPlayer.StopSE(11);
                    break;
                case 2:
                    SEPlayer.StopSE(14);
                    SEPlayer.PlaySE(17, 1);
                    break;
                case 3:
                    SEPlayer.StopSE(23);
                    SEPlayer.PlaySE(24, 1);
                    break;
            }
            //アーム開く音再生;
            //アーム開く;
            //1秒待機;
        }

        if (craneStatus == 12)
        {
            switch (soundType)
            {
                case 3:
                    SEPlayer.PlaySE(25, 1);
                    break;
            }
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