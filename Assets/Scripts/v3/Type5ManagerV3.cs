using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type5ManagerV3 : CraneManagerV3
{
    public Type5ManagerConfig config; // nullでない場合は自動でsetup()を行う
    public bool player2 = false; // player2の場合true
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];
    [HideInInspector] public Animator[] animator = new Animator[3];
    [HideInInspector] public Type5NetworkV3 net;
    [HideInInspector] public int soundType = 0;
    [HideInInspector] public bool buttonPushed = false; // trueならボタンをクリックしているかキーボードを押下している
    [HideInInspector] public float armLPower;
    [HideInInspector] public float armRPower;
    private int alytPlayCount = 0; // 取り放題モード時のプレイ回数カウント
    private TwinArmController armController;
    private ArmControllerSupportV3 support;
    private ArmUnitLifterV3 lifter;
    private TimerV3 timer;
    private ArmNailV3[] nail = new ArmNailV3[2];

    // Start is called before the first frame update
    void Start()
    {
        Transform temp = transform.Find("CraneUnit").transform; ;

        craneStatus = -1;
        craneType = 5;
        host.manualCode = 7;
        // 様々なコンポーネントの取得
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystemV3>();
        // sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPointV3>();
        timer = transform.Find("Timer").GetComponent<TimerV3>();
        lifter = temp.Find("CraneBox").Find("Tube").Find("TubePoint").GetComponent<ArmUnitLifterV3>();
        armController = temp.Find("ArmUnit").GetComponent<TwinArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupportV3>();
        craneBox = temp.Find("CraneBox").GetComponent<CraneBoxV3>();
        creditSystem.SetSEPlayer(sp);
        getPoint.SetManager(this);
        creditSystem.SetSEPlayer(sp);
        support.SetManager(this);
        support.SetLifter(lifter);
        lifter.Up(true);

        // if (!player2)
        // {
        //     config.startPoint = new Vector2(-0.61f + config.startPoint.x, -0.31f + config.startPoint.y);
        //     config.homePoint = new Vector2(-0.61f + config.homePoint.x, -0.31f + config.homePoint.y);
        //     if (config.boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(-0.5f + 0.004525f * config.boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        // }
        // else
        // {
        //     config.startPoint = new Vector2(0.61f - config.startPoint.x, -0.31f + config.startPoint.y);
        //     config.homePoint = new Vector2(0.61f - config.homePoint.x, -0.31f + config.homePoint.y);
        //     if (config.boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(0.5f - 0.004525f * config.boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        // }
        // if (config.boxRestrictions[1] < 100) zLimit.localPosition = new Vector3(zLimit.localPosition.x, zLimit.localPosition.y, -0.19f + 0.00605f * config.boxRestrictions[1]);
        // if (config.downRestriction < 100) downLimit.localPosition = new Vector3(downLimit.localPosition.x, 1.4f - 0.005975f * config.downRestriction, downLimit.localPosition.z);
        // craneBox.GoPosition(config.startPoint);

        if (config != null)
        {
            StartCoroutine(DelayCoroutine(1000, () => { Setup(config); }));
        }
    }

    // Setup実行前に他のコンポーネントの処理を済ませること(ArmUnitLifterV3, CraneBoxV3, CreditSystemV3, ProbabilitySystemV3)
    public void Setup(Type5ManagerConfig config)
    {
        this.config = config;

        Transform temp = transform.Find("CraneUnit").transform; ;

        if (isHibernate)
        {
            creditSystem.SetHibernate();
        }

        if (!isHibernate)
        {
            if (!transform.parent.Find("LCD").gameObject.activeSelf)
                transform.parent.Find("LCD").gameObject.SetActive(true);
            preset[0].text = creditSystem.priceSets[0].ToString();
            preset[1].text = creditSystem.priceSets[1].ToString();
            preset[2].text = creditSystem.timesSets[0].ToString();
            preset[3].text = creditSystem.timesSets[1].ToString();
            if (!player2)
            {
                transform.parent.Find("LCD Component").Find("SegUnit3").gameObject.SetActive(true);
                transform.parent.Find("LCD Component").Find("SegUnit1").gameObject.SetActive(true);
            }
            else
            {
                transform.parent.Find("LCD Component").Find("SegUnit3 (1)").gameObject.SetActive(true);
                transform.parent.Find("LCD Component").Find("SegUnit1 (1)").gameObject.SetActive(true);
            }
        }

        if (!(isHibernate | creditSystem.priceSets[1] == 0 || creditSystem.timesSets[1] == 0 ||
(float)creditSystem.priceSets[0] / creditSystem.timesSets[0] < (float)creditSystem.priceSets[1] / creditSystem.timesSets[1]))
        // 正常なクレジットサービスが可能なとき
        {
            if (!player2)
                transform.parent.Find("LCD Component").Find("SegUnit2").gameObject.SetActive(true);
            else
                transform.parent.Find("LCD Component").Find("SegUnit2 (1)").gameObject.SetActive(true);
        }

        if (!config.downStop)
        {
            transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);
            transform.Find("Floor").Find("Button3").gameObject.SetActive(false);
        }

        for (int i = 0; i < 2; i++)
        {
            string a = "Arm" + (i + 1).ToString();
            GameObject arm;
            switch (config.armSize[i])
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
            nail[i] = arm.transform.Find("Nail").GetComponent<ArmNailV3>();
            if (config.armSize[i] != 0) arm.SetActive(true);
            armController.SetArm(i, config.armSize[i]);
        }

        for (int i = 0; i < 2; i++)
        {
            nail[i].SetManager(this);
            nail[i].SetLifter(lifter);
        }

        if (soundType == 0 || soundType == 1 || soundType == 2) creditSystem.SetSoundNum(0);
        else creditSystem.SetSoundNum(8);

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                getSoundNum = 7;
                break;
            case 3:
                getSoundNum = 15;
                break;
        }

        support.pushTime = config.pushTime; // 押し込みパワーの調整
        armController.SetLimit(config.armApertures);
        // craneStatus = -2;
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        // ロープ巻取り確認
        if (!lifter.CheckPos(1))
        {
            lifter.Up(true);
            while (!lifter.CheckPos(1))
            {
                yield return null;
            }
        }

        // アームユニット位置確認
        if (!player2)
        {
            craneBox.Left(true);
            craneBox.Forward(true);
            while (!craneBox.CheckPos(1))
            {
                yield return null;
            }
        }
        else if (player2)
        {
            craneBox.Right(true);
            craneBox.Forward(true);
            while (!craneBox.CheckPos(3))
            {
                yield return null;
            }
        }

        Vector3 startPos = CalcGoPosition(config.startPoint);
        // Debug.Log(startPos);
        craneBox.GoPosition(new Vector2(startPos.x, startPos.z));

        while (!craneBox.CheckPos(9))
        {
            yield return null;
        }

        lifter.GoPosition(startPos.y);

        while (!lifter.CheckPos(3))
        {
            yield return null;
        }

        craneBox.cbs.useModule = true;

        creditSystem.Setup();
        craneStatus = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus > 0)
        {
            if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.C) && !probability) probability = true; // テスト用隠しコマンド
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                //右移動ボタン有効化;
                DetectKey(craneStatus);
            }

            if (craneStatus == 2)
            { //右移動中
              //コイン投入無効化;
                DetectKey(craneStatus);
                if (!player2 && craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
                if (player2 && craneBox.CheckPos(5))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
                //右移動効果音ループ再生;
            }

            if (craneStatus == 3)
            {
                DetectKey(craneStatus);
                //右移動効果音ループ再生停止;
                //奥移動ボタン有効化;
            }

            if (craneStatus == 4)
            { //奥移動中
                DetectKey(craneStatus);
                //クレーン奥移動;
                if (craneBox.CheckPos(8))
                {
                    buttonPushed = false;
                    craneStatus = 5;
                }
                //奥移動効果音ループ再生;
            }

            if (craneStatus == 6)
            {
                DetectKey(craneStatus);
                if (lifter.CheckPos(2) && craneStatus == 6) craneStatus = 7;
                //アーム下降音再生
                //アーム下降;
            }

            if (craneStatus == 8)
            {
                //アーム上昇音再生;
                if (lifter.CheckPos(1) && craneStatus == 8) craneStatus = 9;
                //アーム上昇;
            }

            if (craneStatus == 10)
            {
                if (craneBox.CheckPos(config.prizezoneType)) craneStatus = 11;
                //アーム獲得口ポジション移動音再生;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 13)
            {
                if ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2))
                {
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            sp.Stop(6);
                            break;
                    }
                    if (craneStatus == 13) craneStatus = 14;
                }
                //アーム初期位置帰還
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus != 0)
        {
            if (craneStatus == 8)
            {
                if (probability)
                {
                    if (armLPower > config.armLPowerConfigSuccess[1]) armLPower -= 0.15f;
                    if (armRPower > config.armRPowerConfigSuccess[1]) armRPower -= 0.15f;
                }
                else
                {
                    if (armLPower > config.armLPowerConfig[1]) armLPower -= 0.15f;
                    if (armRPower > config.armRPowerConfig[1]) armRPower -= 0.15f;
                }
                armController.SetMotorPower(armLPower, 0);
                armController.SetMotorPower(armRPower, 1);
            }

            if (craneStatus == 10)
            {
                if (probability)
                {
                    if (armLPower > config.armLPowerConfigSuccess[2]) armLPower -= 0.15f;
                    if (armRPower > config.armRPowerConfigSuccess[2]) armRPower -= 0.15f;
                }
                else
                {
                    if (armLPower > config.armLPowerConfig[2]) armLPower -= 0.15f;
                    if (armRPower > config.armRPowerConfig[2]) armRPower -= 0.15f;
                }
                armController.SetMotorPower(armLPower, 0);
                armController.SetMotorPower(armRPower, 1);
            }
        }
    }

    private Vector3 CalcGoPosition(Vector3 pos) // 0, 0, 0のとき1Pは左手前端，2Pは右手前端
    {
        int armSize = 0;
        float x = 0;
        float y = 0;
        float z = 0;

        y = lifter.upLimit - (Mathf.Abs(lifter.upLimit - lifter.downLimit) / 100 * pos.y);
        z = -0.31f + 0.59f / 100 * pos.z;

        if (config.armSize[0] > armSize) armSize = config.armSize[0];
        if (config.armSize[1] > armSize) armSize = config.armSize[1];

        switch (armSize)
        {
            case 0:
            case 1:
                // -0.64 ~ -0.19
                if (!player2)
                {
                    x = (0.64f - (0.64f - 0.19f) / 100 * pos.x) * -1;
                }
                else
                {
                    x = 0.64f - (0.64f - 0.19f) / 100 * pos.x;
                }
                break;
            case 2:
                // 暫定
                if (!player2)
                {
                    x = (0.64f - (0.64f - 0.19f) / 100 * pos.x) * -1;
                }
                else
                {
                    x = 0.64f - (0.64f - 0.19f) / 100 * pos.x;
                }
                break;
            case 3:
                // 暫定
                if (!player2)
                {
                    x = (0.64f - (0.64f - 0.19f) / 100 * pos.x) * -1;
                }
                else
                {
                    x = 0.64f - (0.64f - 0.19f) / 100 * pos.x;
                }
                break;
        }

        return new Vector3(x, y, z);
    }

    public override void GetPrize()
    {
        if (net == null) // ネットワークに参加していないとき
            for (int i = 0; i < 3; i++) animator[i].SetTrigger("GetPrize");
        else
            net.CelebrateAll();
        base.GetPrize();
    }

    protected override void DetectKey(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed && !player2)
                    {
                        ButtonDown(1);
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        ButtonDown(4);
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                    {
                        ButtonUp(1);
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                    {
                        ButtonUp(4);
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                    {
                        ButtonDown(2);
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                    {
                        ButtonDown(2);
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                    {
                        ButtonUp(2);
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                    {
                        ButtonUp(2);
                    }
                    break;
                case 6:
                    if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) && !player2 && config.downStop)
                    {
                        ButtonDown(3);
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) && player2 && config.downStop)
                    {
                        ButtonDown(3);
                    }
                    break;
            }
        }
    }
    public override void ButtonDown(int num)
    {
        int credit = 0;
        switch (num)
        {
            case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
                    creditSystem.ResetPay();
                    credit = creditSystem.DecrementCredit();
                    if (credit < 100) credit3d.text = credit.ToString();
                    else credit3d.text = "99.";
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
                }
                break;
            case 2:
                if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                {
                    buttonPushed = true;
                    craneStatus = 4;
                }
                break;
            case 3:
                if (craneStatus == 6)
                {
                    lifter.Down(false);
                    craneStatus = 7;
                }
                break;
            case 4: // player2 case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
                    creditSystem.ResetPay();
                    credit = creditSystem.DecrementCredit();
                    if (credit < 100) credit3d.text = credit.ToString();
                    else credit3d.text = "99.";
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
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
            case 4: // player2 case 1:
                if (craneStatus == 2 && buttonPushed)
                {
                    craneStatus = 3;
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
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    public override void InsertCoinAuto()
    {
        if (!isHibernate && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
    protected override void FirstStatusEvent(int status)
    {
        if (status == 1)
        {

        }
        else if (status == 2)
        {
            if (!player2) craneBox.Right(true);
            else craneBox.Left(true);
            //クレーン右移動;
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(1);
                    break;
                case 3:
                    sp.Play(9);
                    break;
            }
        }
        else if (status == 3)
        {
            if (!player2) craneBox.Right(false);
            else craneBox.Left(false);
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Stop(1);
                    break;
                case 3:
                    sp.Stop(9);
                    break;
            }
        }
        else if (status == 4)
        {
            craneBox.Back(true);
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(2);
                    break;
                case 3:
                    sp.Play(10);
                    break;
            }
        }
        else if (status == 5)
        {
            craneBox.Back(false);
            //アーム開く音再生;
            //アーム開く;
            sp.Stop(1); //奥移動効果音ループ再生停止;
            armController.Open();
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Stop(2);
                    sp.Play(3, 1);
                    break;
                case 3:
                    sp.Stop(10);
                    sp.Play(11, 1);
                    break;
            }
            StartCoroutine(DelayCoroutine(1700, () =>
            {
                if (craneStatus == 5) craneStatus = 6;
            }));
        }
        else if (status == 6)
        {
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(4);
                    break;
                case 3:
                    sp.Play(12);
                    break;
            }
            lifter.Down(true);
        }
        else if (status == 7)
        {
            //アーム掴む;
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Stop(4);
                    break;
                case 3:
                    sp.Stop(12);
                    break;
            }
            StartCoroutine(DelayCoroutine(1000, () =>
            {
                switch (soundType)
                {
                    case 0:
                    case 1:
                    case 2:
                        sp.Play(5, 1);
                        break;
                    case 3:
                        sp.Play(13, 1);
                        break;
                }
                armController.Close(30f);
                StartCoroutine(DelayCoroutine(3000, () =>
                {
                    if (craneStatus == 7)
                    {
                        if (probability)
                        {
                            if (config.armLPowerConfigSuccess[0] >= 30) armLPower = config.armLPowerConfigSuccess[0];
                            else armLPower = 30f;
                            if (config.armRPowerConfigSuccess[0] >= 30) armRPower = config.armRPowerConfigSuccess[0];
                            else armRPower = 30f;
                        }
                        else
                        {
                            if (config.armLPowerConfig[0] >= 30) armLPower = config.armLPowerConfig[0];
                            else armLPower = 30f;
                            if (config.armRPowerConfig[0] >= 30) armRPower = config.armRPowerConfig[0];
                            else armRPower = 30f;
                        }
                        armController.SetMotorPower(armLPower, 0);
                        armController.SetMotorPower(armRPower, 1);
                        craneStatus = 8; //awaitによる時差実行を防止
                    }
                }));
            }));
        }
        else if (status == 8)
        {
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(4);
                    break;
                case 3:
                    sp.Play(14);
                    break;
            }
            lifter.Up(true);
        }
        else if (status == 9)
        {
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Stop(4);
                    break;
                case 3:
                    sp.Stop(14);
                    break;
            }
            StartCoroutine(DelayCoroutine(200, () =>
            {
                //アーム上昇停止音再生;
                //アーム上昇停止;
                if (craneStatus == 9)
                {
                    if (probability)
                    {
                        armLPower = config.armLPowerConfigSuccess[1];
                        armRPower = config.armRPowerConfigSuccess[1];
                    }
                    else
                    {
                        armLPower = config.armLPowerConfig[1];
                        armRPower = config.armRPowerConfig[1];
                    }
                    armController.SetMotorPower(armLPower, 0);
                    armController.SetMotorPower(armRPower, 1);
                    if (config.prizezoneType == 9)
                    {
                        Vector3 homePos = CalcGoPosition(config.homePoint);
                        // Debug.Log(homePos);
                        craneBox.GoPosition(new Vector2(homePos.x, homePos.z));
                    }
                    craneStatus = 10;
                }
            }));
        }
        else if (status == 10)
        {
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(6);
                    break;
                case 3:
                    sp.Play(9);
                    break;
            }
        }
        else if (status == 11)
        {
            //アーム開く音再生;
            //アーム開く;
            //1秒待機;
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Stop(6);
                    break;
                case 3:
                    sp.Stop(9);
                    break;
            }
            StartCoroutine(DelayCoroutine(1000, () =>
            {
                switch (soundType)
                {
                    case 0:
                    case 1:
                    case 2:
                        sp.Play(3, 1);
                        break;
                    case 3:
                        sp.Play(11, 1);
                        break;
                }
                armController.Open();
                StartCoroutine(DelayCoroutine(2500, () =>
                {
                    if (craneStatus == 11) craneStatus = 12;
                }));
            }));
        }
        else if (status == 12)
        {
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(5, 1);
                    break;
                case 3:
                    sp.Play(13, 1);
                    break;
            }
            armController.Close(30f);
            StartCoroutine(DelayCoroutine(1500, () =>
            {
                if (craneStatus == 12) craneStatus = 13;
            }));
            //アーム閉じる音再生;
            //アーム閉じる;
            //1秒待機;
        }
        else if (status == 13)
        {
            if (!player2) craneBox.Left(true);
            else craneBox.Right(true);
            craneBox.Forward(true);
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                    sp.Play(6);
                    break;
            }
        }
        else if (status == 14)
        {
            StartCoroutine(DelayCoroutine(1000, () =>
            {
                StartCoroutine(EndInit());
            }));
        }
    }
    protected override void LastStatusEvent(int status) { }

    private IEnumerator EndInit()
    {

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                sp.Play(6);
                break;
        }

        Vector3 startPos = CalcGoPosition(config.startPoint);
        // Debug.Log(startPos);
        craneBox.GoPosition(new Vector2(startPos.x, startPos.z));

        while (!craneBox.CheckPos(9))
        {
            yield return null;
        }

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                sp.Stop(6);
                break;
        }

        yield return new WaitForSeconds(1);

        lifter.GoPosition(startPos.y);

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                sp.Play(4);
                break;
        }

        while (!lifter.CheckPos(3))
        {
            yield return null;
        }

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                sp.Stop(4);
                break;
        }

        if (creditSystem.GetCreditCount() > 0)
            craneStatus = 1;
        else
            craneStatus = 0;
        //アームスタート位置へ
    }
}
