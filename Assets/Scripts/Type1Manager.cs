using System;
using UnityEngine;
using UnityEngine.UI;

public class Type1Manager : MonoBehaviour
{
    public CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    int craneStatus = -1; //-1:初期化動作，0:待機状態
    //double catchArmpower; //掴むときのアームパワー(%，未確率時)
    //double upArmpower; //上昇時のアームパワー(%，未確率時)
    //double backArmpower; //獲得口移動時のアームパワー(%，未確率時)
    double catchArmpowersuccess; //同確率時
    double upArmpowersuccess; //同確率時
    double backArmpowersuccess; //同確率時
    double armApertures; //開口率
    float catchTime; //キャッチに要する時間
    bool resetFlag = false; //投入金額リセットは1プレイにつき1度のみ実行

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        creditSystem.SetCreditSound(0);
    }

    void Update()
    {
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            //クレーン位置初期化動作;
            //コイン投入無効化;
            BGMPlayer.StopBGM(0);
        }

        if (craneStatus == 0)
        {
            BGMPlayer.PlayBGM(0);
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
        }

        if (craneStatus == 1)
        {
            //コイン投入有効化;
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
            SEPlayer.PlaySE(1, 2); //右移動効果音ループ再生;
        }

        if (craneStatus == 3)
        {
            SEPlayer.StopSE(1); //右移動効果音ループ再生停止;
            //奥移動ボタン有効化;
        }

        if (craneStatus == 4)
        { //奥移動中
          //クレーン奥移動;
            SEPlayer.PlaySE(1, 2); //奥移動効果音ループ再生;
        }

        if (craneStatus == 5)
        {
            SEPlayer.StopSE(1);//奥移動効果音ループ再生停止;
            //アーム開く音再生;
            //アーム開く;
        }

        if (craneStatus == 6)
        {
            SEPlayer.PlaySE(2, 2); //アーム下降音再生
                                   //アーム下降;
        }

        if (craneStatus == 7)
        {
            SEPlayer.StopSE(2); //アーム下降音再生停止;
            SEPlayer.PlaySE(3, 2); //アーム掴む音再生;
                                   //アーム掴む;
        }

        if (craneStatus == 8)
        {
            SEPlayer.StopSE(3);
            SEPlayer.PlaySE(4, 2); //アーム上昇音再生;
                                   //アーム上昇;
        }

        if (craneStatus == 9)
        {
            SEPlayer.StopSE(4);
            //アーム上昇停止音再生;
            //アーム上昇停止;
        }

        if (craneStatus == 10)
        {
            //アーム獲得口ポジション移動音再生;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            //アーム開く音再生;
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