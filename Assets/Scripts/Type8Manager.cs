﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Type8Manager : CraneManager
{
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; //アームパワー(%，確率時)
    [SerializeField] int limitTimeSet = 60; //残り時間を設定
    [SerializeField] int soundType = 1; //0:GEORGE, 1:FW1, 2:FW2, 3:JOHNNY1, 4:JOHNNY2
    [SerializeField] float audioPitch = 1f; //サウンドのピッチ
    public bool leverTilted = false; //trueならレバーがアクティブ
    [SerializeField] bool downStop = true; //下降停止の利用可否
    bool[] isExecuted = new bool[14]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    public float armPower; //現在のアームパワー
    BGMPlayer _BGMPlayer;
    Type8ArmController armController;
    RopeManager ropeManager;
    Lever lever;
    Timer timer;
    public Text limitTimedisplayed;
    [SerializeField] TextMesh credit3d;

    async void Start()
    {
        Transform temp;

        craneStatus = -2;

        // 様々なコンポーネントの取得
        host = transform.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = transform.Find("SE").GetComponent<SEPlayer>();
        lever = transform.Find("Canvas").Find("ControlGroup").Find("Lever").GetComponent<Lever>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        timer = transform.Find("Timer").GetComponent<Timer>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];

        // ロープとアームコントローラに関する処理
        ropeManager = transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type8ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.GetSEPlayer(_SEPlayer);
        timer.limitTime = limitTimeSet;

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                creditSystem.SetCreditSound(0);
                getSoundNum = 6;
                break;
            case 3:
            case 4:
                creditSystem.SetCreditSound(7);
                getSoundNum = 11;
                break;
        }

        _BGMPlayer.SetAudioPitch(audioPitch);
        _SEPlayer.SetAudioPitch(audioPitch);

        getPoint.GetManager(-1); // テスト中

        await Task.Delay(300);
        ropeManager.ArmUnitUp();
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.ArmOpen();

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
            if (craneBox.CheckPos(1)) craneStatus = 0;

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            switch (soundType)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    _BGMPlayer.StopBGM(soundType * 2 + 1);
                    _BGMPlayer.PlayBGM(soundType * 2);
                    break;
                case 4:
                    _BGMPlayer.StopBGM(6);
                    _BGMPlayer.PlayBGM(9);
                    break;
            }
        }
        else
        {
            if (craneStatus == 1) //スタートポジションに移動
            {
                //コイン投入有効化;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    craneBox.goPoint = new Vector2(0.05f, 0);
                    craneBox.goPositionFlag = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _BGMPlayer.StopBGM(soundType * 2);
                            _BGMPlayer.PlayBGM(soundType * 2 + 1);
                            break;
                        case 3:
                            _BGMPlayer.StopBGM(soundType * 2);
                            break;
                        case 4:
                            _BGMPlayer.StopBGM(9);
                            break;
                    }
                }
                if (craneBox.CheckPos(9) && craneStatus == 1) IncrimentStatus();
            }

            if (craneStatus == 2) //操作待ち
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 3:
                            _BGMPlayer.PlayBGM(soundType * 2 + 1);
                            break;
                        case 4:
                            _BGMPlayer.PlayBGM(6);
                            break;
                    }
                }
                if ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                    || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)
                {
                    leverTilted = true;
                }
                if (((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted) || !host.playable)
                {
                    leverTilted = false;
                }
            }

            if (craneStatus == 3) //操作中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    timer.StartTimer();
                    creditSystem.segUpdateFlag = false;
                }
                if ((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                    || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverTilted && host.playable)
                {
                    leverTilted = true;
                }
                if (((!Input.GetKey(KeyCode.H) && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.T) && !Input.GetKey(KeyCode.G)
                && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverTilted) || !host.playable)
                {
                    leverTilted = false;
                }
                DetectKey(craneStatus);
                if (isExecuted[craneStatus] && timer.limitTimeNow <= 0) IncrimentStatus();
            }

            if (craneStatus == 4) //下降中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(1, 1);
                            break;
                        case 3:
                            _BGMPlayer.StopBGM(soundType * 2 + 1);
                            _SEPlayer.PlaySE(9, 1);
                            break;
                        case 4:
                            _BGMPlayer.StopBGM(6);
                            _BGMPlayer.PlayBGM(8);
                            break;
                    }
                    await Task.Delay(300);
                    timer.CancelTimer();
                    creditSystem.segUpdateFlag = true;
                    int credit = creditSystem.Pay(0);
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
                    ropeManager.ArmUnitDown();
                    if (downTime > 0 && downTime <= 5000)
                    {
                        await Task.Delay(downTime);
                        if (craneStatus == 4)
                        {
                            ropeManager.ArmUnitDownForceStop();
                            IncrimentStatus();
                        }
                    }
                }
                if (isExecuted[craneStatus]) DetectKey(craneStatus);
                if (soundType == 3 && !_SEPlayer._AudioSource[9].isPlaying) _BGMPlayer.PlayBGM(soundType * 2 + 1);
                if (ropeManager.DownFinished() && craneStatus == 4) IncrimentStatus();
            }

            if (craneStatus == 5) //アーム掴む
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.StopSE(1);
                            _SEPlayer.PlaySE(2, 1);
                            break;
                        case 3:
                            _BGMPlayer.StopBGM(soundType * 2 + 1);
                            _SEPlayer.PlaySE(10, 1);
                            break;
                        case 4:
                            _BGMPlayer.StopBGM(8);
                            _SEPlayer.PlaySE(10, 1);
                            break;
                    }
                    armController.ArmClose();
                    await Task.Delay(1000);
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
                        IncrimentStatus();
                    }
                }
                if (soundType == 3 && !_SEPlayer._AudioSource[10].isPlaying) _BGMPlayer.PlayBGM(soundType * 2 + 1);
            }

            if (craneStatus == 6) //上昇中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(3, 1);
                            break;
                    }
                    ropeManager.ArmUnitUp();
                }

                if (!_SEPlayer._AudioSource[10].isPlaying)
                {
                    if (soundType == 3) _BGMPlayer.PlayBGM(soundType * 2 + 1);
                    if (soundType == 4) _BGMPlayer.PlayBGM(6);
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

                if (ropeManager.UpFinished() && craneStatus == 6) IncrimentStatus();
            }

            if (craneStatus == 7) //アーム上昇停止
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.StopSE(3);
                            _SEPlayer.PlaySE(4, 1);
                            break;
                    }
                    if (probability) armPower = armPowerConfigSuccess[1];
                    else armPower = armPowerConfig[1];
                    armController.MotorPower(armPower);
                    IncrimentStatus();
                }
                if (!_SEPlayer._AudioSource[10].isPlaying)
                {
                    if (soundType == 3) _BGMPlayer.PlayBGM(soundType * 2 + 1);
                    if (soundType == 4) _BGMPlayer.PlayBGM(6);
                }
            }

            if (craneStatus == 8) //離すポジションに移動
            {
                if (!_SEPlayer._AudioSource[10].isPlaying)
                {
                    if (soundType == 3) _BGMPlayer.PlayBGM(soundType * 2 + 1);
                    if (soundType == 4) _BGMPlayer.PlayBGM(6);
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
                if (craneBox.CheckPos(8)) IncrimentStatus();
            }

            if (craneStatus == 9) //離すポジションに移動
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    craneBox.goPoint = new Vector2(-0.2f, craneBox.transform.localPosition.z);
                    craneBox.goPositionFlag = true;
                }
                if (!_SEPlayer._AudioSource[10].isPlaying)
                {
                    if (soundType == 3) _BGMPlayer.PlayBGM(soundType * 2 + 1);
                    if (soundType == 4) _BGMPlayer.PlayBGM(6);
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
                if (craneBox.CheckPos(9)) IncrimentStatus();
            }

            if (craneStatus == 10) //下降中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 3:
                            _BGMPlayer.StopBGM(soundType * 2 + 1);
                            _SEPlayer.PlaySE(8, 2147483647);
                            break;
                        case 4:
                            _BGMPlayer.StopBGM(6);
                            _SEPlayer.PlaySE(8, 2147483647);
                            break;
                    }
                    ropeManager.ArmUnitDown();

                    await Task.Delay(1000);
                    ropeManager.ArmUnitDownForceStop();
                    IncrimentStatus();
                }
            }

            if (craneStatus == 11) //離す
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.ArmOpen();
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(5, 1);
                            break;
                        case 3:
                        case 4:
                            _SEPlayer.StopSE(8);
                            _SEPlayer.PlaySE(10, 1);
                            break;
                    }
                    await Task.Delay(1000);
                    IncrimentStatus();
                }
            }

            if (craneStatus == 12) //上昇中
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 3:
                        case 4:
                            _SEPlayer.PlaySE(8, 2147483647);
                            break;
                    }
                    ropeManager.ArmUnitUp();
                }
                if (ropeManager.UpFinished()) IncrimentStatus();
            }

            if (craneStatus == 13) //上昇停止
            {

                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 3:
                            _SEPlayer.StopSE(8);
                            _BGMPlayer.PlayBGM(soundType * 2 + 1);
                            break;
                        case 4:
                            _SEPlayer.StopSE(8);
                            _BGMPlayer.PlayBGM(6);
                            break;
                    }
                    for (int i = 0; i < 13; i++)
                        isExecuted[i] = false;

                    if (creditSystem.creditDisplayed > 0) craneStatus = 1;
                    else IncrimentStatus();
                }
            }
            if (craneStatus == 14) //帰還
            {
                if (craneBox.CheckPos(1))
                {
                    if (creditSystem.creditDisplayed > 0) craneStatus = 1;
                    else craneStatus = 0;
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
            if (craneStatus == -1 || craneStatus == 14)
            {
                craneBox.Left();
                craneBox.Forward();
            }
            else if (craneStatus == 2 || craneStatus == 3) DetectLever();
            else if (craneStatus == 8) craneBox.Back();
        }
    }

    public override void DetectKey(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)))
                        IncrimentStatus();
                    break;
                case 4:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && downStop)
                    {
                        Debug.Log("DownStop!");
                        ropeManager.ArmUnitDownForceStop();
                        IncrimentStatus();
                    }
                    break;
            }
        }
    }

    public void DetectLever()
    {
        if (host.playable)
        {
            int credit = 0;

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
                if (craneStatus == 2)
                {
                    IncrimentStatus();
                    creditSystem.ResetPayment();
                    credit = creditSystem.PlayStart();
                    if (credit < 100) credit3d.text = credit.ToString();
                    else credit3d.text = "99.";
                    isExecuted[13] = false;
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
                }
        }
    }

    public override void ButtonDown(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 2:
                    if (craneStatus == 3 && !leverTilted)
                    {
                        IncrimentStatus();
                    }
                    else if (craneStatus == 4 && downStop)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        IncrimentStatus();
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