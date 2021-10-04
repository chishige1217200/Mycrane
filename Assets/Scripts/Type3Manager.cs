using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type3Manager : CraneManager
{
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; //アームパワー(%，確率時)
    [SerializeField] int soundType = 1; //0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    [SerializeField] float audioPitch = 1f; //サウンドのピッチ
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    [SerializeField] bool autoPower = true;
    public float armPower; //現在のアームパワー
    BGMPlayer _BGMPlayer;
    Type3ArmController armController;
    RopeManager ropeManager;
    ArmControllerSupport support;
    [SerializeField] TextMesh credit3d;

    async void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 3;

        // 様々なコンポーネントの取得
        host = this.transform.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        sp = this.transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];

        // ロープとアームコントローラに関する処理
        ropeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();
        support = temp.Find("ArmUnit").Find("Head").Find("Hat").GetComponent<ArmControllerSupport>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        support.SetManager(3);
        support.SetRopeManager(ropeManager);
        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(13);
        if (soundType == 3) creditSystem.SetCreditSound(-1);
        _BGMPlayer.SetAudioPitch(audioPitch);
        sp.SetAudioPitch(audioPitch);
        armController.SetManager(3);
        armController.autoPower = autoPower;

        getPoint.SetManager(-1);

        await Task.Delay(300);
        ropeManager.Up();
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        if (soundType == 2) armController.Open();
        else armController.Close();

        for (int i = 0; i < 12; i++)
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
                    if (!sp.audioSource[5].isPlaying) _BGMPlayer.Play(0);
                    break;
                case 1:
                    if (!sp.audioSource[12].isPlaying) _BGMPlayer.Play(1);
                    break;
                case 2:
                    if (!sp.audioSource[16].isPlaying && !sp.audioSource[17].isPlaying)
                        _BGMPlayer.Play(2);
                    break;
                case 3:
                    _BGMPlayer.Stop(4);
                    _BGMPlayer.Play(3);
                    break;
            }
        }
        else
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _BGMPlayer.Stop(soundType);
                }
                DetectKey(craneStatus);     //右移動ボタン有効化;
                switch (soundType)
                {
                    case 1:
                        if (!sp.audioSource[6].isPlaying)
                            sp.Play(7);
                        break;
                    case 3:
                        _BGMPlayer.Play(4);
                        break;
                }

            }

            if (craneStatus == 2)
            { //右移動中
                _BGMPlayer.Stop(soundType);
                DetectKey(craneStatus);
                //コイン投入無効化;
                switch (soundType)
                {
                    case 0:
                        sp.Play(1);
                        break;
                    case 1:
                        sp.Stop(7);
                        sp.Play(8);
                        break;
                    case 2:
                        sp.Play(14);
                        break;
                    case 3:
                        sp.Play(18);
                        break;
                }
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
            { //奥移動中
                DetectKey(craneStatus);
                //クレーン奥移動;
                switch (soundType)
                {
                    case 0:
                        sp.Stop(1);
                        sp.Play(2);
                        break;
                    case 1:
                        sp.Stop(8);
                        sp.Play(9);
                        break;
                    case 2:
                        sp.Play(14);
                        break;
                    case 3:
                        sp.Stop(18);
                        sp.Play(19);
                        break;
                }
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
                    if (soundType != 2) armController.Open();
                    switch (soundType)
                    {
                        case 0:
                            sp.Stop(2);
                            sp.Play(3);
                            break;
                        case 1:
                            sp.Stop(9);
                            sp.Play(10);
                            break;
                        case 2:
                            sp.Stop(14);
                            break;
                        case 3:
                            sp.Stop(19);
                            sp.Play(20, 1);
                            break;
                    }
                    if (soundType != 2)
                    {
                        if (soundType == 3) await Task.Delay(2000);
                        else await Task.Delay(1000);
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
                    switch (soundType)
                    {
                        case 2:
                            sp.Play(15, 2);
                            break;
                        case 3:
                            sp.Play(21);
                            break;
                    }
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
                    switch (soundType)
                    {
                        case 3:
                            sp.Stop(21);
                            break;
                    }
                    if (probability) armPower = armPowerConfigSuccess[0];
                    else armPower = armPowerConfig[0];
                    armController.MotorPower(armPower);
                    armController.Close();
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
                    switch (soundType)
                    {
                        case 0:
                            sp.Stop(3);
                            sp.Play(4);
                            break;
                        case 1:
                            sp.Stop(10);
                            sp.Play(11);
                            break;
                        case 3:
                            sp.Play(22);
                            break;
                    }
                    ropeManager.Up();
                    await Task.Delay(1500);
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 8 && support.prizeCount > 0) armController.Release(); // 上昇中に離す振り分け
                }
                if (soundType == 2)
                    if (!sp.audioSource[15].isPlaying)
                        sp.Play(14);
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
                    switch (soundType)
                    {
                        case 3:
                            sp.Stop(22);
                            break;
                    }
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 9 && support.prizeCount > 0) armController.Release(); // 上昇後に離す振り分け
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
                    switch (soundType)
                    {
                        case 0:
                            sp.Stop(4);
                            sp.Play(1);
                            break;
                        case 3:
                            sp.Play(23);
                            break;
                    }

                }
                if (soundType == 2)
                    if (!sp.audioSource[15].isPlaying)
                        sp.Play(14);
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
                    switch (soundType)
                    {
                        case 3:
                            sp.Stop(23);
                            sp.Play(24, 1);
                            break;
                    }
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
                    if (soundType != 2) armController.Close();
                    switch (soundType)
                    {
                        case 2:
                            sp.Stop(14);
                            if (!sp.audioSource[16].isPlaying)
                                sp.Play(17, 1);
                            break;
                        case 3:
                            sp.Play(25, 1);
                            break;
                    }
                    for (int i = 0; i < 12; i++)
                        isExecuted[i] = false;
                    await Task.Delay(1000);
                    if (soundType == 3) await Task.Delay(1000);
                    switch (soundType)
                    {
                        case 0:
                            sp.Stop(1);
                            break;
                        case 1:
                            sp.Stop(11);
                            break;
                    }
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
            else if (craneStatus == 2) craneBox.Right();
            else if (craneStatus == 4) craneBox.Back();
        }
    }

    public override void GetPrize()
    {
        switch (soundType)
        {
            case 0:
                getSoundNum = 5;
                sp.Stop(1);
                break;
            case 1:
                getSoundNum = 12;
                sp.Stop(11);
                break;
            case 2:
                getSoundNum = 16;
                sp.Stop(14);
                sp.Stop(17);
                break;
            case 3:
                getSoundNum = 26;
                sp.Stop(25);
                break;
        }

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
                            if (credit < 10) credit3d.text = credit.ToString();
                            else credit3d.text = "9.";
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
                        if (credit < 10) credit3d.text = credit.ToString();
                        else credit3d.text = "9.";
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
        if (host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
