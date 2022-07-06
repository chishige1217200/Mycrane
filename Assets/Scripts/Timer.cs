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
    SEPlayer sp;
    public void SetSEPlayer(SEPlayer s)
    {
        sp = s;
    }

    public void SetAlertSound(int num)
    {
        soundNum = num; //クレジット投入効果音番号登録
    }

    public async void StartTimer()
    {
        limitTimeNow = limitTime;
        while (limitTimeNow >= 0)
        {
            if (limitTimeNow == 0)
            {
                await Task.Delay(1000);
                break;
            }
            if (limitTimeNow <= thresholdTimeCount)
            {
                if (soundNum != -1) sp.ForcePlay(soundNum);
            }
            await Task.Delay(1000);
            limitTimeNow--;
        }
    }

    public void CancelTimer()
    {
        limitTimeNow = -1;
    }

}
