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
    RopeManager ropeManager; //暫定
    //ArmUnitLifter lifter;
    CraneUnitRoter roter;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];

    async void Start()
    {
        Transform temp;

        craneStatus = -2;

        // 様々なコンポーネントの取得
        host = transform.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _SEPlayer = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        preset[0].text = priceSet[0].ToString();
        preset[1].text = priceSet[1].ToString();
        preset[2].text = timesSet[0].ToString();
        preset[3].text = timesSet[1].ToString();

        // ロープとアームコントローラに関する処理
        ropeManager = transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type9ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        roter = temp.GetComponent<CraneUnitRoter>();

        // ロープにマネージャー情報をセット
        creditSystem.GetSEPlayer(_SEPlayer);

        creditSystem.SetCreditSound(0);
        getSoundNum = 4;

        getPoint.GetManager(-1); // テスト中

        await Task.Delay(300);
        ropeManager.ArmUnitUp();
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.Limit(armApertures);

        for (int i = 0; i < 13; i++)
            isExecuted[i] = false;

        craneStatus = -1;
    }

    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();

        if (craneStatus == -1)
            if (roter.CheckPos(2)) IncrimentStatus();
        if (craneStatus == 0) ;
        else
        {
            if (craneStatus == 1)
            {
                DetectKey(craneStatus);
            }
            if (craneStatus == 2)
            {
                DetectKey(craneStatus);
                _SEPlayer.Play(1);
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
                    _SEPlayer.PlaySE(2, 1);
                }
                DetectKey(craneStatus);
                _SEPlayer.StopSE(1);
            }
            if (craneStatus == 4)
            {
                DetectKey(craneStatus);
                _SEPlayer.Play(1);
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
                    _SEPlayer.StopSE(1);
                    _SEPlayer.ForcePlaySE(2);
                    armController.Open();
                    await Task.Delay(1500);
                    if (craneStatus == 5)
                    {
                        ropeManager.ArmUnitDown();
                        IncrimentStatus();
                    }
                }
            }
            if (craneStatus == 6) //下降中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _SEPlayer.PlaySE(3, 1);
                }
                if (ropeManager.DownFinished() && craneStatus == 6) IncrimentStatus();
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
                    await Task.Delay(700);
                    IncrimentStatus();
                }
            }
            if (craneStatus == 8) //上昇中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    ropeManager.ArmUnitUp();
                    if (craneStatus < 11)
                    {
                        armController.Power(leftCatchArmpower, 0);
                        armController.Power(rightCatchArmpower, 1);
                    }
                }
                if (ropeManager.UpFinished() && craneStatus == 8) IncrimentStatus();
            }
            if (craneStatus == 9) //上昇停止
            {
                IncrimentStatus();
            }
            if (craneStatus == 10) //帰還中
            {
                _SEPlayer.Play(1);
                if (roter.CheckPos(2) && craneBox.CheckPos(6)) IncrimentStatus();
            }
            if (craneStatus == 11) //アーム開く
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    armController.Limit(100f); // アーム開口度を100に
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
        if (craneStatus == 0) ;
        else
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
                        craneStatus = 2;
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed)
                    {
                        craneStatus = 5;
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
                        craneStatus = 2;
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
                        craneStatus = 4;
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
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 2:
                    if (craneStatus == 4 && buttonPushed)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    break;
            }
        }
    }

    public override void InsertCoin()
    {
        if (host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
