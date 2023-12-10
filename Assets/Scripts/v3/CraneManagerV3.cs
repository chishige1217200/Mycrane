using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneManagerV3 : MonoBehaviour
{
    public MachineHost host;
    public int craneStatus // クレーン状態
    {
        get { return _craneStatus; }

        set
        {
            LastStatusEvent(_craneStatus);
            _craneStatus = value;
            FirstStatusEvent(_craneStatus);
        }
    }
    private int _craneStatus = 0;
    public SEPlayer sp;
    [SerializeField] protected bool isHibernate = false; // 休止状態
    protected int craneType; // 筐体タイプ
    protected bool testMode = false; // テストプレイモード
    protected bool probability; // 確率判定用
    protected int getSoundNum = -1; // 獲得音番号
    protected CreditSystemV3 creditSystem;
    protected CraneBoxV3 craneBox;
    protected GetPointV3 getPoint;
    protected GameObject canvas;
    public static bool useUI = true; // UI表示

    protected IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    public virtual void GetPrize()
    {
        Debug.Log("CraneManagerV3 GetPrize");
        creditSystem.NewPrize();
        switch (creditSystem.GetMode())
        {
            case 2:
            case 3:
                creditSystem.ResetCreditProbability();
                break;
        }
        if (getSoundNum != -1)
        {
            if (!sp.audioSources[getSoundNum].isPlaying)
                sp.Play(getSoundNum, 1);
        }
    }
    protected abstract void DetectKey(int num);
    public abstract void ButtonDown(int num);
    public abstract void ButtonUp(int num);
    public abstract void InsertCoin();
    public abstract void InsertCoinAuto();
    protected abstract void FirstStatusEvent(int status);
    protected abstract void LastStatusEvent(int status);
    public int GetCraneType()
    {
        return craneType;
    }
    public bool GetProbability()
    {
        return probability;
    }
}
