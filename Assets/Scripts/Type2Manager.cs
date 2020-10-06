using System;
using UnityEngine;
using UnityEngine.UI;

public class Type2Manager : MonoBehaviour
{
    public static int craneStatus = -1; //-1:初期化動作，0:待機状態
    double catchArmpower; //掴むときのアームパワー(%，未確率時)
    double upArmpower; //上昇時のアームパワー(%，未確率時)
    double backArmpower; //獲得口移動時のアームパワー(%，未確率時)
    double catchArmpowersuccess; //同確率時
    double upArmpowersuccess; //同確率時
    double backArmpowersuccess; //同確率時
    int operationType = 1; //0:ボタン式，1:レバー式
    int limitTime = 60; //レバー式の場合
    float craneMovespeed = 1.0f; //クレーンの移動速度設定;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {

    }

    void Update()
    {
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            //クレーン位置初期化動作; DECACRE・CARINOタイプは不要
            //コイン投入無効化;
            craneStatus++;
        }

        if (craneStatus == 0)
        {
            BGMPlayer.StopBGM(1);
            BGMPlayer.PlayBGM(0);
            //コイン投入有効化;
        }

        if (operationType == 0)
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                BGMPlayer.StopBGM(0);
                BGMPlayer.PlayBGM(1);
                //右移動ボタン有効化;
            }

            if (craneStatus == 2)
            { //右移動中
                //コイン投入無効化;
                //nowpaid = 0; //投入金額リセット
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
                BGMPlayer.StopBGM(0);
                BGMPlayer.PlayBGM(1);
                limitTime = 60;
                //レバー操作有効化;
                //降下ボタン有効化;
            }

            if (craneStatus == 6)
            {
                SEPlayer.PlaySE(1, 2); //アーム下降音再生
                //アーム下降;
            }

            if (craneStatus == 7)
            {
                SEPlayer.StopSE(1); //アーム下降音再生停止;
                SEPlayer.PlaySE(2, 1); //アーム掴む音再生;
                //アーム掴む;
            }

            if (craneStatus == 8)
            {
                //アーム上昇音再生;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {
                SEPlayer.StopSE(2);
                SEPlayer.PlaySE(3, 1); //アーム上昇停止音再生;
                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
                //アーム獲得口ポジション移動音再生;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                SEPlayer.PlaySE(4, 1); //アーム開く音再生;
                //アーム開く;
                //1秒待機;
            }

            if (craneStatus == 12)
            {
                //アーム閉じる音再生;
                //アーム閉じる;
                //1秒待機;
                /*if (credit > 0)
                {
                    craneStatus = 1;
                    credit--;
                }
                else
                {*/
                craneStatus = 0;
                //}
            }
        }
    }
}