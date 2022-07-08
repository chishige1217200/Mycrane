using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Type3ManagerV2 : CraneManager
{
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    [SerializeField] float[] armPowerConfig = new float[2]; //アームパワー(%)
    [SerializeField] float audioPitch = 1f; //サウンドのピッチ
    public float romVer = 1.7f;
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    [SerializeField] int[] armPowerErrorProbability = new int[2]; //アームパワーが弱くなる確率
    [SerializeField] int[] ropeBreakProbability = new int[2]; //ロープが切れる確率
    [SerializeField] int[] revWingProbability = new int[2]; //逆巻きする確率
    [SerializeField] int[] downStopProbability = new int[2]; //途中で下降停止してしまう確率
    [SerializeField] int[] humanProbability = new int[2]; //ヒューマンする確率
    [SerializeField] int[] upStopErrorProbability = new int[2]; //上昇停止スイッチが誤反応する確率
    [SerializeField] int[] upStopDisableProbability = new int[2]; //上昇停止スイッチが反応しない確率
    bool upStopDisable = false;
    bool revWingNow = false;
    bool doHuman = false;
    bool ropeBroken = false;
    int releaseTiming = -1; //離すタイミングの抽選 0-2
    BGMPlayer bp;
    Type3ArmController armController;
    RopeManager ropeManager;
    ArmControllerSupport support;
    Timer errorTimer;
    [SerializeField] TextMesh credit3d;
    EnarcYmCoord coord;

    void Start()
    {
        Transform temp;

        craneStatus = -4;
        craneType = 3;

        // 様々なコンポーネントの取得
        //host = transform.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        errorTimer = transform.Find("Timer").GetComponent<Timer>();
        coord = GetComponent<EnarcYmCoord>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            credit3d.text = "--";
            creditSystem.SetHibernate();
        }
        else
            credit3d.text = romVer.ToString("f1");

        // ロープとアームコントローラに関する処理
        ropeManager = transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();
        support = temp.Find("ArmUnit").Find("Head").Find("Hat").GetComponent<ArmControllerSupport>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        support.SetManager(this);
        support.SetLifter(ropeManager);
        creditSystem.SetSEPlayer(sp);
        creditSystem.SetCreditSound(0);
        bp.SetAudioPitch(audioPitch);
        sp.SetAudioPitch(audioPitch);
        armController.SetManager(3);
        getPoint.SetManager(this);
        coord.SetManager(this);
        getSoundNum = 3;

        // 確率変数エラーチェック
        if (armPowerErrorProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (armPowerErrorProbability[0] > armPowerErrorProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = armPowerErrorProbability[0];
            armPowerErrorProbability[0] = armPowerErrorProbability[1];
            armPowerErrorProbability[1] = swap;
        }

        if (ropeBreakProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (ropeBreakProbability[0] > ropeBreakProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = ropeBreakProbability[0];
            ropeBreakProbability[0] = ropeBreakProbability[1];
            ropeBreakProbability[1] = swap;
        }

        if (revWingProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (revWingProbability[0] > revWingProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = revWingProbability[0];
            revWingProbability[0] = revWingProbability[1];
            revWingProbability[1] = swap;
        }

        if (downStopProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (downStopProbability[0] > downStopProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = downStopProbability[0];
            downStopProbability[0] = downStopProbability[1];
            downStopProbability[1] = swap;
        }

        if (humanProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (humanProbability[0] > humanProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = humanProbability[0];
            humanProbability[0] = humanProbability[1];
            humanProbability[1] = swap;
        }

        if (upStopErrorProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (upStopErrorProbability[0] > upStopErrorProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = upStopErrorProbability[0];
            upStopErrorProbability[0] = upStopErrorProbability[1];
            upStopErrorProbability[1] = swap;
        }

        if (upStopDisableProbability.Length < 2) Debug.LogError("確率分数が適切に設定されていません");
        if (upStopDisableProbability[0] > upStopDisableProbability[1]) // 確率分数に不正な値が設定されたとき
        {
            int swap = upStopDisableProbability[0];
            upStopDisableProbability[0] = upStopDisableProbability[1];
            upStopDisableProbability[1] = swap;
        }

        if (UnityEngine.Random.Range(1, armPowerErrorProbability[1] + 1) <= armPowerErrorProbability[0]) //アームパワー半減エラー
        {
            armPowerConfig[0] = armPowerConfig[0] / 2;
            armPowerConfig[1] = armPowerConfig[1] / 2;
        }

        if (UnityEngine.Random.Range(1, humanProbability[1] + 1) <= humanProbability[0])
        {
            doHuman = true;
        }

        craneStatus = -3;
        Invoke("Initialize", 1);
    }

    public async void Initialize()
    {
        if (craneStatus == -3 || craneStatus == 99)
        {
            craneStatus = -2;
            sp.Stop(5);
            credit3d.text = romVer.ToString("f1");
            errorTimer.StartTimer();
            if (!ropeBroken)
            {
                armController.Open();
                ropeManager.Up();
            }
            while (!ropeManager.UpFinished())
            {
                if (craneStatus == 99) return;
                await Task.Delay(100);
            }

            errorTimer.CancelTimer();
            coord.ResetCoordinate();
            coord.ResetBeforePush();

            for (int i = 0; i < 12; i++)
                isExecuted[i] = false;

            int credit = creditSystem.Pay(0);
            if (credit < 0x100) credit3d.text = credit.ToString("X");
            else credit3d.text = "FF.";

            //ropeManager.SetUpSpeed(0.0015f);
            //ropeManager.SetDownSpeed(0.0015f);
            //ResetCoordinate();

            craneStatus = -1;
        }
    }

    public void Error()
    {
        if (craneStatus == -2 && !ropeBroken)
        {
            RopePoint r = ropeManager.ropePoint[ropeManager.ropePoint.Length - 1];
            r.GetComponent<HingeJoint>().breakForce = 0.01f;
        }
        craneStatus = 99;
        errorTimer.CancelTimer();
        ropeManager.UpForceStop();
        ropeManager.DownForceStop();
        credit3d.text = "E6";
        sp.Stop(1);
        sp.Stop(2);
        sp.Stop(3);
        sp.Stop(4);
        sp.Play(5);
    }

    public async void Human()
    {
        if (doHuman)
        {
            HingeJoint h = transform.Find("CraneUnit").Find("ArmUnit").Find("Head").GetComponent<HingeJoint>();
            h.useMotor = true;
            await Task.Delay(500);
            h.useMotor = false;
        }
    }

    public async void DownStopError()
    {
        if (UnityEngine.Random.Range(1, downStopProbability[1] + 1) <= downStopProbability[0])
        {
            int downErrorTime = UnityEngine.Random.Range(1, 2001);
            await Task.Delay(downErrorTime);
            if (craneStatus == 6)
            {
                ropeManager.DownForceStop();
                craneStatus = 7;
            }
        }
    }
    public async void UpStopError()
    {
        if (UnityEngine.Random.Range(1, upStopErrorProbability[1] + 1) <= upStopErrorProbability[0])
        {
            int upErrorTime = UnityEngine.Random.Range(1, 2001);
            await Task.Delay(upErrorTime);
            if (craneStatus == 8)
            {
                ropeManager.UpForceStop();
                craneStatus = 9;
            }
        }
    }

    public async void RopeBreak()
    {
        if (UnityEngine.Random.Range(1, ropeBreakProbability[1] + 1) <= ropeBreakProbability[0])
        {
            int waitTime = UnityEngine.Random.Range(500, 3001);
            await Task.Delay(waitTime);
            if (!ropeBroken)
            {
                ropeBroken = true;
                RopePoint r = ropeManager.ropePoint[ropeManager.ropePoint.Length - 1];
                r.GetComponent<HingeJoint>().breakForce = 0.01f;
            }
        }
    }

    public bool UpStopDisable()
    {
        if (UnityEngine.Random.Range(1, upStopDisableProbability[1] + 1) <= upStopDisableProbability[0])
            return true;
        return false;
    }

    async void Update()
    {
        if (host.playable && craneStatus == 99 && Input.GetKeyDown(KeyCode.R)) Initialize();
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();

        if (craneStatus == -1)
            if (craneBox.CheckPos(1))
            {
                craneStatus = 0;
                int credit = creditSystem.Pay(0);
                if (credit > 0 && craneStatus == 0) craneStatus = 1;
            }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (!sp.audioSource[3].isPlaying && !sp.audioSource[4].isPlaying)
                bp.Play(0);
        }
        else
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    bp.Stop(0);
                }
                DetectKey(craneStatus);     //右移動ボタン有効化;
            }

            if (craneStatus == 2)
            {
                //右移動中
                bp.Stop(0);
                DetectKey(craneStatus);
                //コイン投入無効化;
                sp.Play(1);
                if (craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
                //クレーン右移動;
                //右移動効果音ループ再生;
            }

            if (craneStatus == 3)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    coord.ReleaseButton(0);
                }
                DetectKey(craneStatus);         //奥移動ボタン有効化;
                //右移動効果音ループ再生停止;
            }

            if (craneStatus == 4)
            {
                //奥移動中
                DetectKey(craneStatus);
                //クレーン奥移動;
                sp.Play(1);
                if (craneBox.CheckPos(8))
                {
                    buttonPushed = false;
                    craneStatus = 5;
                }
                //奥移動効果音ループ再生;
            }

            if (craneStatus == 5)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(1);
                    coord.ReleaseButton(1);
                    if (probability) releaseTiming = -1;
                    else releaseTiming = UnityEngine.Random.Range(0, 3);
                    if (releaseTiming == 2)
                    {
                        if (romVer < 2)
                            releaseTiming = UnityEngine.Random.Range(0, 2);
                        else if (romVer == 4.2f)
                        {
                            if (UnityEngine.Random.Range(1, 101) <= 99)
                                releaseTiming = UnityEngine.Random.Range(0, 2);
                        }
                    }

                    if (!ropeManager.DownFinished() && (revWingNow || UnityEngine.Random.Range(1, revWingProbability[1] + 1) <= revWingProbability[0]))
                    {
                        revWingNow = true;
                        int speedt = UnityEngine.Random.Range(1, 3);
                        int start6 = UnityEngine.Random.Range(0, 6);
                        float speed = 0f;
                        if (speedt == 1) // 早くなる方向
                            speed = UnityEngine.Random.Range(0.005f, 0.01f);
                        else // 遅くなる方向
                            speed = UnityEngine.Random.Range(0.0002f, 0.001f);

                        for (int i = start6; i < start6 + 7; i++)
                            ropeManager.SetDownSpeed(speed, i);
                    }

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
                    errorTimer.StartTimer();
                    sp.Play(2, 1);
                    DownStopError();
                    if (downTime > 0 && downTime <= 4600)
                    {
                        await Task.Delay(downTime);
                        if (craneStatus == 6)
                        {
                            ropeManager.DownForceStop();
                            craneStatus = 7;
                        }
                    }
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
                    errorTimer.CancelTimer();
                    armController.Close();
                    armController.MotorPower(armPowerConfig[0]);
                    upStopDisable = UpStopDisable();
                    await Task.Delay(1000);
                    if (craneStatus == 7) craneStatus = 8;
                }
                //アーム下降音再生停止;
                //アーム掴む音再生;
                //アーム掴む;
            }

            if (craneStatus == 8)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;

                    if (revWingNow)
                    {
                        revWingNow = false;
                        if (UnityEngine.Random.Range(1, 3) <= 1)
                        {
                            int start6 = UnityEngine.Random.Range(0, 6);
                            float speed = UnityEngine.Random.Range(0.0002f, 0.001f);

                            for (int i = start6; i < start6 + 7; i++)
                                ropeManager.SetUpSpeed(speed, i);
                        }
                    }
                    if (!ropeBroken)
                    {
                        ropeManager.Up();
                        UpStopError();
                    }
                    RopeBreak();
                    errorTimer.StartTimer();
                    await Task.Delay(1500);
                    if (releaseTiming == 0) armController.MotorPower(armPowerConfig[1]);
                }
                if (!sp.audioSource[2].isPlaying)
                    sp.Play(1);
                if (!upStopDisable && ropeManager.UpFinished() && craneStatus == 8)
                {
                    Human();
                    craneStatus = 9;
                }
                //アーム上昇音再生;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {
                if (releaseTiming == 1) armController.MotorPower(armPowerConfig[1]);
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    errorTimer.CancelTimer();
                    await Task.Delay(200);
                    if (craneStatus == 9) craneStatus = 10;
                }
                //アーム上昇停止音再生;
                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
                if (releaseTiming == 2 && coord.isOnHome())
                    armController.MotorPower(armPowerConfig[1]);
                //アーム獲得口ポジション移動音再生;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(2);
                    sp.Stop(1);
                    sp.Play(1);
                }
                if (craneBox.CheckPos(1) && craneStatus == 10) craneStatus = 11;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    coord.ResetCoordinate();
                    coord.ResetBeforePush();
                    upStopDisable = false;
                    armController.Open();
                    await Task.Delay(1000);
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
                    sp.Stop(1);
                    if (!sp.audioSource[3].isPlaying)
                        sp.Play(4, 1);
                    for (int i = 0; i < 12; i++)
                        isExecuted[i] = false;
                    await Task.Delay(1500);

                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                    //アーム閉じる音再生;
                    //アーム閉じる;
                }
            }

            if (errorTimer.limitTimeNow == 0 && craneStatus != 99) // Error
            {
                //if (craneStatus != -2) errorTimer.CancelTimer();
                Error();
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus != 0)
        {
            if (craneStatus == -1 || craneStatus == 10)
            {
                craneBox.Left();
                craneBox.Forward();
            }
            else if (craneStatus == 2)
                craneBox.Right();
            else if (craneStatus == 4)
                craneBox.Back();
        }
    }

    public override void GetPrize()
    {
        sp.Stop(1);
        sp.Stop(4);
        base.GetPrize();
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
                            if (credit < 0x100) credit3d.text = credit.ToString("X");
                            else credit3d.text = "FF.";
                            isExecuted[12] = false;
                            probability = creditSystem.ProbabilityCheck();
                            Debug.Log("Probability:" + probability);
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
                        if (credit < 0x100) credit3d.text = credit.ToString("X");
                        else credit3d.text = "FF.";
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
        if (!isHibernate && host.playable && craneStatus >= 0 && craneStatus != 99)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 0x100) credit3d.text = credit.ToString("X");
            else credit3d.text = "FF.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
