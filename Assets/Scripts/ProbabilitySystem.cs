using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilitySystem : MonoBehaviour
{
    [SerializeField] int creditProbability = 3; //設定クレジット数
                                                //private int costProbability = 200; //設定金額
                                                //private int nowPaidforProbability = 0; //確率設定用の投入金額
                                                //private int creditRemainbyCost = -1; //設定金額到達時の残クレジット数（初期化時-1）
    protected int creditPlayed = 0; //現在プレイ中のクレジット数（リセットあり）
    [SerializeField] int[] n = new int[2]; //ランダム確率設定n[0]/n[1]
                                           //private List<int> costList = new List<int>(); //投入された金額をリセット毎に分けて保存
    public int probabilityMode; //0：確率なし，1:ランダム確率，2:クレジット回数固定天井設定，3:クレジット回数周期設定，(4:設定金額天井設定，5:設定金額周期設定) 6:クレジット回数収束天井設定
    protected int prizeCount = 0;

    protected void Start()
    {
        if (probabilityMode == 1)
        {
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
        if (probabilityMode == 0) return true; //常に確率
        if (probabilityMode == 1 && UnityEngine.Random.Range(1, n[1] + 1) <= n[0]) return true; // 1/nの確率（nの数値有効）
        if (probabilityMode == 2 && creditPlayed >= creditProbability) return true;
        // *景品獲得時にResetCreditProbability()の処理が必要

        if (probabilityMode == 3 && creditPlayed % creditProbability == 0 && creditPlayed != 0)
        {
            ResetCreditProbability();
            return true;
        }

        /*if (probabilityMode == 4 && creditPlayed >= creditRemainbyCost && creditRemainbyCost != -1) return true; 無効オプション
        // *景品獲得時にResetCostProbability()の処理が必要

        if (probabilityMode == 5 && creditPlayed == creditRemainbyCost && creditRemainbyCost != -1) 無効オプション
        {
            ResetCostProbability();
            return true;
        }*/

        if(probabilityMode == 6 && creditPlayed / (prizeCount + 1) >= creditProbability) return true;
        return false;
    }

    public void ResetCreditProbability()
    {
        creditPlayed = 0;
    }

    public void ResetCostProbability() //設定金額ベースの確率リセット
    {
        /*int outCredit = creditPlayed - creditRemainbyCost; //確率超過プレイ回数
        int outCost = 0;
        int count = 0;
        if (creditPlayed > creditRemainbyCost && probabilityMode == 4) //過剰に消費したクレジット分の代金を引く
        {
            Debug.Log(costList.Count);
            for (int i = 0; i < costList.Count; i++)
                Debug.Log("costList[" + i + "] : " + costList[i]);

            outCost = outCredit / rateSet[1, 1] * rateSet[1, 0]; //高額レート換算分
            outCredit -= outCredit / rateSet[1, 1]; //高額レートで換算された分を除外
            outCost += outCredit / rateSet[0, 1] * rateSet[0, 0]; //低額レート換算分
        }
        for (int i = 0; i < costList.Count; i++)
        {
            if (outCost == 0) break;
            if (costList[i] > outCost)
            {
                costList[i] -= outCost;
                outCost = 0;
            }
            else
            {
                outCost -= costList[i];
                count++;
            }
        }
        costList.RemoveRange(0, count);

        creditRemainbyCost = -1;
        creditPlayed = 0;
        if (creditDisplayed == 0 && probabilityMode == 4) costList.Clear();*/
    }

    /*int CreditReproduction()
    {
        int count = 0; //設定金額に達するまでの要素数
        int costSum = 0; //金額の合計管理
        int creditSum = 0; //プレイされるであろう回数の合計（確率用）
        int creditTemp; //creditNewの位置づけ
        int paid = 0; //nowPaidの位置づけ
        int costSumforRemove = 0; //リスト整理のための金額合計管理

        Debug.Log(costList.Count);
        for (int i = 0; i < costList.Count; i++)
            Debug.Log("costList[" + i + "] : " + costList[i]);

        while (true)
        {
            if (creditRemainbyCost == -1)
            {
                if (costSum >= costProbability) break;
                if (count >= costList.Count) return -1; //合計金額が確率に満たないとき
                costSum += costList[count];
                count++;
            }
            else return creditRemainbyCost;
        }

        Debug.Log("First step passed.");

        for (int i = 0; i < count; i++)
        {
            creditTemp = 0; //creditNewと同じ位置づけのため，毎回初期化
            paid = costList[i]; //未精算分がないことに注意
            if (paid % rateSet[1, 0] == 0 && creditTemp < paid / rateSet[1, 0] * rateSet[1, 1]) //高額レート優先（設定されていない場合は低額レートの値を使用）
                creditTemp = paid / rateSet[1, 0] * rateSet[1, 1];
            else if (paid % rateSet[0, 0] == 0 && creditTemp < paid / rateSet[0, 0] * rateSet[0, 1]) //低額レート
                creditTemp = paid / rateSet[0, 0] * rateSet[0, 1];
            creditSum += creditTemp; //クレジット数合計
            costSumforRemove += paid; //投入金額合計
            if (costSumforRemove > costProbability) //設定金額より同時に多く投入されている場合の繰越し
            {
                costList[i] = costSumforRemove - costProbability; //繰越金計算
                count--; //繰越されたリストが先頭に来るようにする
            }
        }
        costList.RemoveRange(0, count);
        if (creditSum > costProbability / rateSet[1, 0] * rateSet[1, 1]) creditSum = costProbability / rateSet[1, 0] * rateSet[1, 1];
        return creditSum;
    }*/
    // 1回分だけで設定金額を上回った場合の対策を行うこと
}
