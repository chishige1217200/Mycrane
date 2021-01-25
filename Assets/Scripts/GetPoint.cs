using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GetPoint : MonoBehaviour
{
    private SEPlayer _SEPlayer;
    private CreditSystem creditSystem;
    public int getSoundNum = -1;
    private bool checkFlag = true; // trueのときのみ獲得口チェック

    async void Update()
    {
        if (!checkFlag)
        {
            await Task.Delay(5000);
            checkFlag = true;
        }
    }

    public void GetSEPlayer(SEPlayer s)
    {
        _SEPlayer = s;
    }

    public void GetCreditSystem(CreditSystem c)
    {
        creditSystem = c;
    }

    void OnTriggerEnter()
    {
        if (checkFlag)
        {
            checkFlag = false;
            if (getSoundNum != -1)
                _SEPlayer.PlaySE(getSoundNum, 1);
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
    }

    public void SetGetSound(int num)
    {
        getSoundNum = num; //ゲット効果音番号登録
    }
}
