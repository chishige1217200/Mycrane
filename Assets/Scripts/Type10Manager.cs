using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type10Manager : CraneManager
{
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; //アームパワー(%，確率時)
    public int operationType = 0; //0:ボタン式，1:レバー式
    [SerializeField] int limitTimeSet = 15; //レバー式の場合，残り時間を設定
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool leverTilted = false; //trueならレバーがアクティブ
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    [SerializeField] bool autoPower = true;
    public float armPower; //現在のアームパワー
    Type3ArmController armController;
    BaseLifter ropeManager;
    Lever lever;
    Timer timer;
    ArmControllerSupport support;
    public Text limitTimedisplayed;
    TextMesh credit3d;

    async void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 10;

        // 様々なコンポーネントの取得
        //host = transform.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        //bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        lever = transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        timer = transform.Find("Timer").GetComponent<Timer>();
        temp = transform.Find("CraneUnit").transform;

        await Task.Delay(100);

        // ControlGroupの制御
        if (operationType == 0)
        {
            transform.Find("Canvas").Find("ControlGroup").Find("Button 1").gameObject.SetActive(true);
            transform.Find("Canvas").Find("ControlGroup").Find("Button 2").gameObject.SetActive(true);
            transform.Find("Floor").Find("Type10B").gameObject.SetActive(true);
            credit3d = transform.Find("Floor").Find("Type10B").Find("7Seg").GetComponent<TextMesh>();
        }
        else if (operationType == 1)
        {
            transform.Find("Canvas").Find("ControlGroup").Find("Lever Hole").gameObject.SetActive(true);
            transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").gameObject.SetActive(true);
            transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").gameObject.SetActive(true);
            transform.Find("Floor").Find("Type10L").gameObject.SetActive(true);
            credit3d = transform.Find("Floor").Find("Type10L").Find("7Seg").GetComponent<TextMesh>();
        }

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            if (operationType == 0)
                credit3d.text = "-";
            else
                credit3d.text = "--";
            creditSystem.SetHibernate();
        }

        // ロープとアームコントローラに関する処理
        ropeManager = transform.Find("RopeManager").GetComponent<BaseLifter>();
        armController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();
        support = temp.Find("ArmUnit").Find("Head").Find("Hat").GetComponent<ArmControllerSupport>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        timer.limitTime = limitTimeSet;
        timer.SetSEPlayer(sp);
        support.SetManager(this);
        support.SetLifter(ropeManager);
        creditSystem.SetCreditSound(0);

        armController.SetManager(10);
        armController.autoPower = autoPower;

        getPoint.SetManager(this);

        for (int i = 0; i < 12; i++)
            isExecuted[i] = false;

        await Task.Delay(300);

        ropeManager.Up();
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.Close();

        host.manualCode = 12 + operationType;
        craneStatus = -1;
    }

    async void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == -1)
            if ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2)) craneStatus = 0;

        if (craneStatus == 0)
        {
            if (creditSystem.Pay(0) > 0)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(100);
                    if (craneStatus == 0)
                    {
                        int credit = creditSystem.PlayStart();
                        if (operationType == 0)
                        {
                            if (credit < 10) credit3d.text = credit.ToString("");
                            else if (credit < 0) credit3d.text = "0";
                            else credit3d.text = "9.";
                        }
                        else
                        {
                            if (credit < 100) credit3d.text = credit.ToString("D2");
                            else if (credit < 0) credit3d.text = "00";
                            else credit3d.text = "99.";
                        }
                        craneStatus = 1;
                    }
                }
            }
            //コイン投入有効化;
        }
        else
        {
            if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.C) && !probability) probability = true; // テスト用隠しコマンド
            if (operationType == 0)
            {
                if (craneStatus == 1)
                {
                    //コイン投入有効化;
                    DetectKey(craneStatus);     //右移動ボタン有効化;
                }

                if (craneStatus == 2)
                { //右移動中
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
                    //コイン投入無効化;
                    //クレーン右移動;
                    //右移動効果音ループ再生;
                }

                if (craneStatus == 3)
                {
                    DetectKey(craneStatus);         //奥移動ボタン有効化;
                                                    //右移動効果音ループ再生停止;
                }

                if (craneStatus == 4)
                { //奥移動中
                    DetectKey(craneStatus);
                    if (craneBox.CheckPos(8))
                    {
                        buttonPushed = false;
                        craneStatus = 5;
                    }
                    //クレーン奥移動;
                    //奥移動効果音ループ再生;
                }
            }

            if (operationType == 1)
            {
                if (craneStatus == 3)
                {
                    if (!isExecuted[craneStatus])
                    {
                        isExecuted[craneStatus] = true;
                        timer.StartTimer();
                        creditSystem.segUpdateFlag = false;
                    }
                    DetectKey(5);
                    if (isExecuted[craneStatus] && timer.limitTimeNow <= 0) craneStatus = 5;
                }
            }

            if (craneStatus == 5)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(300);
                    timer.CancelTimer();
                    creditSystem.segUpdateFlag = true;
                    armController.Open();
                    int credit = creditSystem.Pay(0);
                    if (operationType == 0)
                    {
                        if (credit < 10) credit3d.text = credit.ToString("");
                        else credit3d.text = "9.";
                    }
                    else
                    {
                        if (credit < 100)
                        {
                            limitTimedisplayed.text = credit.ToString("D2");
                            credit3d.text = credit.ToString("D2");
                        }
                        else
                        {
                            limitTimedisplayed.text = "99";
                            credit3d.text = "99.";
                        }
                    }

                    await Task.Delay(1000);
                    ropeManager.Down();
                    if (craneStatus == 5) craneStatus = 6;
                }
                //奥移動効果音ループ再生停止;
                //アーム開く音再生;
                //アーム開く;
            }

            if (craneStatus == 6)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (operationType)
                    {
                        case 0:
                            sp.Play(3);
                            break;
                        case 1:
                            sp.Play(1);
                            break;
                    }
                    if (downTime > 0)
                    {
                        await Task.Delay(downTime);
                        if (craneStatus == 6)
                        {
                            ropeManager.DownForceStop();
                            craneStatus = 7;
                        }
                    }
                    //if (craneStatus == 6) ropeManager.Down(); //awaitによる時差実行を防止
                }
                if (ropeManager.DownFinished() && craneStatus == 6) craneStatus = 7;
                //アーム下降音再生
                //アーム下降;
            }

            if (craneStatus == 7)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (operationType)
                    {
                        case 0:
                            sp.Stop(3); //アーム下降音再生停止;
                            sp.Play(4, 1); //アーム掴む音再生;
                            break;
                        case 1:
                            sp.Stop(1);
                            sp.Play(2, 1);
                            break;
                    }
                    if (probability) armPower = armPowerConfigSuccess[0];
                    else armPower = armPowerConfig[0];
                    armController.Close();
                    armController.MotorPower(armPower);
                    await Task.Delay(1000);
                    if (craneStatus == 7) craneStatus = 8; //awaitによる時差実行を防止
                }
                //アーム掴む;
            }

            if (craneStatus == 8)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (operationType)
                    {
                        case 0:
                            sp.Stop(4);
                            sp.Play(3);
                            break;
                        case 1:
                            sp.Stop(2);
                            sp.Play(1);
                            break;
                    }
                    ropeManager.Up();
                    await Task.Delay(1500);
                    if (!probability && UnityEngine.Random.Range(0, 3) == 0 && craneStatus == 8 && support.prizeCount > 0) armController.Release(); //上昇中に離す振り分け(autoPower設定時のみ)
                }

                if (ropeManager.UpFinished() && craneStatus == 8) craneStatus = 9;
                //アーム上昇音再生;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {
                if (!armController.autoPower)
                {
                    if (probability) armPower = armPowerConfigSuccess[1];
                    else armPower = armPowerConfig[1];
                    armController.MotorPower(armPower);
                }
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    if (operationType == 0) sp.Stop(3);
                    await Task.Delay(500);
                    if (operationType == 0) sp.Play(3);
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 9 && support.prizeCount > 0) armController.Release(); // 上昇後に離す振り分け
                    if (craneStatus == 9) craneStatus = 10;
                }
                //アーム上昇停止音再生;
                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
                //アーム獲得口ポジション移動音再生;
                if (((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2)) && craneStatus == 10) craneStatus = 11;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(500);
                    armController.Open();
                    switch (operationType)
                    {
                        case 0:
                            sp.Stop(3);
                            sp.Play(4, 1);
                            break;
                        case 1:
                            sp.Stop(1);
                            sp.Play(2, 1);
                            break;
                    }
                    await Task.Delay(1500);
                    craneStatus = 12;
                }
                //アーム開く音再生;
                //アーム開く;
                //1秒待機;
            }

            if (craneStatus == 12)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.Close();

                    for (int i = 0; i < 12; i++)
                        isExecuted[i] = false;
                    await Task.Delay(1500);

                    craneStatus = 0;
                    //アーム閉じる音再生;
                    //アーム閉じる;
                }
            }

            if (!creditSystem.segUpdateFlag) // Timer表示用
            {
                if (timer.limitTimeNow >= 0)
                {
                    limitTimedisplayed.text = timer.limitTimeNow.ToString("D2");
                    credit3d.text = timer.limitTimeNow.ToString("D2");
                }
                else
                {
                    limitTimedisplayed.text = "00";
                    credit3d.text = "00";
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
                if (craneStatus == 10)
                {
                    if (!armController.autoPower)
                    {
                        if (support.prizeCount > 0)
                        {
                            if (probability && armPower > armPowerConfigSuccess[2]) armPower -= 0.3f;
                            else if (!probability && armPower > armPowerConfig[2]) armPower -= 0.3f;
                            armController.MotorPower(armPower);
                        }
                        else armController.MotorPower(100f);
                    }
                }
                if (!player2) craneBox.Left();
                else craneBox.Right();
                craneBox.Forward();
            }
            else if (craneStatus == 8)
            {
                if (!armController.autoPower)
                {
                    if (support.prizeCount > 0)
                    {
                        if (probability && armPower > armPowerConfigSuccess[1]) armPower -= 0.3f;
                        else if (!probability && armPower > armPowerConfig[1]) armPower -= 0.3f;
                        armController.MotorPower(armPower);
                    }
                    else armController.MotorPower(100f);
                }
            }
            if (operationType == 0)
            {
                if (craneStatus == 2)
                {
                    if (!player2) craneBox.Right();
                    else craneBox.Left();
                }
                if (craneStatus == 4) craneBox.Back();
            }
            else
                if (craneStatus == 1 || craneStatus == 3)
                DetectLever();
        }
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
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            isExecuted[12] = false;
                            probability = creditSystem.ProbabilityCheck();
                            Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            isExecuted[12] = false;
                            probability = creditSystem.ProbabilityCheck();
                            Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    break;
                case 5: // レバー操作時に使用
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !player2 && craneStatus == 3)
                        craneStatus = 5;
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && player2 && craneStatus == 3)
                        craneStatus = 5;
                    break;
            }
        }
    }

    public void DetectLever() // キーボード，UI共通のレバー処理
    {
        if (host.playable)
        {
            if (!player2)
            {
                if (Input.GetKey(KeyCode.H) || lever.rightFlag)
                    craneBox.Right();
                if (Input.GetKey(KeyCode.F) || lever.leftFlag)
                    craneBox.Left();
                if (Input.GetKey(KeyCode.T) || lever.backFlag)
                    craneBox.Back();
                if (Input.GetKey(KeyCode.G) || lever.forwardFlag)
                    craneBox.Forward();

                if (Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag)
                    if (craneStatus == 1)
                    {
                        craneStatus = 3;
                        creditSystem.ResetPayment();
                        isExecuted[12] = false;
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
            }
            else //2Pレバー
            {
                if (Input.GetKey(KeyCode.L) || lever.rightFlag)
                    craneBox.Right();
                if (Input.GetKey(KeyCode.J) || lever.leftFlag)
                    craneBox.Left();
                if (Input.GetKey(KeyCode.I) || lever.backFlag)
                    craneBox.Back();
                if (Input.GetKey(KeyCode.K) || lever.forwardFlag)
                    craneBox.Forward();

                if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag)
                    if (craneStatus == 1)
                    {
                        craneStatus = 3;
                        creditSystem.ResetPayment();
                        isExecuted[12] = false;
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
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
                    isExecuted[12] = false;
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
            case 3: // レバー操作用
                if (craneStatus == 3)
                    craneStatus = 5;
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
            if (operationType == 0)
            {
                if (credit < 10) credit3d.text = credit.ToString("");
                else credit3d.text = "9.";
            }
            else
            {
                if (credit < 100) credit3d.text = credit.ToString("D2");
                else credit3d.text = "99.";
            }

            //if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    public override void InsertCoinAuto()
    {
        if (!isHibernate && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (operationType == 0)
            {
                if (credit < 10) credit3d.text = credit.ToString("");
                else credit3d.text = "9.";
            }
            else
            {
                if (credit < 100) credit3d.text = credit.ToString("D2");
                else credit3d.text = "99.";
            }

            //if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
