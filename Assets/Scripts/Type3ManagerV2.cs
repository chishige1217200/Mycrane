using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Type3ManagerV2 : CraneManager
{
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%)
    [SerializeField] float audioPitch = 1f; //サウンドのピッチ
    [SerializeField] float romVer = 1.7f;
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    public float armPower; //現在のアームパワー
    BGMPlayer bp;
    Type3ArmController armController;
    RopeManager ropeManager;
    [SerializeField] TextMesh credit3d;

    async void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 3;

        // 様々なコンポーネントの取得
        //host = transform.Find("CP").GetComponent<MachineHost>();
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
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

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        creditSystem.SetCreditSound(0);
        bp.SetAudioPitch(audioPitch);
        sp.SetAudioPitch(audioPitch);
        armController.SetManager(3);
        getPoint.SetManager(this);

        Invoke("Initialize", 1);
    }

    public async void Initialize()
    {
        armController.Open();
        ropeManager.Up();
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }

        for (int i = 0; i < 12; i++)
            isExecuted[i] = false;

        credit3d.text = "00";

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
                    sp.Play(2, 1);
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
                    armPower = armPowerConfig[0];
                    armController.Close();
                    armController.MotorPower(armPower);
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
                }
                if (!sp.audioSource[2].isPlaying)
                    sp.Play(1);
                if (ropeManager.UpFinished() && craneStatus == 8) craneStatus = 9;
                //アーム上昇音再生;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {

                armPower = armPowerConfig[1];
                armController.MotorPower(armPower);
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(200);
                    if (craneStatus == 9) craneStatus = 10;
                }
                //アーム上昇停止音再生;
                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
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
                    if (armPower > armPowerConfig[2]) armPower -= 0.5f;
                    armController.MotorPower(armPower);
                }
                craneBox.Left();
                craneBox.Forward();
            }
            else if (craneStatus == 2) craneBox.Right();
            else if (craneStatus == 4) craneBox.Back();
            else if (craneStatus == 8)
            {
                if (armPower > armPowerConfig[1]) armPower -= 0.5f;
                else armController.MotorPower(armPower);
            }
        }
    }

    public override void GetPrize()
    {
        getSoundNum = 3;
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
        if (!isHibernate && host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 0x100) credit3d.text = credit.ToString("X");
            else credit3d.text = "FF.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
