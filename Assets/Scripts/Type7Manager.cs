using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type7Manager : CraneManager
{
    public int price = 100;
    public int times = 1;
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; //アームパワー(%，確率時)
    [SerializeField] int limitTimeSet = 60; //操作制限時間
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    [SerializeField] bool autoPower = true;
    public float armPower; //現在のアームパワー
    BGMPlayer _BGMPlayer;
    Type3ArmController armController;
    RopeManager ropeManager;
    ArmControllerSupport support;
    Lever[] lever = new Lever[2];
    Timer timer;
    int leverState = 0; // 0:ニュートラル，1:下降中，2:上昇中
    int armState = 0; // 0:閉じている，1:開いている
    public TextMesh limitTimedisplayed;

    async void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 7;
        // 様々なコンポーネントの取得
        host = this.transform.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever[0] = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        lever[1] = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").GetComponent<Lever>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;
        timer = this.transform.Find("Timer").GetComponent<Timer>();

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = price;
        creditSystem.rateSet[1, 0] = 0;
        creditSystem.rateSet[0, 1] = times;
        creditSystem.rateSet[1, 1] = 0;

        // ロープとアームコントローラに関する処理
        ropeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();
        support = temp.Find("ArmUnit").Find("Head").Find("Hat").GetComponent<ArmControllerSupport>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(_SEPlayer);
        timer.limitTimeNow = limitTimeSet;
        timer.limitTime = limitTimeSet;
        support.SetManager(7);
        support.SetRopeManager(ropeManager);
        creditSystem.SetCreditSound(0);
        armController.SetManager(7);
        armController.autoPower = autoPower;

        getPoint.SetManager(-1);
        getSoundNum = 4;

        await Task.Delay(300);
        ropeManager.Up();
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }

        for (int i = 0; i < 12; i++)
            isExecuted[i] = false;

        craneStatus = -1;
    }

    async void Update()
    {
        limitTimedisplayed.text = timer.limitTimeNow.ToString("D2");
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);

        if (craneStatus == -1)
            if (craneBox.CheckPos(1)) craneStatus = 0;

        if (craneStatus == 0)
        {
            _BGMPlayer.Play(0);
            //コイン投入有効化;
            if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) InsertCoin();
        }
        else
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                _BGMPlayer.Stop(0);
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(3000);
                    isExecuted[12] = false;
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
                    _SEPlayer.Play(1, 1);
                    await Task.Delay(3000);
                    timer.StartTimer();
                    _BGMPlayer.Play(1);
                    if (craneStatus == 1) craneStatus = 2;
                }

                //開始準備;
            }

            if (craneStatus == 2)
            { //移動可能
                if (timer.limitTimeNow <= 10)
                {
                    if (!isExecuted[craneStatus])
                    {
                        isExecuted[craneStatus] = true;
                        _BGMPlayer.Stop(1);
                        _SEPlayer.Play(2, 1);
                    }
                    if (!_SEPlayer.audioSource[2].isPlaying && timer.limitTimeNow <= 9) _SEPlayer.Play(3);
                }
                if (timer.limitTimeNow == 0) craneStatus = 7;
                DetectKey(0);
            }

            if (craneStatus == 7)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _SEPlayer.Stop(3);
                    _SEPlayer.Play(5, 1);
                    creditSystem.ResetPayment();
                    creditSystem.PlayStart();
                    ropeManager.DownForceStop();
                    ropeManager.UpForceStop();
                    leverState = 0;

                    if (probability) armPower = armPowerConfigSuccess[0];
                    else armPower = armPowerConfig[0];
                    armController.MotorPower(armPower);
                    if (armState == 1)
                    {
                        armState = 0;
                        armController.Close();
                    }
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
                    ropeManager.Up();
                    await Task.Delay(1500);
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 8 && support.prizeCount > 0) armController.Release(); // 上昇中に離す振り分け
                }
                if (probability && armPower > armPowerConfigSuccess[1])
                {
                    armPower -= 0.5f;
                    armController.MotorPower(armPower);
                }
                else if (!probability && armPower > armPowerConfig[1])
                {
                    armPower -= 0.5f;
                    armController.MotorPower(armPower);
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
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 9 && support.prizeCount > 0) armController.Release(); // 上昇後に離す振り分け
                    if (craneStatus == 9) craneStatus = 10;
                }
                //アーム上昇停止音再生;
                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
                //アーム獲得口ポジション移動音再生;
                if (!armController.autoPower)
                {
                    if (support.prizeCount > 0)
                    {
                        if (probability && armPower > armPowerConfigSuccess[2])
                        {
                            armPower -= 0.5f;
                            armController.MotorPower(armPower);
                        }
                        else if (!probability && armPower > armPowerConfig[2])
                        {
                            armPower -= 0.5f;
                            armController.MotorPower(armPower);
                        }
                    }
                    else armController.MotorPower(100f);
                }
                if (craneBox.CheckPos(1) && craneStatus == 10) craneStatus = 11;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.Open();
                    await Task.Delay(2000);
                    if (craneStatus == 11) craneStatus = 12;
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
                    await Task.Delay(1000);
                    timer.limitTimeNow = limitTimeSet;
                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                    //アーム閉じる音再生;
                    //アーム閉じる;
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
                craneBox.Left();
                craneBox.Forward();
            }
            if (craneStatus == 2) DetectLever();
        }
    }

    public override void GetPrize()
    {
        base.GetPrize();
        probability = creditSystem.ProbabilityCheck();
        Debug.Log("Probability:" + probability);
    }

    public void DetectLever() // キーボード，UI共通のレバー処理
    {
        if (host.playable)
        {
            if (Input.GetKey(KeyCode.H) || lever[0].rightFlag)
                craneBox.Right();
            if (Input.GetKey(KeyCode.F) || lever[0].leftFlag)
                craneBox.Left();
            if (Input.GetKey(KeyCode.T) || lever[0].backFlag)
                craneBox.Back();
            if (Input.GetKey(KeyCode.G) || lever[0].forwardFlag)
                craneBox.Forward();

            if ((Input.GetKey(KeyCode.I) || lever[1].backFlag) && leverState != 2)
            {
                Debug.Log("Up");
                leverState = 2;
                ropeManager.Up();
            }
            if ((Input.GetKey(KeyCode.K) || lever[1].forwardFlag) && leverState != 1 && !support.isShieldcollis)
            {
                Debug.Log("Down");
                leverState = 1;
                ropeManager.Down();
            }
            if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !lever[1].backFlag && !lever[1].forwardFlag)
            {
                leverState = 0;
                ropeManager.UpForceStop();
                ropeManager.DownForceStop();
            }
            if (support.isShieldcollis) ropeManager.DownForceStop();
        }
    }

    protected override void DetectKey(int num)
    {
        if (host.playable)
        {
            if (Input.GetKeyDown(KeyCode.O) && armState == 0)
            {
                armState = 1;
                armController.Open();
            }
            if (Input.GetKeyDown(KeyCode.L) && armState == 1)
            {
                armState = 0;
                armController.Close();
            }
        }
    }

    public override void ButtonDown(int num)
    {
        if (host.playable)
        {
            if (craneStatus >= 2 && craneStatus <= 3)
            {
                if (num == 0 && armState == 0)
                {
                    armState = 1;
                    armController.Open();
                }
                if (num == 1 && armState == 1)
                {
                    armState = 0;
                    armController.Close();
                }
            }
        }
    }
    public override void InsertCoin()
    {
        if (host.playable && craneStatus >= 0 && creditSystem.creditDisplayed == 0)
            if (creditSystem.Pay(100) >= 1) craneStatus = 1;
    }
}
