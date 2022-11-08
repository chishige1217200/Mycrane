using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditSystem : ProbabilitySystem
{
    public int creditDisplayed { get; private set; } = 0; //筐体に表示されるクレジット（表示・参照用）
    private int creditNew = 0; //新規クレジット（割り算の都合上，新規のみを管理）
    private int creditOld = 0; //全クレジット（プレイ可能なものを合算したもの．常に最新ではないことに注意）
    private int nowpaid = 0; //投入金額（開始時リセット）
    public int nowpaidSum { get; private set; } = 0; //投入金額合計
    private int creditPlayedSum = 0; //プレイされたクレジット数
    public bool segUpdateFlag = true; //trueならクレジット情報を7セグに表示，タイマー共存時用
    public int[,] rateSet = new int[2, 2]; //100円1PLAY，500円6PLAYなどのプリセット?
    private int creditSoundNum = -1; //投入時サウンド番号
    public Text[] priceSet = new Text[2]; //プレイ回数に対応する金額表示(timesSetと連携)
    public Text[] timesSet = new Text[2]; //プレイ回数対応表示
    private SEPlayer sp;
    public Text Credit; //残クレジット
    //-------------------------------------------------
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }

    new void Start()
    {
        base.Start();
        StartCoroutine(DelayCoroutine(100, () =>
        {
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

            Credit.text = "00";
        }));
    }

    public void SetHibernate()
    {
        StartCoroutine(DelayCoroutine(150, () =>
        {
            Credit.text = "--";
            timesSet[0].text = "-";
            timesSet[1].text = "-";
            priceSet[0].text = "---";
            priceSet[1].text = "---";
        }));
    }

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
        if (creditSoundNum != -1) sp.ForcePlay(creditSoundNum); //サウンド再生

        if (creditDisplayed >= 100) Credit.text = "99."; //表示更新
        else if (creditDisplayed < 0) Credit.text = "00";
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
        NewPlay();         //ProbabilitySystemの確率用プレイ数を1加算
        creditPlayedSum++; //合計プレイ数を1加算

        if (creditDisplayed >= 100) Credit.text = "99."; //表示更新
        else if (creditDisplayed < 0) Credit.text = "00";
        else Credit.text = creditDisplayed.ToString("D2");

        return creditOld;
    }

    public void ServiceButton()
    {
        creditOld++;
        creditDisplayed = creditOld + creditNew; //クレジット表示を更新
        if (creditSoundNum != -1) sp.ForcePlay(creditSoundNum);
    }

    public void SetCreditSound(int num)
    {
        creditSoundNum = num; //クレジット投入効果音番号登録
    }

    public void SetSEPlayer(SEPlayer s)
    {
        sp = s;
    }
}
