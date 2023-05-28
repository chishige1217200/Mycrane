using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// シリアライズはなしで別途値を設定するように
public class CreditSystemV3 : ProbabilitySystemV3
{
    public int[] priceSets = new int[2]; // 原則Setup()で与える
    public int[] timesSets = new int[2]; // 原則Setup()で与える
    public Text[] priceTexts = new Text[2]; // プレイ回数に対応する金額表示(priceSetsと連携)
    public Text[] timesTexts = new Text[2]; // プレイ回数対応表示(timesSetsと連携)
    public Text Credit; // 残クレジット表示
    [HideInInspector] public bool segUpdateFlag = true; // trueならクレジット情報を7セグに表示，タイマー共存時用
    //-------------------------------------------------
    private int creditDisplayed = 0; // 筐体に表示されるクレジット（表示・参照用）
    private int nowpaidSum = 0; // 投入金額合計
    private int creditNew = 0; // 新規クレジット（割り算の都合上，新規のみを管理）
    private int creditOld = 0; // 全クレジット（プレイ可能なものを合算したもの．常に最新ではないことに注意）
    private int nowpaid = 0; // 投入金額（開始時リセット）
    private int soundNum = -1; // 投入時サウンド番号
    private bool isInitialized = false; // 初期化されたらtrueに
    private SEPlayer sp;
    //-------------------------------------------------

    public int GetCreditCount()
    {
        return creditDisplayed;
    }

    public int GetNowpaidSum()
    {
        return nowpaidSum;
    }

    public void Setup()
    {
        if (priceSets[0] == 0 || timesSets[0] == 0)
        {
            priceSets[0] = 100;
            timesSets[0] = 1;
        }
        if (priceSets[1] == 0 || timesSets[1] == 0 || (float)priceSets[0] / timesSets[0] < (float)priceSets[1] / timesSets[1])
        // 未入力の場合，低価格設定を反映 // 高額のレートになるとコストが多くなる設定エラーのとき
        {
            priceSets[1] = priceSets[0];
            timesSets[1] = timesSets[0];
        }

        priceTexts[0].text = priceSets[0].ToString();
        timesTexts[0].text = timesSets[0].ToString();
        if (priceSets[0] == priceSets[1] && timesSets[0] == timesSets[1]) // 単一プレイ回数設定時
        {
            priceTexts[1].text = ""; // 下段側は表示しない
            timesTexts[1].text = "";
        }
        else // 2つのレート設定があるとき
        {
            priceTexts[1].text = priceSets[1].ToString();
            timesTexts[1].text = timesSets[1].ToString();
        }
        Credit.text = "00";
        isInitialized = true;
    }

    // SEPlayerとsoundNumの設定も行うこと
    public void Setup(int p1, int t1, int p2, int t2)
    {
        priceSets[0] = p1;
        timesSets[0] = t1;
        priceSets[1] = p2;
        timesSets[1] = t2;

        if (priceSets[0] == 0 || timesSets[0] == 0)
        {
            priceSets[0] = 100;
            timesSets[0] = 1;
        }
        if (priceSets[1] == 0 || timesSets[1] == 0 || (float)priceSets[0] / timesSets[0] < (float)priceSets[1] / timesSets[1])
        // 未入力の場合，低価格設定を反映 // 高額のレートになるとコストが多くなる設定エラーのとき
        {
            priceSets[1] = priceSets[0];
            timesSets[1] = timesSets[0];
        }

        priceTexts[0].text = priceSets[0].ToString();
        timesTexts[0].text = timesSets[0].ToString();
        if (priceSets[0] == priceSets[1] && timesSets[0] == timesSets[1]) // 単一プレイ回数設定時
        {
            priceTexts[1].text = ""; // 下段側は表示しない
            timesTexts[1].text = "";
        }
        else // 2つのレート設定があるとき
        {
            priceTexts[1].text = priceSets[1].ToString();
            timesTexts[1].text = timesSets[1].ToString();
        }
        Credit.text = "00";
        isInitialized = true;
    }

    public void SetHibernate()
    {
        Credit.text = "--";
        timesTexts[0].text = "-";
        timesTexts[1].text = "-";
        priceTexts[0].text = "---";
        priceTexts[1].text = "---";
    }

    public int Pay(int cost)
    {
        if (isInitialized)
        {
            if (cost == 0) return creditDisplayed;

            nowpaid += cost;
            nowpaidSum += cost;
            if (nowpaid % priceSets[1] == 0 && creditNew < nowpaid / priceSets[1] * timesSets[1]) // 高額レート優先（設定されていない場合は低額レートの値を使用）
            {
                creditNew = nowpaid / priceSets[1] * timesSets[1];
                ResetPay();
            }
            else if (nowpaid % priceSets[0] == 0 && creditNew < nowpaid / priceSets[0] * timesSets[0]) // 低額レート
                creditNew = nowpaid / priceSets[0] * timesSets[0];
            else
                Debug.Log("あなたは損または保留をしています"); // いずれでも割り切れない場合

            creditDisplayed = creditOld + creditNew; // 内部クレジットを更新
            if (soundNum != -1) sp.ForcePlay(soundNum); // サウンド再生

            UpdateSeg();
        }
        return creditDisplayed;
    }

    public int ServiceButton()
    {
        creditOld++;
        creditDisplayed = creditOld;
        if (soundNum != -1) sp.ForcePlay(soundNum); // サウンド再生
        UpdateSeg();

        return creditOld;
    }

    public void ResetPay()
    {
        if (nowpaid % priceSets[1] == 0 || nowpaid % priceSets[0] == 0) nowpaid = 0; // 新規クレジット用に投入された金額で割り切れる部分のみ精算
        else nowpaid = nowpaid % priceSets[0];
        creditOld = creditOld + creditNew; // 内部クレジットを更新
        creditNew = 0; // 新規クレジットを初期化
        creditDisplayed = creditOld; // creditOldがクレジットの実体（creditNew=0のため）
    }

    // 確率機は別途NewPlay()を呼ぶこと
    public int DecrementCredit()
    {
        creditOld--;
        creditDisplayed = creditOld;
        // NewPlay(); // ProbabilitySystemの確率用プレイ数を1加算
        UpdateSeg();

        return creditOld;
    }

    private void UpdateSeg()
    {
        if (segUpdateFlag)
        {
            if (creditDisplayed >= 100) Credit.text = "99."; // 表示更新
            else if (creditDisplayed < 0) Credit.text = "00";
            else Credit.text = creditDisplayed.ToString("D2");
        }
    }

    public void SetSoundNum(int num)
    {
        soundNum = num; // クレジット投入効果音番号登録
    }

    public void SetSEPlayer(SEPlayer s)
    {
        sp = s;
    }

    public new void ResetAll()
    {
        creditDisplayed = 0;
        creditNew = 0;
        creditOld = 0;
        nowpaid = 0;
        nowpaidSum = 0;
        UpdateSeg();
    }
}
