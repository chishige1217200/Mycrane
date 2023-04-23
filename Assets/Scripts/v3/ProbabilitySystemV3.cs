using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シリアライズはなしで別途値を設定するように
public class ProbabilitySystemV3 : MonoBehaviour
{
    [SerializeField] int mode = 0; // 0：確率なし，1:ランダム確率，2:クレジット回数固定天井設定，3:クレジット回数周期設定，4:クレジット回数収束天井設定
    [SerializeField] int creditProbability = 3; // 設定クレジット数
    [SerializeField] int[] n = new int[2]; // ランダム確率設定n[0]/n[1]
    //-------------------------------------------------
    private int creditPlayed = 0; // 現在プレイ中のクレジット数（リセットあり）
    private int prizeCount = 0; // 景品獲得数
    //-------------------------------------------------
    public int GetMode()
    {
        return mode;
    }

    public void Setup(int mode, int creditProbability, int n1, int n2)
    {
        this.mode = mode;
        this.creditProbability = creditProbability;
        n[0] = n1;
        n[1] = n2;

        if (mode == 1)
        {
            if(n[1] == 0) n[1] = 1;
            if (n.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
            if (n[0] > n[1]) // 確率分数に不正な値が設定されたとき
            {
                int temp = n[0];
                n[0] = n[1];
                n[1] = temp;
            }
        }
    }

    public void NewPlay()
    {
        creditPlayed++; //確率用プレイ数を1加算
    }

    public void NewPrize()
    {
        prizeCount++;
    }

    public bool ProbabilityCheck()
    {
        if (mode == 0) return true; // 常に確率
        if (mode == 1 && UnityEngine.Random.Range(1, n[1] + 1) <= n[0]) return true; // 1/nの確率（nの数値有効）
        if (mode == 2 && creditPlayed >= creditProbability) return true; // 景品獲得時にResetCreditProbability()の処理が必要f


        if (mode == 3 && creditPlayed % creditProbability == 0 && creditPlayed != 0)
        {
            ResetCreditProbability();
            return true;
        }

        if (mode == 4 && creditPlayed / (prizeCount + 1) >= creditProbability) return true;
        return false;
    }

    public void ResetCreditProbability()
    {
        creditPlayed = 0;
    }

    public void ResetAll()
    {
        creditPlayed = 0;
        prizeCount = 0;
    }
}
