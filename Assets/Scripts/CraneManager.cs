using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneManager : MonoBehaviour
{
    protected int craneType;
    protected int craneStatus; //クレーン状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    protected bool probability; //確率判定用
    protected int getSoundNum = -1;
    public CreditSystem creditSystem;
    public SEPlayer sp;
    public CraneBox craneBox;
    protected GetPoint getPoint;
    public MachineHost host;
    protected GameObject canvas;
    public bool isHibernate = false;
    public static bool useUI = true;

    public virtual void GetPrize()
    {
        Debug.Log("CraneManager GetPrize");
        creditSystem.NewPrize();
        switch (creditSystem.probabilityMode)
        {
            case 2:
            case 3:
                creditSystem.ResetCreditProbability();
                break;
            case 4:
            case 5:
                creditSystem.ResetCostProbability();
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
    public int GetStatus()
    {
        return craneStatus;
    }

    public void SetStatus(int next)
    {
        if (next == craneStatus + 1) Debug.LogWarning("craneStatusが1増える操作です．IncrimentStatus()を利用してください．");
        craneStatus = next;
    }

    public void IncrimentStatus()
    {
        craneStatus++;
    }

    public int GetCType()
    {
        return craneType;
    }

    public bool GetProbability()
    {
        return probability;
    }
}
