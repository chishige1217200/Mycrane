﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type4Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] float leftCatchArmpower = 20f; //左アームパワー
    [SerializeField] float rightCatchArmpower = 20f; //右アームパワー
    [SerializeField] float armApertures = 80f; //開口率
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
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    SEPlayer _SEPlayer;
    Type1ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];
    Lever lever;
    VideoPlay videoPlay;
    Type4ArmunitRoter roter;
    KeyCode downButtonAlpha;
    KeyCode downButtonNumpad;
    MachineHost host;
    GameObject canvas;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];

    async void Start()
    {
        Transform temp;
        // 様々なコンポーネントの取得
        host = this.transform.root.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
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
        armController = temp.Find("ArmUnit").GetComponent<Type1ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        nail[0] = temp.Find("ArmUnit").Find("Arm1").Find("Nail1").GetComponent<ArmNail>();
        nail[1] = temp.Find("ArmUnit").Find("Arm2").Find("Nail2").GetComponent<ArmNail>();
        videoPlay = this.transform.Find("VideoPlay").GetComponent<VideoPlay>();
        roter = temp.Find("ArmUnit").Find("Main").GetComponent<Type4ArmunitRoter>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        //craneBox.GetManager(4);

        // ロープにマネージャー情報をセット
        ropeManager.SetManagerToPoint(4);
        creditSystem.GetSEPlayer(_SEPlayer);
        getPoint.GetManager(4);
        ropeManager.ArmUnitUp();
        creditSystem.SetCreditSound(0);
        creditSystem.GetSEPlayer(_SEPlayer);
        //support.GetManager(4);
        support.GetRopeManager(ropeManager);
        roter.GetSEPlayer(_SEPlayer);
        support.pushTime = 300; // 押し込みパワーの調整
        for (int i = 0; i < 2; i++)
        {
            nail[i].GetManager(4);
            nail[i].GetRopeManager(ropeManager);
        }

        for (int i = 0; i < 15; i++)
            isExecuted[i] = false;

        isExecuted[15] = true; //初回処理用（レバーを倒しっぱなしでプレイし始めるときの対策）

        // ControlGroupの制御
        if (operationType == 0)
        {
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever Hole").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").gameObject.SetActive(false);
            this.transform.Find("Floor").Find("ButtonUnit").gameObject.SetActive(true);
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
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 1").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 2").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);
            this.transform.Find("Floor").Find("LeverUnit").gameObject.SetActive(true);
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

        // イニシャル移動とinsertFlagを後に実行
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.ArmLimit(armApertures);
        armController.ArmFinalClose();
        videoPlay.randomMode = true;
        if (!player2) craneBox.leftMoveFlag = true;
        else craneBox.rightMoveFlag = true;
        craneBox.forwardMoveFlag = true;

        while (true)
        {
            if (!player2 && craneBox.CheckPos(1)) break;
            if (player2 && craneBox.CheckPos(3)) break;
            await Task.Delay(1000);
        }

        craneStatus = 0;
    }

    // Update is called once per frame
    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            videoPlay.randomMode = true;
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
                        if (Random.Range(0, 2) == 0 && !player2) videoPlay.PlayVideo(4);
                        else videoPlay.PlayVideo(0);
                    }
                    InputKeyCheck(craneStatus);         //右移動ボタン有効化;
                }

                if (craneStatus == 2)
                { //右移動中
                    InputKeyCheck(craneStatus);
                    if (!player2 & craneBox.CheckPos(7))
                    {
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        buttonPushed = false;
                        craneStatus = 3;
                    }
                    if (player2 & craneBox.CheckPos(5))
                    {
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        buttonPushed = false;
                        craneStatus = 3;
                    }
                    //クレーン右移動;
                }

                if (craneStatus == 3)
                {
                    InputKeyCheck(craneStatus);         //奥移動ボタン有効化;
                }

                if (craneStatus == 4)
                { //奥移動中
                    InputKeyCheck(craneStatus);
                    if (craneBox.CheckPos(8))
                    {
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
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
                        videoPlay.PlayVideo(0);
                    }
                    InputLeverCheck();
                }
                if (craneStatus == 3)
                {
                    InputLeverCheck();
                    if (!leverTilted) InputKeyCheck(5);
                }
            }

            if (craneStatus == 5)
            {
                InputKeyCheck(craneStatus);
            }
            if (craneStatus == 6)
            {
                await Task.Delay(10);
                if (craneStatus == 6) InputKeyCheck(craneStatus);
            }
            if (craneStatus == 7)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.ArmOpen();
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
                    _SEPlayer.StopSE(3);
                    _SEPlayer.StopSE(4);
                    _SEPlayer.PlaySE(5, 1);
                    if (craneStatus == 8) ropeManager.ArmUnitDown(); //awaitによる時差実行を防止
                }
                InputKeyCheck(craneStatus);
                if (ropeManager.DownFinished() && craneStatus == 8) craneStatus = 9;
            }
            if (craneStatus == 9)
            {   //アーム掴む
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    if (catchTiming > 0) await Task.Delay(catchTiming);
                    if (leftCatchArmpower >= 30 || rightCatchArmpower >= 30) //閉じるときのアームパワーは大きい方を採用．最低値は30f
                    {
                        if (leftCatchArmpower >= rightCatchArmpower) armController.ArmClose(leftCatchArmpower);
                        else armController.ArmClose(rightCatchArmpower);
                    }
                    else armController.ArmClose(30f);
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
                        ropeManager.ArmUnitUp();
                        await Task.Delay(1000);
                        if (craneStatus < 13)
                        {
                            armController.MotorPower(leftCatchArmpower, 0);
                            armController.MotorPower(rightCatchArmpower, 1);
                        }
                    }
                    if (ropeManager.UpFinished() && craneStatus == 10) craneStatus = 11;
                }
            }
            if (craneStatus == 11)
            {   //アーム上昇停止
                if (!isExecuted[craneStatus])
                {
                    if (backTime > 0) await Task.Delay(backTime);
                    if (!player2) craneBox.leftMoveFlag = true;
                    else craneBox.rightMoveFlag = true;
                    craneBox.forwardMoveFlag = true;
                    craneStatus = 12;
                }
            }
            if (craneStatus == 12)
            {
                if (craneBox.CheckPos(1) && craneStatus == 12 & !player2) craneStatus = 13;
                if (craneBox.CheckPos(3) && craneStatus == 12 & player2) craneStatus = 13;
            }
            if (craneStatus == 13)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    videoPlay.PlayVideo(5);
                    armController.ArmLimit(100f); // アーム開口度を100に
                    armController.ArmOpen();
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
                    armController.ArmFinalClose();
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
                    armController.ArmLimit(armApertures); //アーム開口度リセット
                    if (!_SEPlayer._AudioSource[6].isPlaying) _SEPlayer.PlaySE(7, 1);

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

    public void GetPrize()
    {
        _SEPlayer.StopSE(7);
        if (!_SEPlayer._AudioSource[6].isPlaying)
            _SEPlayer.PlaySE(6, 1);
    }

    public void InputKeyCheck(int num)
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

                            videoPlay.PlayVideo(1);
                            isExecuted[15] = false;
                        }
                        craneStatus = 2;
                        _SEPlayer.PlaySE(1, 1);
                        if (!player2) craneBox.rightMoveFlag = true;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();

                            videoPlay.PlayVideo(1);
                            isExecuted[15] = false;
                        }
                        craneStatus = 2;
                        _SEPlayer.PlaySE(1, 1);
                        craneBox.leftMoveFlag = true;
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                    {
                        craneStatus = 3;
                        craneBox.rightMoveFlag = false;
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                    {
                        craneStatus = 3;
                        craneBox.leftMoveFlag = false;
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        _SEPlayer.PlaySE(1, 1);
                        craneBox.backMoveFlag = true;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        _SEPlayer.PlaySE(1, 1);
                        craneBox.backMoveFlag = true;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                    {
                        craneStatus = 5;
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        craneBox.backMoveFlag = false;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                    {
                        craneStatus = 5;
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        craneBox.backMoveFlag = false;
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
                            videoPlay.PlayVideo(2);
                        }
                        else
                        {
                            craneStatus = 7;
                            videoPlay.PlayVideo(3);
                        }
                    }

                    break;
                case 6:
                    if ((Input.GetKeyDown(downButtonNumpad) || Input.GetKeyDown(downButtonAlpha)) && !buttonPushed)
                    {
                        craneStatus = 7;
                        roter.RotateStop();
                        videoPlay.PlayVideo(3);
                    }
                    break;
                case 8:
                    if ((Input.GetKeyDown(downButtonNumpad) || Input.GetKeyDown(downButtonAlpha)) && downStop)
                    {
                        if (downStop)
                        {
                            ropeManager.ArmUnitDownForceStop();
                            craneStatus = 9;
                        }
                    }
                    break;
            }
        }
    }
    public void InputLeverCheck() // キーボード，UI共通のレバー処理
    {
        if (host.playable)
        {
            if (!player2)
            {
                if (isExecuted[15] && ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)) // 初回用の処理
                {
                    leverTilted = true;
                    isExecuted[15] = false;
                    videoPlay.PlayVideo(1);
                    _SEPlayer.StopSE(2);
                    _SEPlayer.PlaySE(1, 1);
                }

                if ((Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.G)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)
                {
                    leverTilted = true;
                    videoPlay.PlayVideo(1);
                    _SEPlayer.StopSE(2);
                    _SEPlayer.PlaySE(1, 1);
                }
                if ((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted)
                {
                    leverTilted = false;
                    videoPlay.PlayVideo(0);
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                }

                if (Input.GetKey(KeyCode.H) || lever.rightFlag)
                    craneBox.rightMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.H) || !lever.rightFlag)
                    craneBox.rightMoveFlag = false;
                if (Input.GetKey(KeyCode.F) || lever.leftFlag)
                    craneBox.leftMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.F) || !lever.leftFlag)
                    craneBox.leftMoveFlag = false;
                if (Input.GetKey(KeyCode.T) || lever.backFlag)
                    craneBox.backMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.T) || !lever.backFlag)
                    craneBox.backMoveFlag = false;
                if (Input.GetKey(KeyCode.G) || lever.forwardFlag)
                    craneBox.forwardMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.G) || !lever.forwardFlag)
                    craneBox.forwardMoveFlag = false;

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
                if (isExecuted[15] && ((Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)) // 初回用の処理
                {
                    leverTilted = true;
                    isExecuted[15] = false;
                    videoPlay.PlayVideo(1);
                    _SEPlayer.StopSE(2);
                    _SEPlayer.PlaySE(1, 1);
                }


                if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.K)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted)
                {
                    leverTilted = true;
                    videoPlay.PlayVideo(1);
                    _SEPlayer.StopSE(2);
                    _SEPlayer.PlaySE(1, 1);
                }
                if ((!Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K)
                && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted)
                {
                    leverTilted = false;
                    videoPlay.PlayVideo(0);
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                }

                if (Input.GetKey(KeyCode.L) || lever.rightFlag)
                    craneBox.rightMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.L) || !lever.rightFlag)
                    craneBox.rightMoveFlag = false;
                if (Input.GetKey(KeyCode.J) || lever.leftFlag)
                    craneBox.leftMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.J) || !lever.leftFlag)
                    craneBox.leftMoveFlag = false;
                if (Input.GetKey(KeyCode.I) || lever.backFlag)
                    craneBox.backMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.I) || !lever.backFlag)
                    craneBox.backMoveFlag = false;
                if (Input.GetKey(KeyCode.K) || lever.forwardFlag)
                    craneBox.forwardMoveFlag = true;
                else if (Input.GetKeyUp(KeyCode.K) || !lever.forwardFlag)
                    craneBox.forwardMoveFlag = false;

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

    public void ButtonDown(int num)
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
                        videoPlay.PlayVideo(1);
                        _SEPlayer.PlaySE(1, 1);
                        isExecuted[15] = false;
                    }
                    if (craneStatus == 2 && buttonPushed)
                    {
                        _SEPlayer.PlaySE(1, 1);
                        craneBox.rightMoveFlag = true;
                    }
                    break;
                case 2:
                    if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        _SEPlayer.PlaySE(1, 1);
                        craneBox.backMoveFlag = true;
                    }
                    break;
                case 3:
                    if ((craneStatus == 5 && operationType == 0) || (craneStatus == 3 && operationType == 1 && !leverTilted))
                    {
                        if (rotation)
                        {
                            craneStatus = 6;
                            roter.RotateStart();
                            videoPlay.PlayVideo(2);
                        }
                        else
                        {
                            craneStatus = 7;
                            videoPlay.PlayVideo(3);
                        }
                    }
                    else if (craneStatus == 6)
                    {
                        craneStatus = 7;
                        roter.RotateStop();
                        videoPlay.PlayVideo(3);
                    }
                    else if (craneStatus == 8)
                    {
                        if (downStop)
                        {
                            ropeManager.ArmUnitDownForceStop();
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
                        videoPlay.PlayVideo(1);
                        _SEPlayer.PlaySE(1, 1);
                        isExecuted[15] = false;
                    }
                    if (craneStatus == 2 && buttonPushed)
                    {
                        _SEPlayer.PlaySE(1, 1);
                        craneBox.leftMoveFlag = true;
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
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        craneBox.rightMoveFlag = false;
                        buttonPushed = false;
                    }
                    break;
                case 2:
                    if (craneStatus == 4 && buttonPushed)
                    {
                        craneStatus = 5;
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        craneBox.backMoveFlag = false;
                        buttonPushed = false;
                    }
                    break;
                case 4: // player2 case 1:
                    if (craneStatus == 2 && buttonPushed)
                    {
                        craneStatus = 3;
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 1);
                        craneBox.leftMoveFlag = false;
                        buttonPushed = false;
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
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0)
            {
                craneStatus = 1;
                videoPlay.randomMode = false;
            }
        }
    }
}
