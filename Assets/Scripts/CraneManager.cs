using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraneManager : MonoBehaviour
{
    protected int craneType;
    protected int craneStatus; //クレーン状態
    protected bool probability; //確率判定用
    protected bool probabilityReset = true; //景品獲得時に確率をリセットするか
    protected int getSoundNum = -1;
    protected CreditSystem creditSystem;
    protected SEPlayer _SEPlayer;
    protected CraneBox craneBox;
    protected GetPoint getPoint;
    protected MachineHost host;
    protected GameObject canvas;

    public void GetPrize()
    {
        if (probabilityReset)
        {
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
        }
        if (!_SEPlayer._AudioSource[getSoundNum].isPlaying)
        {
            if (getSoundNum != -1)
                _SEPlayer.PlaySE(getSoundNum, 1);
        }
    }

    protected abstract void DetectKey(int num);
    public abstract void ButtonDown(int num);
    public abstract void InsertCoin();
    public int GetStatus()
    {
        return craneStatus;
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
