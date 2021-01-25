using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPoint : MonoBehaviour
{
    private SEPlayer _SEPlayer;
    private CreditSystem creditSystem;
    public int getSoundNum = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    public void SetGetSound(int num)
    {
        getSoundNum = num; //ゲット効果音番号登録
    }
}
