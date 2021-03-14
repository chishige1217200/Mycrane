using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditSystem : MonoBehaviour
{
    public int creditDisplayed = 0; //筐体に表示されるクレジット（表示・参照用）
    private int creditNew = 0; //新規クレジット（割り算の都合上，新規のみを管理）
    private int creditOld = 0; //全クレジット（プレイ可能なものを合算したもの．常に最新ではないことに注意）
    private int nowpaid = 0; //投入金額（開始時リセット）
    private int nowpaidSum = 0; //投入金額合計
    private int creditPlayedSum = 0; //プレイされたクレジット数
    private bool serviceMode = false; //trueならサービスモード
    public bool insertFlag = false; //trueならコイン投入可能，初期化処理用
    public bool segUpdateFlag = true; //trueならクレジット情報を7セグに表示，タイマー共存時用
    public bool playable = true; // trueならプレイ可能，ユーザ指定用
    private int[,] rateSet = new int[2, 2]; //100円1PLAY，500円6PLAYなどのプリセット?
    private int creditSoundNum = -1; //投入時サウンド番号
    public Text[] priceSet = new Text[2]; //プレイ回数に対応する金額表示(timesSetと連携)
    public Text[] timesSet = new Text[2]; //プレイ回数対応表示
    private SEPlayer _SEPlayer;

    //For test-----------------------------------------
    public Text nowPaid; //試験用
    public Text Credit; //残クレジット
    //-------------------------------------------------
    void Start()
    {
        rateSet[0, 0] = 100; //temporary
        rateSet[0, 1] = 1; //temporary
        rateSet[1, 0] = 500; //temporary
        rateSet[1, 1] = 6; //temporary

        if ((float)rateSet[0, 0] / rateSet[0, 1] < (float)rateSet[1, 0] / rateSet[1, 1])
            Debug.Log("rateSet value error."); //高額のレートになるとコストが多くなる設定エラーのとき

        if (playable) //プレイ可能のとき
        {
            if (!serviceMode) //非サービスモード時に，7セグに表示を反映
            {
                priceSet[0].text = rateSet[0, 0].ToString();
                timesSet[0].text = rateSet[0, 1].ToString();
                if (rateSet[0, 0] == rateSet[1, 0] && rateSet[0, 1] == rateSet[1, 1]) // 単一プレイ回数設定時
                {
                    priceSet[1].text = "---"; //下段側は-表示
                    timesSet[1].text = "-";
                }
                else //2つのレート設定があるとき
                {
                    priceSet[1].text = rateSet[1, 0].ToString();
                    timesSet[1].text = rateSet[1, 1].ToString();
                }
            }
            else //サービスモード時
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
        if (segUpdateFlag && playable) // segUpdateFlagはタイマー存在機種のみ使用 falseにすると7セグの表示を更新しない
        {
            if (!serviceMode) // 通常時
            {
                Credit.text = creditDisplayed.ToString();
                //nowPaid.text = nowpaid.ToString();
            }
            else // サービスモード時
            {
                creditOld = 1;
                creditNew = 0;
                creditDisplayed = creditOld + creditNew; //表示は更新してない（処理の都合上計算）
            }
        }

    }

    public void GetPayment(int cost)
    {
        if (!serviceMode && insertFlag && playable) //プレイ可能かつサービスモードでないとき
        {
            nowpaid += cost;
            nowpaidSum += cost;
            nowPaidforProbability += cost;
            if (nowpaid % rateSet[1, 0] == 0 && creditNew < nowpaid / rateSet[1, 0] * rateSet[1, 1]) //高額レート優先（設定されていない場合は低額レートの値を使用）
                creditNew = nowpaid / rateSet[1, 0] * rateSet[1, 1];
            else if (nowpaid % rateSet[0, 0] == 0 && creditNew < nowpaid / rateSet[0, 0] * rateSet[0, 1]) //低額レート
                creditNew = nowpaid / rateSet[0, 0] * rateSet[0, 1];
            else
                Debug.Log("あなたは損または保留をしています"); //いずれでも割り切れない場合

            if (probabilityMode == 4 || probabilityMode == 5)   //書き換え可能始め
                if (nowPaidforProbability % costProbability == 0)
                    if (nowPaidforProbability / costProbability == 1)
                    {
                        creditRemainbyCost = creditOld + creditNew;
                        if (probabilityMode == 5) creditPlayed = 0; //creditPlayedとcreditRemainbyCostの辻褄あわせ
                    }                                           //書き換え可能終わり

            creditDisplayed = creditOld + creditNew; //処理の都合上実行
            if (creditSoundNum != -1) _SEPlayer.ForcePlaySE(creditSoundNum); //サウンド再生
        }
        else Debug.Log("休止中です．"); //プレイできないとき
    }

    public void ResetNowPayment()
    {
        if (!serviceMode && playable)
        {
            if (nowpaid % rateSet[1, 0] == 0 || nowpaid % rateSet[0, 0] == 0) nowpaid = 0; //新規クレジット用に投入された金額で割り切れる部分のみ精算
            else nowpaid = nowpaid % rateSet[0, 0];
            creditOld = creditOld + creditNew; //内部クレジットを更新
            creditNew = 0; //新規クレジットを初期化
            if (creditOld > 0) creditOld--; //クレジット1減らす
            creditDisplayed = creditOld; //creditOldがクレジットの実体（creditNew=0のため）
        }
    }

    public void ServiceButton()
    {
        if (!serviceMode && playable)
        {
            creditOld++;
            creditDisplayed = creditOld + creditNew; //クレジット表示を更新
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
    private int costProbability = 500; //設定金額
    private int nowPaidforProbability = 0; //確率設定用の投入金額
    private int creditRemainbyCost = -1; //設定金額到達時の残クレジット数（初期化時-1）
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
        nowPaidforProbability = nowpaidSum - nowPaidforProbability; //既に投入済みの金額を引き継ぎ（引き継ぎできてない）
        creditPlayed = 0;
    }
}
