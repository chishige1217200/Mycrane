﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type6Manager : MonoBehaviour
{
    public int craneStatus = -2; //-2:初期化動作，0:待機状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; //アームパワー(%，確率時)
    [SerializeField] float armApertures = 80f; //開口率
    [SerializeField] float[] boxRestrictions = new float[2];
    [SerializeField] float downRestriction = 100f;
    [SerializeField] int limitTimeSet = 30; //残り時間を設定
    public int soundType = 0; //SEの切り替え 0,1: CATCHER 9 Selecterで指定すること
    bool timerFlag = false; //タイマーの起動は1プレイにつき1度のみ実行
    bool[] isExecuted = new bool[11]; //各craneStatusで1度しか実行しない処理の管理
    public bool leverTilted = false; //trueならレバーがアクティブ
    public bool probability; //確率判定用
    public float armPower;
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool downStop = true; //下降停止の使用可否
    [SerializeField] bool openEnd = true; //終了時にアームは開きっぱなしになるか
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    public SEPlayer _SEPlayer;
    Type6ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[3];
    Lever lever;
    Timer timer;
    MachineHost host;
    GameObject canvas;
    public Text limitTimedisplayed;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];
    public Animator[] animator = new Animator[3];

    async void Start()
    {
        Transform temp;
        Transform xLimit = this.transform.Find("Floor").Find("XLimit");
        Transform zLimit = this.transform.Find("Floor").Find("ZLimit");
        Transform downLimit = this.transform.Find("Floor").Find("DownLimit");
        // 様々なコンポーネントの取得
        host = this.transform.root.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        //_SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever").GetComponent<Lever>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        timer = this.transform.Find("Timer").GetComponent<Timer>();
        temp = this.transform.Find("CraneUnit").transform;

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
        ropeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type6ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        nail[0] = temp.Find("ArmUnit").Find("Arm1").Find("Nail1").GetComponent<ArmNail>();
        nail[1] = temp.Find("ArmUnit").Find("Arm2").Find("Nail2").GetComponent<ArmNail>();
        nail[2] = temp.Find("ArmUnit").Find("Arm3").Find("Nail3").GetComponent<ArmNail>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.GetSEPlayer(_SEPlayer);
        timer.limitTime = limitTimeSet;
        timer.GetSEPlayer(_SEPlayer);
        timer.SetAlertSound(-1);
        getPoint.GetManager(6);
        ropeManager.ArmUnitUp();
        await Task.Delay(500);
        creditSystem.SetCreditSound(0);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(6);
        support.GetRopeManager(ropeManager);
        support.pushTime = 300; // 押し込みパワーの調整
        for (int i = 0; i < 3; i++)
        {
            nail[i].GetManager(6);
            nail[i].GetRopeManager(ropeManager);
        }

        for (int i = 0; i < 11; i++)
            isExecuted[i] = false;


        // イニシャル移動とinsertFlagを後に実行
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.ArmLimit(armApertures);
        if (openEnd) armController.ArmOpen();
        else armController.ArmClose(100f);

        if (!player2)
        {
            if (boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(-0.5f + 0.004525f * boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        }
        else
        {
            if (boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(0.5f - 0.004525f * boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        }
        if (boxRestrictions[1] < 100) zLimit.localPosition = new Vector3(zLimit.localPosition.x, zLimit.localPosition.y, -0.19f + 0.00605f * boxRestrictions[1]);
        if (downRestriction < 100) downLimit.localPosition = new Vector3(downLimit.localPosition.x, 1.4f - 0.005975f * downRestriction, downLimit.localPosition.z);

        craneStatus = -1;
    }

    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == -1 && ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2)))
        {
            craneStatus = 0;
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
        }
        else
        {
            if (craneStatus == 1) // 操作待ち
            {
                if (!player2)
                {
                    if ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                    || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)
                    {
                        leverTilted = true;
                        _SEPlayer.PlaySE(1, 2147483647);
                    }
                    if ((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                    && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted)
                    {
                        leverTilted = false;
                        _SEPlayer.StopSE(1);
                    }
                }
                else
                {
                    if ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                    || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)
                    {
                        leverTilted = true;
                        _SEPlayer.PlaySE(1, 2147483647);
                    }
                    if ((!Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K)
                    && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted)
                    {
                        leverTilted = false;
                        _SEPlayer.StopSE(1);
                    }
                }
            }

            if (craneStatus == 2) // 1度でも移動したことがある
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    timerFlag = true;
                    timer.StartTimer();
                    creditSystem.segUpdateFlag = false;
                }
                // タイマーの起動処理を書く
                if (!player2)
                {
                    if ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                    || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)
                    {
                        leverTilted = true;
                        _SEPlayer.PlaySE(1, 2147483647);
                    }
                    if ((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                    && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted)
                    {
                        leverTilted = false;
                        _SEPlayer.StopSE(1);
                    }
                }
                else
                {
                    if ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                    || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)
                    {
                        leverTilted = true;
                        _SEPlayer.PlaySE(1, 2147483647);
                    }
                    if ((!Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K)
                    && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted)
                    {
                        leverTilted = false;
                        _SEPlayer.StopSE(1);
                    }
                }
                if (isExecuted[craneStatus] && timer.limitTimeNow <= 0) craneStatus = 3; // 時間切れになったら下降
                if (!leverTilted) InputKeyCheck(craneStatus); // 下降開始ボタン有効化
            }

            if (craneStatus == 3) // アーム開く
            {
                _SEPlayer.StopSE(1);
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(500);
                    if (craneStatus == 3)
                    {
                        creditSystem.segUpdateFlag = true;
                        timer.CancelTimer();
                        int credit = creditSystem.Pay(0);
                        if (credit < 100)
                        {
                            limitTimedisplayed.text = credit.ToString();
                            credit3d.text = credit.ToString();
                        }
                        else
                        {
                            limitTimedisplayed.text = "99";
                            credit3d.text = "99.";
                        }
                        if (!openEnd)
                        {
                            armController.ArmOpen();
                            _SEPlayer.PlaySE(3, 1);
                            await Task.Delay(1200);
                        }
                        if (craneStatus == 3) craneStatus = 4;
                    }
                }
            }

            if (craneStatus == 4) // アーム下降中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _SEPlayer.PlaySE(4, 2147483647);
                    if (craneStatus == 4) ropeManager.ArmUnitDown(); //awaitによる時差実行を防止
                }
                if (downStop) InputKeyCheck(craneStatus); // 下降停止ボタン有効化
                if (ropeManager.DownFinished() && craneStatus == 4) craneStatus = 5;
            }

            if (craneStatus == 5) // アーム閉じる
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _SEPlayer.StopSE(4);
                    //アーム下降音再生停止;
                    await Task.Delay(1000);
                    //アーム掴む音再生;
                    _SEPlayer.PlaySE(5, 1);

                    armController.ArmClose(30f);

                    await Task.Delay(3000);
                    if (craneStatus == 5)
                    {
                        if (probability)
                        {
                            if (armPowerConfigSuccess[0] >= 30) armPower = armPowerConfigSuccess[0];
                            else armPower = 30f;
                        }
                        else
                        {
                            if (armPowerConfig[0] >= 30) armPower = armPowerConfig[0];
                            else armPower = 30f;
                        }
                        armController.MotorPower(armPower);
                        craneStatus = 6; //awaitによる時差実行を防止
                    }
                }
            }

            if (craneStatus == 6) // アーム上昇中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _SEPlayer.PlaySE(4, 2147483647);

                    ropeManager.ArmUnitUp();
                }

                if (probability)
                {
                    if (armPower > armPowerConfigSuccess[1]) armPower -= 0.1f;
                }
                else
                {
                    if (armPower > armPowerConfig[1]) armPower -= 0.1f;
                }
                armController.MotorPower(armPower);

                if (ropeManager.UpFinished() && craneStatus == 6) craneStatus = 7;
            }


            if (craneStatus == 7) // アーム上昇停止
            {
                if (probability)
                {
                    armPower = armPowerConfigSuccess[1];
                }
                else
                {
                    armPower = armPowerConfig[1];
                }
                armController.MotorPower(armPower);
                craneStatus = 8;
            }

            if (craneStatus == 8) // 帰還中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;

                    _SEPlayer.StopSE(4);
                    _SEPlayer.PlaySE(6, 2147483647);
                }

                if (probability)
                {
                    if (armPower > armPowerConfigSuccess[2]) armPower -= 0.1f;
                }
                else
                {
                    if (armPower > armPowerConfig[2]) armPower -= 0.1f;
                }
                armController.MotorPower(armPower);

                if ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2)) craneStatus = 9;
            }

            if (craneStatus == 9) // アーム開く
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;

                    _SEPlayer.StopSE(6);
                    _SEPlayer.PlaySE(3, 1);
                    armController.ArmLimit(100f); // アーム開口度を100に
                    armController.ArmOpen();
                    await Task.Delay(2500);
                    if (craneStatus == 9) craneStatus = 10;
                }
            }

            if (craneStatus == 10) // アーム閉じる
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    if (!openEnd)
                    {
                        _SEPlayer.PlaySE(5, 1);
                        armController.ArmClose(100f);
                        await Task.Delay(2500);
                    }
                    timerFlag = false;
                    for (int i = 0; i < 10; i++)
                        isExecuted[i] = false;
                    armController.ArmLimit(armApertures); //アーム開口度リセット

                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
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
        if (craneStatus == 0) ;
        else
        {
            if (craneStatus == -1 || craneStatus == 8)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
                craneBox.Forward();
            }
            else if (craneStatus == 1 || craneStatus == 2)
                InputLeverCheck();
        }
    }

    public void GetPrize()
    {
        int getSoundNum = 7;

        for (int i = 0; i < 3; i++) animator[i].SetTrigger("GetPrize");

        switch (creditSystem.probabilityMode)
        {
            case 2:
            case 3:
                creditSystem.ResetCreditProbability();
                break;
            case 4:
            case 5:
                creditSystem.ResetCostProbability();
                break;
        }

        if (!_SEPlayer._AudioSource[getSoundNum].isPlaying)
        {
            if (getSoundNum != -1)
                _SEPlayer.PlaySE(getSoundNum, 1);
        }
    }

    public void InputKeyCheck(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 2:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !player2)
                        craneStatus = 3;
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && player2)
                        craneStatus = 3;
                    break;
                case 4:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !player2 && downStop)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 5;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && player2 && downStop)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 5;
                    }
                    break;
            }
        }
    }

    public void InputLeverCheck() // キーボード，UI共通のレバー処理
    {
        if (host.playable)
        {
            int credit = 0;
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
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString();
                        else credit3d.text = "99.";
                        isExecuted[10] = false;
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
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString();
                        else credit3d.text = "99.";
                        isExecuted[10] = false;
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
            }
        }
    }

    public void ButtonDown(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 2:
                    if (craneStatus == 2)
                        craneStatus = 3;
                    break;
                case 4:
                    if (downStop && craneStatus == 4)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 5;
                    }
                    break;
            }
        }
    }

    public void InsertCoin()
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
