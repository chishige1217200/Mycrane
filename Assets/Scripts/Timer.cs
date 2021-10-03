using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int limitTime = 15; //レバー式の場合，残り時間を設定
    public int limitTimeNow = 0; //実際のカウントダウン
    public int thresholdTimeCount = 10; //この秒数以下になると警告音
    public int soundNum = -1;
    //public bool isCounting = false; //タイマー起動中
    //CreditSystem creditSystem;
    SEPlayer _SEPlayer;
    public void SetSEPlayer(SEPlayer s)
    {
        _SEPlayer = s;
    }

    public void SetAlertSound(int num)
    {
        soundNum = num; //クレジット投入効果音番号登録
    }

    public async void StartTimer()
    {
        limitTimeNow = limitTime;
        //creditSystem.segUpdateFlag = false;
        while (limitTimeNow >= 0)
        {
            if (limitTimeNow == 0)
            {
                //craneStatus = 6;
                await Task.Delay(1000);
                //creditSystem.segUpdateFlag = true;
                break;
            }
            if (limitTimeNow <= thresholdTimeCount)
            {
                if (soundNum != -1) _SEPlayer.ForcePlay(soundNum);
            }
            await Task.Delay(1000);
            limitTimeNow--;
        }
    }

    public void CancelTimer()
    {
        limitTimeNow = -1;
        //creditSystem.segUpdateFlag = true;
    }

}
