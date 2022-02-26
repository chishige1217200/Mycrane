using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Type9Manager : CraneManager
{
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    [SerializeField] float leftCatchArmpower = 20f; //左アームパワー
    [SerializeField] float rightCatchArmpower = 20f; //右アームパワー
    [SerializeField] float armApertures = 80f; //開口率
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    Type9ArmController armController;
    ArmUnitLifter lifter;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];
    CraneUnitRoter roter;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];

    async void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 9;
        // 様々なコンポーネントの取得
        //host = transform.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            credit3d.text = "---";
            creditSystem.SetHibernate();
            preset[0].text = "---";
            preset[1].text = "---";
            preset[2].text = "-";
            preset[3].text = "-";
        }
        else
        {
            preset[0].text = priceSet[0].ToString();
            preset[1].text = priceSet[1].ToString();
            preset[2].text = timesSet[0].ToString();
            preset[3].text = timesSet[1].ToString();
        }

        // ロープとアームコントローラに関する処理
        lifter = temp.Find("CraneBox").Find("Tube").Find("TubePoint").GetComponent<ArmUnitLifter>();
        armController = temp.Find("ArmUnit").GetComponent<Type9ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        nail[0] = temp.Find("ArmUnit").Find("Arm1").Find("Nail1").GetComponent<ArmNail>();
        nail[1] = temp.Find("ArmUnit").Find("Arm2").Find("Nail2").GetComponent<ArmNail>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        roter = temp.GetComponent<CraneUnitRoter>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);

        creditSystem.SetCreditSound(0);
        support.SetManager(this);
        support.SetLifter(lifter);
        support.pushTime = 300; // 押し込みパワーの調整
        getSoundNum = 4;
        for (int i = 0; i < 2; i++)
        {
            nail[i].SetManager(this);
            nail[i].SetLifter(lifter);
        }

        getPoint.SetManager(this);

        await Task.Delay(300);
        lifter.Up();
        while (!lifter.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.SetLimit(armApertures);

        for (int i = 0; i < 13; i++)
            isExecuted[i] = false;

        IncrimentStatus();
    }

    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();

        if (craneStatus == -1)
            if (roter.CheckPos(2)) IncrimentStatus();
        if (craneStatus > 0)
        {
            if (craneStatus == 1)
            {
                DetectKey(craneStatus);
            }
            if (craneStatus == 2)
            {
                DetectKey(craneStatus);
                sp.Play(1);
                if (roter.CheckPos(1))
                {
                    buttonPushed = false;
                    IncrimentStatus();
                }
            }
            if (craneStatus == 3)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Play(2, 1);
                }
                DetectKey(craneStatus);
                sp.Stop(1);
            }
            if (craneStatus == 4)
            {
                DetectKey(craneStatus);
                sp.Play(1);
                if (craneBox.CheckPos(8))
                {
                    buttonPushed = false;
                    IncrimentStatus();
                }
            }
            if (craneStatus == 5) //アーム開く
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(1);
                    sp.ForcePlay(2);
                    armController.Open();
                    await Task.Delay(1500);
                    if (craneStatus == 5)
                    {
                        lifter.Down();
                        IncrimentStatus();
                    }
                }
            }
            if (craneStatus == 6) //下降中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Play(3, 1);
                }
                if (lifter.DownFinished() && craneStatus == 6) IncrimentStatus();
            }
            if (craneStatus == 7) //アーム閉じる
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(300);
                    if (leftCatchArmpower >= 30 || rightCatchArmpower >= 30) //閉じるときのアームパワーは大きい方を採用．最低値は30f
                    {
                        if (leftCatchArmpower >= rightCatchArmpower) armController.Close(leftCatchArmpower);
                        else armController.Close(rightCatchArmpower);
                    }
                    else armController.Close(30f);
                    await Task.Delay(800);
                    IncrimentStatus();
                }
            }
            if (craneStatus == 8) //上昇中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    lifter.Up();
                    if (craneStatus < 11)
                    {
                        armController.SetMotorPower(leftCatchArmpower, 0);
                        armController.SetMotorPower(rightCatchArmpower, 1);
                    }
                }
                if (lifter.UpFinished() && craneStatus == 8) IncrimentStatus();
            }
            if (craneStatus == 9) //上昇停止
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(200);
                    if (craneStatus == 9) IncrimentStatus();
                }
            }
            if (craneStatus == 10) //帰還中
            {
                sp.Play(1);
                if (roter.CheckPos(2) && craneBox.CheckPos(6)) IncrimentStatus();
            }
            if (craneStatus == 11) //アーム開く
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(1);
                    sp.Play(2, 1);
                    armController.SetLimit(100f); // アーム開口度を100に
                    armController.Open();
                    await Task.Delay(2000);
                    if (craneStatus == 11) IncrimentStatus();
                }
            }
            if (craneStatus == 12) //アーム閉じる
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.Close(30f);
                    for (int i = 0; i < 12; i++)
                        isExecuted[i] = false;
                    await Task.Delay(1000);
                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus != 0)
        {
            if (craneStatus == -1 || craneStatus == 10)
            {
                roter.Right();
                craneBox.Forward();
            }
            else if (craneStatus == 2) roter.Left();
            else if (craneStatus == 4) craneBox.Back();
        }
    }

    protected override void DetectKey(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            int credit = creditSystem.PlayStart();
                            if (credit < 100) credit3d.text = credit.ToString();
                            else credit3d.text = "99.";
                            isExecuted[12] = false;
                        }
                        IncrimentStatus();
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed)
                    {
                        IncrimentStatus();
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        IncrimentStatus();
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed)
                    {
                        IncrimentStatus();
                        buttonPushed = false;
                    }
                    break;
            }
        }
    }

    public override void ButtonDown(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if (craneStatus == 1 && !buttonPushed)
                    {
                        buttonPushed = true;
                        IncrimentStatus();
                        creditSystem.ResetPayment();
                        int credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString();
                        else credit3d.text = "99.";
                        isExecuted[12] = false;
                    }
                    break;
                case 2:
                    if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                    {
                        buttonPushed = true;
                        IncrimentStatus();
                    }
                    break;
            }
        }
    }

    public void ButtonUp(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if (craneStatus == 2 && buttonPushed)
                    {
                        IncrimentStatus();
                        buttonPushed = false;
                    }
                    break;
                case 2:
                    if (craneStatus == 4 && buttonPushed)
                    {
                        IncrimentStatus();
                        buttonPushed = false;
                    }
                    break;
            }
        }
    }

    public override void InsertCoin()
    {
        if (!isHibernate && host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) IncrimentStatus();
        }
    }
}
