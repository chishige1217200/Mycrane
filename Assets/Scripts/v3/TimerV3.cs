using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerV3 : MonoBehaviour
{
    [SerializeField] int[] alertSoundNums = new int[5]; // 配列の要素番号と音を再生する残り秒が一致します
    [SerializeField] SEPlayer sp;
    private int limitTimeNow = 0; //実際のカウントダウン
    private Coroutine timerCoroutine;

    public void SetSEPlayer(SEPlayer sp)
    {
        this.sp = sp;
    }

    public int GetLimitTimeNow()
    {
        return limitTimeNow;
    }

    public void Activate(int limitTime)
    {
        timerCoroutine = StartCoroutine(InternalActive(limitTime));
    }

    public void Cancel()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
    }

    IEnumerator InternalActive(int limitTime)
    {
        limitTimeNow = limitTime;
        while (true)
        {
            if (limitTimeNow == 0)
            {
                sp.ForcePlay(alertSoundNums[limitTimeNow]);
                break;
            }
            if (limitTimeNow < alertSoundNums.Length)
            {
                if (alertSoundNums[limitTimeNow] >= 0) sp.ForcePlay(alertSoundNums[limitTimeNow]);
            }
            yield return new WaitForSeconds(1);
            limitTimeNow--;
        }
    }
}
