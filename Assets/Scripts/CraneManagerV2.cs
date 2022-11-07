using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneManagerV2 : MonoBehaviour
{
    protected int craneType;
    public int craneStatus //クレーン状態
    {
        get { return _craneStatus; }

        set
        {
            LastStatusEvent(_craneStatus);
            _craneStatus = value;
            FirstStatusEvent(_craneStatus);
        }
    } //クレーン状態
    private int _craneStatus = 0;
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    protected bool probability; //確率判定用
    protected int getSoundNum = -1;
    public CreditSystem creditSystem;
    public SEPlayer sp;
    public CraneBox craneBox;
    protected GetPointV2 getPoint;
    public MachineHost host;
    protected GameObject canvas;
    public bool isHibernate = false;
    public static bool useUI = true;

    public virtual void GetPrize()
    {
        Debug.Log("CraneManager GetPrize");
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
            if (!sp.audioSource[getSoundNum].isPlaying)
                sp.Play(getSoundNum, 1);
        }
    }

    protected abstract void DetectKey(int num);
    public abstract void ButtonDown(int num);
    public abstract void InsertCoin();
    protected abstract void FirstStatusEvent(int status);


    protected abstract void LastStatusEvent(int status);


    public int GetCType()
    {
        return craneType;
    }

    public bool GetProbability()
    {
        return probability;
    }
}
