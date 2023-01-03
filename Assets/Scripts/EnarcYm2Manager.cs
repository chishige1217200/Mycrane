using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnarcYm2Manager : CraneManagerV2
{
    [SerializeField] float leftCatchArmpower = 20f; //左アームパワー
    [SerializeField] float rightCatchArmpower = 20f; //右アームパワー
    [SerializeField] float armApertures = 80f; //開口率
    [SerializeField] int[] armSize = new int[2]; // 0:なし，1:S，2:M，3:L
    TwinArmController armController;
    ArmControllerSupportV2 support;
    ArmUnitLifter lifter;
    ArmNailV2[] nail = new ArmNailV2[2];
    CraneUnitRoter roter;
    EnarcYm2Compresser cp;
    SEPlayer cbs;
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] TextMesh credit3d;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 5;
        // 様々なコンポーネントの取得
        //host = transform.root.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPointV2>();
        cp = transform.parent.Find("MainCP").GetComponent<EnarcYm2Compresser>();

        temp = transform.Find("CraneUnitBase").Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            creditSystem.SetHibernate();
            credit3d.text = "-";
        }

        // ロープとアームコントローラに関する処理
        lifter = temp.Find("CraneBox").Find("Tube").Find("TubePoint").GetComponent<ArmUnitLifter>();
        armController = temp.Find("ArmUnit").GetComponent<TwinArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupportV2>();

        for (int i = 0; i < 2; i++)
        {
            string a = "Arm" + (i + 1).ToString();
            GameObject arm;
            switch (armSize[i])
            {
                case 0:
                case 1:
                    a += "S";
                    break;
                case 2:
                    a += "M";
                    break;
                case 3:
                    a += "L";
                    break;
            }
            arm = temp.Find("ArmUnit").Find(a).gameObject;
            nail[i] = arm.transform.Find("Nail").GetComponent<ArmNailV2>();
            if (armSize[i] != 0) arm.SetActive(true);
            armController.SetArm(i, armSize[i]);
        }

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        cbs = craneBox.GetComponent<SEPlayer>();
        roter = temp.GetComponent<CraneUnitRoter>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        getPoint.SetManager(this);
        getSoundNum = 7;

        lifter.Up();

        creditSystem.SetCreditSound(0);
        creditSystem.SetSEPlayer(sp);
        support.SetManager(this);
        support.SetLifter(lifter);
        support.pushTime = 300; // 押し込みパワーの調整
        for (int i = 0; i < 2; i++)
        {
            nail[i].SetManager(this);
            nail[i].SetLifter(lifter);
        }

        StartCoroutine(DelayCoroutine(300, () =>
        {
            StartCoroutine(InternalStart());
        }));

    }

    IEnumerator InternalStart()
    {
        // イニシャル移動とinsertFlagを後に実行
        while (!lifter.UpFinished())
        {
            yield return new WaitForSeconds(0.1f);
        }
        armController.SetLimit(armApertures);

        host.manualCode = 21;
        craneStatus = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) InsertCoin();
        if (craneStatus == -1)
            if (roter.CheckPos(1)) craneStatus = 0;

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
                if (roter.CheckPos(2))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
            }
            if (craneStatus == 3)
            {
                DetectKey(craneStatus);
                sp.Stop(1);
            }
            if (craneStatus == 4)
            {
                DetectKey(craneStatus);
                sp.Play(2);
                if (craneBox.CheckPos(8))
                {
                    buttonPushed = false;
                    craneStatus = 5;
                }
            }
            if (craneStatus == 5)
            {
                sp.Stop(2);
            }
            if (craneStatus == 6) //下降中
            {
                if (lifter.DownFinished() && craneStatus == 6) craneStatus = 7;
            }
            if (craneStatus == 8)
            {
                if (lifter.UpFinished() && craneStatus == 8) craneStatus = 9;
            }
            if (craneStatus == 10)
            {
                if (roter.CheckPos(3) && craneBox.CheckPos(8)) craneStatus = 11;
            }
            if (craneStatus == 11)
            {
                sp.Stop(6);
            }
            if (craneStatus == 12)
            {
                if (roter.CheckPos(1) && craneBox.CheckPos(6)) craneStatus = 13;
            }
        }

    }

    void FixedUpdate()
    {
        if (craneStatus != 0)
        {
            if (craneStatus == -1)
            {
                roter.Left();
                craneBox.Forward();
            }
            else if (craneStatus == 2) roter.Right();
            else if (craneStatus == 4) craneBox.Back();
            else if (craneStatus == 10)
            {
                craneBox.Back();
            }
            else if (craneStatus == 12)
            {
                roter.Left();
                craneBox.Forward();
            }
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
                            if (credit < 10) credit3d.text = credit.ToString("D1");
                            else credit3d.text = "9.";
                            //probability = creditSystem.ProbabilityCheck();
                            //Debug.Log("Probability:" + probability);
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
        switch (num)
        {
            case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
                    creditSystem.ResetPayment();
                    int credit = creditSystem.PlayStart();
                    if (credit < 10) credit3d.text = credit.ToString("D1");
                    else credit3d.text = "9.";
                    //probability = creditSystem.ProbabilityCheck();
                    //Debug.Log("Probability:" + probability);
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

    public override void ButtonUp(int num)
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

    public override void InsertCoin()
    {
        if (!isHibernate && host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString("D1");
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    public override void InsertCoinAuto()
    {
        if (!isHibernate && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString("D1");
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    protected override void FirstStatusEvent(int status)
    {
        switch (status)
        {
            case 1:
                cp.DoCompressor();
                break;
            case 4:
                cbs.Play(0, 1);
                break;
            case 5:
                cbs.Stop(0);
                StartCoroutine(DelayCoroutine(300, () =>
                {
                    armController.Open();
                    sp.Play(3, 1);
                    StartCoroutine(DelayCoroutine(1000, () =>
                    {
                        if (craneStatus == 5)
                        {
                            lifter.Down();
                            cbs.Play(0, 1);
                            sp.Play(4);
                            craneStatus = 6;
                        }
                    }));
                }));
                break;
            case 7:
                cbs.Stop(0);
                sp.Stop(4);
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    sp.Play(5, 1);
                    if (leftCatchArmpower >= 30 || rightCatchArmpower >= 30) //閉じるときのアームパワーは大きい方を採用．最低値は30f
                    {
                        if (leftCatchArmpower >= rightCatchArmpower) armController.Close(leftCatchArmpower);
                        else armController.Close(rightCatchArmpower);
                    }
                    else armController.Close(30f);

                    StartCoroutine(DelayCoroutine(3000, () =>
                    {
                        craneStatus = 8;
                    }));
                }));
                break;
            case 8:
                cbs.Play(0, 1);
                sp.Play(6);
                lifter.Up();
                if (craneStatus < 11)
                {
                    armController.SetMotorPower(leftCatchArmpower, 0);
                    armController.SetMotorPower(rightCatchArmpower, 1);
                }
                break;
            case 9:
                cbs.Stop(0);
                roter.RotateToTarget(45f);
                craneStatus = 10;
                break;
            case 10:
                cbs.Play(0, 1);
                StartCoroutine(CraneBoxStopSound());
                break;
            case 11:
                StartCoroutine(DelayCoroutine(300, () =>
                {
                    sp.Play(3, 1);
                    armController.SetLimit(100f); // アーム開口度を100に
                    armController.Open();
                    StartCoroutine(DelayCoroutine(2000, () =>
                    {
                        craneStatus = 12;
                    }));
                }));

                break;
            case 12:
                armController.Close(30f);
                cbs.Play(0, 1);
                break;
            case 13:
                cbs.Stop(0);
                cbs.Play(1, 1);
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                }));
                break;
        }
    }

    protected override void LastStatusEvent(int status)
    {
    }

    private IEnumerator CraneBoxStopSound()
    {
        while (!craneBox.CheckPos(6) && !craneBox.CheckPos(8)) yield return null;
        cbs.Stop(0);
        cbs.Play(1, 1);
    }
}
