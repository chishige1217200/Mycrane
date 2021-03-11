using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditSystem : MonoBehaviour
{
    private int creditNew = 0; //新規クレジット
    public int creditAll = 0; //全クレジット（全体管理用）
    public int creditDisplayed = 0; //筐体に表示されるクレジット（表示専用）
    private int nowpaid = 0; //投入金額（開始時リセット）
    private int nowpaidSum = 0; //投入金額合計
    private int creditPlayedSum = 0; //プレイされたクレジット数
    private bool serviceMode = false; // trueならサービスモード
    public bool insertFlag = false; // trueならコイン投入可能，初期化処理街用
    public bool segUpdateFlag = true; // trueならクレジット情報を7セグに表示，タイマー共存時用
    public bool playable = true; // trueならプレイ可能，ユーザ指定用
    private int[,] rateSet = new int[2, 2]; //100円1PLAY，500円6PLAYなどのプリセット?
    private int creditSoundNum = -1; //投入時サウンド番号
    private SEPlayer _SEPlayer;

    //For test-----------------------------------------

    public Text nowPaid; //試験用
    public Text Credit; //残クレジット
    public Text[] priceSet = new Text[2];
    public Text[] timesSet = new Text[2];
    //-------------------------------------------------
    void Start()
    {
        rateSet[0, 0] = 100; //temporary
        rateSet[0, 1] = 1; //temporary
        rateSet[1, 0] = 500; //temporary
        rateSet[1, 1] = 6; //temporary

        if ((float)rateSet[0, 0] / rateSet[0, 1] < (float)rateSet[1, 0] / rateSet[1, 1])
            Debug.Log("rateSet value error."); //高額のレートになるとコストが多くなる設定エラーのとき

        if (playable)
        {
            if (!serviceMode)
            {
                priceSet[0].text = rateSet[0, 0].ToString();
                timesSet[0].text = rateSet[0, 1].ToString();
                if (rateSet[0, 0] == rateSet[1, 0] && rateSet[0, 1] == rateSet[1, 1]) // 単一プレイ回数設定
                {
                    priceSet[1].text = "---";
                    timesSet[1].text = "-";
                }
                else
                {
                    priceSet[1].text = rateSet[1, 0].ToString();
                    timesSet[1].text = rateSet[1, 1].ToString();
                }
            }
            else
            {
                priceSet[0].text = "---";
                priceSet[1].text = "---";
                timesSet[0].text = "-";
                timesSet[1].text = "-";
                Credit.text = "--";
                nowPaid.text = "--";
                Debug.Log("サービスモードです");
            }
        }
    }

    void Update()
    {
        if (segUpdateFlag && playable) // segUpdateFlagはタイマー存在機種のみ使用 falseにすると表示を更新しない
        {
            if (!serviceMode) // 通常時
            {
                Credit.text = creditDisplayed.ToString();
                //nowPaid.text = nowpaid.ToString();
            }
            else // サービスモード時
            {
                creditAll = 1;
                creditNew = 0;
                creditDisplayed = creditAll + creditNew; // 表示は更新してない
            }
        }

    }

    public void GetPayment(int cost)
    {
        if (!serviceMode && insertFlag && playable)
        {
            nowpaid += cost;
            nowpaidSum += cost;
            nowPaidforProbability += cost;
            if (nowpaid % rateSet[1, 0] == 0 && creditNew < nowpaid / rateSet[1, 0] * rateSet[1, 1])
                creditNew = nowpaid / rateSet[1, 0] * rateSet[1, 1];
            else if (nowpaid % rateSet[0, 0] == 0 && creditNew < nowpaid / rateSet[0, 0] * rateSet[0, 1])
                creditNew = nowpaid / rateSet[0, 0] * rateSet[0, 1];
            else
                Debug.Log("あなたは損または保留をしています");

            if (probabilityMode == 4 || probabilityMode == 5)
                if (nowPaidforProbability % costProbability == 0)
                    if (nowPaidforProbability / costProbability == 1)
                    {
                        creditRemainbyCost = creditAll + creditNew;
                        if (probabilityMode == 5) creditPlayed = 0; // creditPlayedとcreditRemainbyCostの辻褄あわせ
                    }

            creditDisplayed = creditAll + creditNew;
            if (creditSoundNum != -1) _SEPlayer.ForcePlaySE(creditSoundNum);
        }
        else Debug.Log("休止中です．");
    }

    public void ResetNowPayment()
    {
        if (!serviceMode && playable)
        {
            if (nowpaid % rateSet[1, 0] == 0 || nowpaid % rateSet[0, 0] == 0) nowpaid = 0;
            else nowpaid = nowpaid % rateSet[0, 0];
            creditAll = creditAll + creditNew; //実クレジットを更新
            creditNew = 0; //新規クレジットを初期化
            if (creditAll > 0) creditAll--; //クレジット1減らす
            creditDisplayed = creditAll + creditNew; //CreditAllがクレジットの実体
        }
    }

    public void ServiceButton()
    {
        if (!serviceMode && playable)
        {
            creditAll++;
            creditDisplayed = creditAll + creditNew; //クレジット表示を更新
            if (creditSoundNum != -1) _SEPlayer.ForcePlaySE(creditSoundNum);
        }
    }

    public void SetCreditSound(int num)
    {
        creditSoundNum = num; //クレジット投入効果音番号登録
    }

    public void GetSEPlayer(SEPlayer s)
    {
        _SEPlayer = s;
    }

    //Probability Function-----------------------------------------------

    private int creditProbability = 3; //設定クレジット数
    private int costProbability = 0; //設定金額
    private int nowPaidforProbability = 0; //確率設定用の投入金額
    private int creditRemainbyCost = -1; //設定金額到達時の残クレジット数
    private int creditPlayed = 0; //現在プレイ中のクレジット数（リセットあり）
    private int n = 3; //ランダム確率設定n
    public int probabilityMode; //0：確率なし，1:ランダム確率，2:クレジット回数天井設定，3:クレジット回数周期設定，4:設定金額天井設定，5:設定金額周期設定

    public bool ProbabilityCheck()
    {
        Debug.Log("creditPlayed" + creditPlayed + " creditRemainbyCost" + creditRemainbyCost);
        if (probabilityMode == 0) return true; //常に確率
        if (probabilityMode == 1 && UnityEngine.Random.Range(1, n + 1) == 1) return true; // 1/nの確率（nの数値有効）
        if (probabilityMode == 2 && creditPlayed >= creditProbability) return true;
        // *景品獲得時にResetCreditProbability()の処理が必要

        if (probabilityMode == 3 && creditPlayed % creditProbability == 0 && creditPlayed != 0)
        {
            ResetCreditProbability();
            return true;
        }
        if (!serviceMode) //お金を投入する場合のみ
        {
            if (probabilityMode == 4 && creditPlayed >= creditRemainbyCost && creditRemainbyCost != -1) return true;
            // *景品獲得時にResetCostProbability()の処理が必要

            if (probabilityMode == 5 && creditPlayed == creditRemainbyCost && creditRemainbyCost != -1)
            {
                ResetCostProbability();
                return true;
            }
        }
        return false;
    }

    public void AddCreditPlayed() //プレイ開始時に1加算
    {
        creditPlayed++; //確率用プレイ数を1加算
        creditPlayedSum++; //合計プレイ数を1加算
    }

    public void ResetCreditProbability()
    {
        creditPlayed = 0;
    }

    public void ResetCostProbability() //設定金額ベースの確率リセット
    {
        creditRemainbyCost = -1;
        nowPaidforProbability = nowpaidSum - nowPaidforProbability; //既に投入済みの金額を引き継ぎ
        creditPlayed = 0;
    }
}
