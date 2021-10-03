using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CreditSystem : MonoBehaviour
{
    public int creditDisplayed { get; private set; } = 0; //筐体に表示されるクレジット（表示・参照用）
    private int creditNew = 0; //新規クレジット（割り算の都合上，新規のみを管理）
    private int creditOld = 0; //全クレジット（プレイ可能なものを合算したもの．常に最新ではないことに注意）
    private int nowpaid = 0; //投入金額（開始時リセット）
    private int nowpaidSum = 0; //投入金額合計
    private int creditPlayedSum = 0; //プレイされたクレジット数
    //private bool serviceMode = false; //trueならサービスモード
    //public bool insertFlag = false; //trueならコイン投入可能，初期化処理用
    public bool segUpdateFlag = true; //trueならクレジット情報を7セグに表示，タイマー共存時用
    //public bool playable = true; // trueならプレイ可能，ユーザ指定用
    public int[,] rateSet = new int[2, 2]; //100円1PLAY，500円6PLAYなどのプリセット?
    private int creditSoundNum = -1; //投入時サウンド番号
    public Text[] priceSet = new Text[2]; //プレイ回数に対応する金額表示(timesSetと連携)
    public Text[] timesSet = new Text[2]; //プレイ回数対応表示
    private SEPlayer _SEPlayer;
    [SerializeField] Text Credit; //残クレジット
    //-------------------------------------------------
    async void Start()
    {
        await Task.Delay(100);

        if (rateSet[0, 0] == 0 || rateSet[0, 1] == 0)
        {
            rateSet[0, 0] = 100;
            rateSet[0, 1] = 1;
        }
        if (rateSet[1, 0] == 0 || rateSet[1, 1] == 0 || (float)rateSet[0, 0] / rateSet[0, 1] < (float)rateSet[1, 0] / rateSet[1, 1])
        // 未入力の場合，低価格設定を反映 //高額のレートになるとコストが多くなる設定エラーのとき
        {
            rateSet[1, 0] = rateSet[0, 0];
            rateSet[1, 1] = rateSet[0, 1];
        }

        //if (!serviceMode) //非サービスモード時に，7セグに表示を反映
        //{
        priceSet[0].text = rateSet[0, 0].ToString();
        timesSet[0].text = rateSet[0, 1].ToString();
        if (rateSet[0, 0] == rateSet[1, 0] && rateSet[0, 1] == rateSet[1, 1]) // 単一プレイ回数設定時
        {
            priceSet[1].text = ""; //下段側は表示しない
            timesSet[1].text = "";
        }
        else //2つのレート設定があるとき
        {
            priceSet[1].text = rateSet[1, 0].ToString();
            timesSet[1].text = rateSet[1, 1].ToString();
        }
        //}
        /*else //サービスモード時
        {
            priceSet[0].text = "fre";
            priceSet[1].text = "mod";
            timesSet[0].text = "e";
            timesSet[1].text = "e";
            Credit.text = " F";
            Debug.Log("サービスモードです");
        }*/

        Credit.text = "00";
    }

    /*void Update()
    {
        if (segUpdateFlag) // segUpdateFlagはタイマー存在機種のみ使用 falseにすると7セグの表示を更新しない
        {
            if (!serviceMode) // 通常時
            {
                if (creditDisplayed >= 100) Credit.text = "99.";
                else Credit.text = creditDisplayed.ToString();
                //nowPaid.text = nowpaid.ToString();
            }
            else // サービスモード時
            {
                creditOld = 1;
                creditNew = 0;
                creditDisplayed = creditOld + creditNew; //表示は更新してない（処理の都合上計算）
                Credit.text = " F";
            }
        }
    }*/

    public int Pay(int cost)
    {
        if (cost == 0) return creditDisplayed;

        nowpaid += cost;
        nowpaidSum += cost;
        if (nowpaid % rateSet[1, 0] == 0 && creditNew < nowpaid / rateSet[1, 0] * rateSet[1, 1]) //高額レート優先（設定されていない場合は低額レートの値を使用）
        {
            creditNew = nowpaid / rateSet[1, 0] * rateSet[1, 1];
            ResetPayment();
        }
        else if (nowpaid % rateSet[0, 0] == 0 && creditNew < nowpaid / rateSet[0, 0] * rateSet[0, 1]) //低額レート
            creditNew = nowpaid / rateSet[0, 0] * rateSet[0, 1];
        else
            Debug.Log("あなたは損または保留をしています"); //いずれでも割り切れない場合

        creditDisplayed = creditOld + creditNew; //内部クレジットを更新
        if (creditSoundNum != -1) _SEPlayer.ForcePlay(creditSoundNum); //サウンド再生

        if (creditDisplayed >= 100) Credit.text = "99."; //表示更新
        else Credit.text = creditDisplayed.ToString("D2");

        return creditDisplayed;
    }

    public void ResetPayment()
    {
        if (nowpaid % rateSet[1, 0] == 0 || nowpaid % rateSet[0, 0] == 0) nowpaid = 0; //新規クレジット用に投入された金額で割り切れる部分のみ精算
        else nowpaid = nowpaid % rateSet[0, 0];
        creditOld = creditOld + creditNew; //内部クレジットを更新
        creditNew = 0; //新規クレジットを初期化
        creditDisplayed = creditOld; //creditOldがクレジットの実体（creditNew=0のため）
    }

    public int PlayStart()
    {
        creditOld--;
        creditDisplayed = creditOld;
        creditPlayed++; //確率用プレイ数を1加算
        creditPlayedSum++; //合計プレイ数を1加算

        if (creditDisplayed >= 100) Credit.text = "99."; //表示更新
        else Credit.text = creditDisplayed.ToString("D2");

        return creditOld;
    }

    /*public void GetPayment(int cost)
    {
        if (!serviceMode) //プレイ可能かつサービスモードでないとき
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

            creditDisplayed = creditOld + creditNew; //処理の都合上実行
            if (creditSoundNum != -1) _SEPlayer.ForcePlaySE(creditSoundNum); //サウンド再生
        }
        else Debug.Log("休止中です．"); //プレイできないとき
    }

    public void ResetNowPayment()
    {
        //int paid; //今回精算分
        if (!serviceMode)
        {
            //paid = nowpaid; //今回の支払い分
            if (nowpaid % rateSet[1, 0] == 0 || nowpaid % rateSet[0, 0] == 0) nowpaid = 0; //新規クレジット用に投入された金額で割り切れる部分のみ精算
            else nowpaid = nowpaid % rateSet[0, 0];
            //paid -= nowpaid; //未精算分を差し引き
            /*if (paid != 0 && creditPlayed < creditProbability && (probabilityMode == 4 || probabilityMode == 5)) costList.Add(paid); //精算分を追加
            if ((probabilityMode == 4 || probabilityMode == 5) && costList.Count > 0)
                creditRemainbyCost = CreditReproduction();*/
    /*creditOld = creditOld + creditNew; //内部クレジットを更新
    creditNew = 0; //新規クレジットを初期化
    if (creditOld > 0) creditOld--; //クレジット1減らす
    creditDisplayed = creditOld; //creditOldがクレジットの実体（creditNew=0のため）
    }
    }*/

    public void ServiceButton()
    {
        //if (!serviceMode)
        //{
        creditOld++;
        creditDisplayed = creditOld + creditNew; //クレジット表示を更新
        if (creditSoundNum != -1) _SEPlayer.ForcePlay(creditSoundNum);
        //}
    }

    public void SetCreditSound(int num)
    {
        creditSoundNum = num; //クレジット投入効果音番号登録
    }

    public void SetSEPlayer(SEPlayer s)
    {
        _SEPlayer = s;
    }

    //Probability Function-----------------------------------------------

    [SerializeField] int creditProbability = 3; //設定クレジット数
                                                //private int costProbability = 200; //設定金額
                                                //private int nowPaidforProbability = 0; //確率設定用の投入金額
                                                //private int creditRemainbyCost = -1; //設定金額到達時の残クレジット数（初期化時-1）
    private int creditPlayed = 0; //現在プレイ中のクレジット数（リセットあり）
    [SerializeField] int n = 3; //ランダム確率設定n
                                //private List<int> costList = new List<int>(); //投入された金額をリセット毎に分けて保存
    public int probabilityMode; //0：確率なし，1:ランダム確率，2:クレジット回数天井設定，3:クレジット回数周期設定，(4:設定金額天井設定，5:設定金額周期設定)

    public bool ProbabilityCheck()
    {
        if (probabilityMode == 0) return true; //常に確率
        if (probabilityMode == 1 && UnityEngine.Random.Range(1, n + 1) == 1) return true; // 1/nの確率（nの数値有効）
        if (probabilityMode == 2 && creditPlayed >= creditProbability) return true;
        // *景品獲得時にResetCreditProbability()の処理が必要

        if (probabilityMode == 3 && creditPlayed % creditProbability == 0 && creditPlayed != 0)
        {
            ResetCreditProbability();
            return true;
        }
        /*if (!serviceMode) //お金を投入する場合のみ
        {
            if (probabilityMode == 4 && creditPlayed >= creditRemainbyCost && creditRemainbyCost != -1) return true; 無効オプション
            // *景品獲得時にResetCostProbability()の処理が必要

            if (probabilityMode == 5 && creditPlayed == creditRemainbyCost && creditRemainbyCost != -1) 無効オプション
            {
                ResetCostProbability();
                return true;
            }
        }*/
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
