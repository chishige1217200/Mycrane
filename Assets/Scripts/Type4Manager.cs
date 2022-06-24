using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type4Manager : CraneManager
{
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] float leftCatchArmpower = 20f; //左アームパワー
    [SerializeField] float rightCatchArmpower = 20f; //右アームパワー
    [SerializeField] float armApertures = 80f; //開口率
    [SerializeField] float[] boxRestrictions = new float[2]; //横・縦の順で移動制限設定
    [SerializeField] float downRestriction = 100f;
    [SerializeField] int operationType = 1; //0:ボタン式，1:レバー式
    [SerializeField] int catchLong = 2000; //キャッチに要する時間(m秒)
    [SerializeField] int catchTiming = 2000; //キャッチが始まるまでの時間(m秒)
    [SerializeField] int backTime = 1000; //戻り動作が始まるまでの時間(m秒)
    bool[] isExecuted = new bool[16]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool leverTilted = false; //trueならレバーがアクティブ
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool rotation = true; //回転機能の使用可否
    [SerializeField] bool downStop = true; //下降停止機能の使用可否
    [SerializeField] int[] armSize = new int[2]; // 0:なし，1・2:M，3:L
    TwinArmController armController;
    ArmUnitLifter lifter;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];
    Lever lever;
    Type4VideoManager videoManager;
    Type4ArmunitRoter roter;
    KeyCode downButtonAlpha;
    KeyCode downButtonNumpad;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];

    async void Start()
    {
        Transform temp;
        Transform xLimit = transform.Find("Floor").Find("XLimit");
        Transform zLimit = transform.Find("Floor").Find("ZLimit");
        Transform downLimit = transform.Find("Floor").Find("DownLimit");

        craneStatus = -2;
        craneType = 4;
        // 様々なコンポーネントの取得
        //host = transform.root.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        lever = transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            credit3d.text = "-";
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
        armController = temp.Find("ArmUnit").GetComponent<TwinArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        videoManager = transform.Find("VideoPlay").GetComponent<Type4VideoManager>();
        roter = temp.Find("ArmUnit").Find("Main").GetComponent<Type4ArmunitRoter>();

        for (int i = 0; i < 2; i++)
        {
            if (armSize[i] == 1) armSize[i] = 2;
            string a = "Arm" + (i + 1).ToString();
            GameObject arm;
            switch (armSize[i])
            {
                case 1:
                    a += "S";
                    break;
                case 0:
                case 2:
                    a += "M";
                    break;
                case 3:
                    a += "L";
                    break;
            }
            arm = temp.Find("ArmUnit").Find(a).gameObject;
            nail[i] = arm.transform.Find("Nail").GetComponent<ArmNail>();
            if (armSize[i] != 0) arm.SetActive(true);
            armController.SetArm(i, armSize[i]);
        }

        await Task.Delay(500);

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        getPoint.SetManager(this);
        getSoundNum = 6;
        lifter.Up();
        creditSystem.SetCreditSound(0);
        creditSystem.SetSEPlayer(sp);
        support.SetManager(this);
        support.SetLifter(lifter);
        roter.SetSEPlayer(sp);
        support.pushTime = 300; // 押し込みパワーの調整
        for (int i = 0; i < 2; i++)
        {
            nail[i].SetManager(this);
            nail[i].SetLifter(lifter);
        }

        for (int i = 0; i < 15; i++)
            isExecuted[i] = false;

        isExecuted[15] = true; //初回処理用（レバーを倒しっぱなしでプレイし始めるときの対策）

        // ControlGroupの制御
        if (operationType == 0)
        {
            transform.Find("Canvas").Find("ControlGroup").Find("Lever Hole").gameObject.SetActive(false);
            transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").gameObject.SetActive(false);
            transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").gameObject.SetActive(false);
            transform.Find("Floor").Find("ButtonUnit").gameObject.SetActive(true);
            if (!player2)
            {
                downButtonAlpha = KeyCode.Alpha3;
                downButtonNumpad = KeyCode.Keypad3;
            }
            if (player2)
            {
                downButtonAlpha = KeyCode.Alpha9;
                downButtonNumpad = KeyCode.Keypad9;
            }
        }
        else if (operationType == 1)
        {
            transform.Find("Canvas").Find("ControlGroup").Find("Button 1").gameObject.SetActive(false);
            transform.Find("Canvas").Find("ControlGroup").Find("Button 2").gameObject.SetActive(false);
            transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);
            transform.Find("Floor").Find("LeverUnit").gameObject.SetActive(true);
            if (!player2)
            {
                downButtonAlpha = KeyCode.Alpha2;
                downButtonNumpad = KeyCode.Keypad2;
            }
            if (player2)
            {
                downButtonAlpha = KeyCode.Alpha8;
                downButtonNumpad = KeyCode.Keypad8;
            }
        }
        if (boxRestrictions[0] < 100)
        {
            if (!player2) xLimit.localPosition = new Vector3(-0.52f + 0.0041f * boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
            else xLimit.localPosition = new Vector3(0.52f - 0.0041f * boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        }
        if (boxRestrictions[1] < 100) zLimit.localPosition = new Vector3(zLimit.localPosition.x, zLimit.localPosition.y, -0.13f + 0.0048f * boxRestrictions[1]);
        if (downRestriction < 100) downLimit.localPosition = new Vector3(downLimit.localPosition.x, 1.37f - 0.0051f * downRestriction, downLimit.localPosition.z);

        // イニシャル移動とinsertFlagを後に実行
        while (!lifter.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.SetLimit(armApertures);
        armController.Close(30f);

        craneStatus = -1;
    }

    // Update is called once per frame
    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == -1)
        {
            if (craneBox.CheckPos(1) && !player2) craneStatus = 0;
            if (craneBox.CheckPos(3) && player2) craneStatus = 0;
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            videoManager.randomMode = true;
        }
        else
        {
            if (operationType == 0)
            {
                if (craneStatus == 1)
                {
                    if (!isExecuted[craneStatus])
                    {
                        isExecuted[craneStatus] = true;
                        if (Random.Range(0, 2) == 0 && !player2) videoManager.Play(4);
                        else videoManager.Play(0);
                    }
                    DetectKey(craneStatus);         //右移動ボタン有効化;
                }

                if (craneStatus == 2)
                { //右移動中
                    DetectKey(craneStatus);
                    if (!player2 && craneBox.CheckPos(7))
                    {
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                        craneStatus = 3;
                    }
                    if (player2 && craneBox.CheckPos(5))
                    {
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                        craneStatus = 3;
                    }
                    //クレーン右移動;
                }

                if (craneStatus == 3)
                {
                    DetectKey(craneStatus);         //奥移動ボタン有効化;
                }

                if (craneStatus == 4)
                { //奥移動中
                    DetectKey(craneStatus);
                    if (craneBox.CheckPos(8))
                    {
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                        craneStatus = 5;
                    }
                    //クレーン奥移動;
                }
            }

            if (operationType == 1)
            {
                if (craneStatus == 1)
                {
                    if (!isExecuted[craneStatus])
                    {
                        isExecuted[craneStatus] = true;
                        videoManager.Play(0);
                    }
                    if (!player2)
                    {
                        if (isExecuted[15] && ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)) // 初回用の処理
                        {
                            leverTilted = true;
                            isExecuted[15] = false;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }

                        if ((Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.G)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)
                        {
                            leverTilted = true;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }
                        if (((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                        && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted && host.playable) || (leverTilted && !host.playable))
                        {
                            leverTilted = false;
                            videoManager.Play(0);
                            sp.Stop(1);
                            sp.Play(2, 1);
                        }
                    }
                    else
                    {
                        if (isExecuted[15] && ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)) // 初回用の処理
                        {
                            leverTilted = true;
                            isExecuted[15] = false;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }


                        if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.K)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)
                        {
                            leverTilted = true;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }
                        if (((!Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K)
                        && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted && host.playable) || (leverTilted && !host.playable))
                        {
                            leverTilted = false;
                            videoManager.Play(0);
                            sp.Stop(1);
                            sp.Play(2, 1);
                        }
                    }
                }
                if (craneStatus == 3)
                {
                    if (!player2)
                    {
                        if (isExecuted[15] && ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)) // 初回用の処理
                        {
                            leverTilted = true;
                            isExecuted[15] = false;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }

                        if ((Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.G)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)
                        {
                            leverTilted = true;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }
                        if (((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                        && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted && host.playable) || (leverTilted && !host.playable))
                        {
                            leverTilted = false;
                            videoManager.Play(0);
                            sp.Stop(1);
                            sp.Play(2, 1);
                        }
                    }
                    else
                    {
                        if (isExecuted[15] && ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)) // 初回用の処理
                        {
                            leverTilted = true;
                            isExecuted[15] = false;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }


                        if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.K)
                        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)
                        {
                            leverTilted = true;
                            videoManager.Play(1);
                            sp.Stop(2);
                            sp.Play(1, 1);
                        }
                        if (((!Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K)
                        && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted && host.playable) || (leverTilted && !host.playable))
                        {
                            leverTilted = false;
                            videoManager.Play(0);
                            sp.Stop(1);
                            sp.Play(2, 1);
                        }

                    }
                    if (!leverTilted) DetectKey(5);
                }
            }

            if (craneStatus == 5) DetectKey(craneStatus);
            if (craneStatus == 6)
            {
                await Task.Delay(10);
                if (craneStatus == 6) DetectKey(craneStatus);
            }
            if (craneStatus == 7)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.Open();
                    await Task.Delay(1000);
                    if (craneStatus == 7) craneStatus = 8;
                }
                //アーム開く音再生;
                //アーム開く;
            }
            if (craneStatus == 8)
            {   //アーム下降中
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(3);
                    sp.Stop(4);
                    sp.Play(5, 1);
                    if (craneStatus == 8) lifter.Down(); //awaitによる時差実行を防止
                }
                DetectKey(craneStatus);
                if (lifter.DownFinished() && craneStatus == 8) craneStatus = 9;
            }
            if (craneStatus == 9)
            {   //アーム掴む
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    if (catchTiming > 0) await Task.Delay(catchTiming);
                    if (leftCatchArmpower >= 30 || rightCatchArmpower >= 30) //閉じるときのアームパワーは大きい方を採用．最低値は30f
                    {
                        if (leftCatchArmpower >= rightCatchArmpower) armController.Close(leftCatchArmpower);
                        else armController.Close(rightCatchArmpower);
                    }
                    else armController.Close(30f);
                    if (catchLong > 0) await Task.Delay(catchLong);
                    if (craneStatus == 9) craneStatus = 10;
                }
            }
            if (craneStatus == 10)
            {   //アーム上昇中
                {
                    if (!isExecuted[craneStatus])
                    {
                        isExecuted[craneStatus] = true;
                        lifter.Up();
                        await Task.Delay(1000);
                        if (craneStatus < 13)
                        {
                            armController.SetMotorPower(leftCatchArmpower, 0);
                            armController.SetMotorPower(rightCatchArmpower, 1);
                        }
                    }
                    if (lifter.UpFinished() && craneStatus == 10) craneStatus = 11;
                }
            }
            if (craneStatus == 11)
            {   //アーム上昇停止
                if (!isExecuted[craneStatus])
                {
                    if (backTime > 0) await Task.Delay(backTime);
                    craneStatus = 12;
                }
            }
            if (craneStatus == 12)
            {
                if (craneBox.CheckPos(1) && !player2) craneStatus = 13;
                if (craneBox.CheckPos(3) && player2) craneStatus = 13;
            }
            if (craneStatus == 13)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    videoManager.Play(5);
                    armController.SetLimit(100f); // アーム開口度を100に
                    armController.Open();
                    await Task.Delay(2000);
                    if (craneStatus == 13) craneStatus = 14;
                    //アーム開く音再生;
                    //アーム開く;
                    //1秒待機;
                }
            }
            if (craneStatus == 14)
            {
                if (!isExecuted[craneStatus])
                {
                    armController.Close(30f);
                    await Task.Delay(1000);
                    if (craneStatus == 14) craneStatus = 15;
                }
                //アーム閉じる音再生;
                //アーム閉じる;
                //1秒待機;
            }
            if (craneStatus == 15)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    for (int i = 0; i < 14; i++)
                        isExecuted[i] = false;
                    armController.SetLimit(armApertures); //アーム開口度リセット
                    if (!sp.audioSource[6].isPlaying) sp.Play(7, 1);

                    creditSystem.ResetPayment();
                    int credit = creditSystem.PlayStart();
                    if (credit < 10) credit3d.text = credit.ToString();
                    else credit3d.text = "9.";

                    roter.RotateToHome();
                    await Task.Delay(5000);
                    creditSystem.ResetPayment();
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
            if (craneStatus == -1 || craneStatus == 12)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
                craneBox.Forward();
            }
            if (operationType == 0)
            {
                if (craneStatus == 2)
                {
                    if (!player2) craneBox.Right();
                    else craneBox.Left();
                }
                else if (craneStatus == 4) craneBox.Back();
            }
            else
                if (craneStatus == 1 || craneStatus == 3)
                DetectLever();
        }
    }

    public override void GetPrize()
    {
        sp.Stop(7);
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
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();

                            videoManager.Play(1);
                            isExecuted[15] = false;
                        }
                        craneStatus = 2;
                        sp.Play(1, 1);
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();

                            videoManager.Play(1);
                            isExecuted[15] = false;
                        }
                        craneStatus = 2;
                        sp.Play(1, 1);
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                    {
                        craneStatus = 3;
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                    {
                        craneStatus = 3;
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        sp.ForcePlay(1);
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        sp.ForcePlay(1);
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                    {
                        craneStatus = 5;
                        sp.Stop(1);
                        sp.ForcePlay(2);
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                    {
                        craneStatus = 5;
                        sp.Stop(1);
                        sp.ForcePlay(2);
                        buttonPushed = false;
                    }
                    break;
                case 5:
                    if ((Input.GetKeyDown(downButtonNumpad) || Input.GetKeyDown(downButtonAlpha)) && !buttonPushed)
                    {
                        if (rotation)
                        {
                            craneStatus = 6;
                            roter.RotateStart();
                            videoManager.Play(2);
                        }
                        else
                        {
                            craneStatus = 7;
                            videoManager.Play(3);
                        }
                    }

                    break;
                case 6:
                    if ((Input.GetKeyDown(downButtonNumpad) || Input.GetKeyDown(downButtonAlpha)) && !buttonPushed)
                    {
                        craneStatus = 7;
                        roter.RotateStop();
                        videoManager.Play(3);
                    }
                    break;
                case 8:
                    if ((Input.GetKeyDown(downButtonNumpad) || Input.GetKeyDown(downButtonAlpha)) && downStop)
                    {
                        if (downStop)
                        {
                            lifter.DownForceStop();
                            craneStatus = 9;
                        }
                    }
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
                        isExecuted[15] = false;
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
                        isExecuted[15] = false;
                    }
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
                        videoManager.Play(1);
                        sp.Play(1, 1);
                        isExecuted[15] = false;
                    }
                    if (craneStatus == 2 && buttonPushed)
                        sp.Play(1, 1);

                    break;
                case 2:
                    if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        sp.Play(1, 1);
                    }
                    break;
                case 3:
                    if ((craneStatus == 5 && operationType == 0) || (craneStatus == 3 && operationType == 1 && !leverTilted))
                    {
                        if (rotation)
                        {
                            craneStatus = 6;
                            roter.RotateStart();
                            videoManager.Play(2);
                        }
                        else
                        {
                            craneStatus = 7;
                            videoManager.Play(3);
                        }
                    }
                    else if (craneStatus == 6)
                    {
                        craneStatus = 7;
                        roter.RotateStop();
                        videoManager.Play(3);
                    }
                    else if (craneStatus == 8)
                    {
                        if (downStop)
                        {
                            lifter.DownForceStop();
                            craneStatus = 9;
                        }
                    }
                    break;
                case 4: // player2 case 1:
                    if (craneStatus == 1 && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        videoManager.Play(1);
                        sp.Play(1, 1);
                        isExecuted[15] = false;
                    }
                    if (craneStatus == 2 && buttonPushed)
                        sp.Play(1, 1);
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
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                    }
                    break;
                case 2:
                    if (craneStatus == 4 && buttonPushed)
                    {
                        craneStatus = 5;
                        sp.Stop(1);
                        sp.Play(2, 1);
                        buttonPushed = false;
                    }
                    break;
                case 4: // player2 case 1:
                    if (craneStatus == 2 && buttonPushed)
                    {
                        craneStatus = 3;
                        sp.Stop(1);
                        sp.Play(2, 1);
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
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0)
            {
                craneStatus = 1;
                videoManager.randomMode = false;
            }
        }
    }
}
