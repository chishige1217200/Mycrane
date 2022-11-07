using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Type8ManagerBeta : CraneManager
{
    [SerializeField] float catchArmpower = 100; //掴むときのアームパワー(%，未確率時)
    [SerializeField] float upArmpower = 100; //上昇時のアームパワー(%，未確率時)
    [SerializeField] float backArmpower = 100; //獲得口移動時のアームパワー(%，未確率時)
    [SerializeField] float catchArmpowersuccess = 100; //同確率時
    [SerializeField] float upArmpowersuccess = 100; //同確率時
    [SerializeField] float backArmpowersuccess = 100; //同確率時
    [SerializeField] int soundType = 1; //0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    [SerializeField] float audioPitch = 1f; //サウンドのピッチ
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    public float armPower; //現在のアームパワー
    BGMPlayer bp;
    Type8ArmController armController;
    RopeManager ropeManager;
    [SerializeField] TextMesh credit3d;

    async void Start()
    {
        Transform temp;

        craneStatus = -2;


        // 様々なコンポーネントの取得
        host = this.transform.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        bp = this.transform.Find("BGM").GetComponent<BGMPlayer>();
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
        armController = temp.Find("ArmUnit").GetComponent<Type8ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(13);
        if (soundType == 3) creditSystem.SetCreditSound(-1);
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
        bp.SetAudioPitch(audioPitch);
        sp.SetAudioPitch(audioPitch);

        getPoint.SetManager(this);

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
                    if (!sp.audioSource[5].isPlaying) bp.Play(0);
                    break;
                case 1:
                    if (!sp.audioSource[12].isPlaying) bp.Play(1);
                    break;
                case 2:
                    if (!sp.audioSource[16].isPlaying && !sp.audioSource[17].isPlaying)
                        bp.Play(2);
                    break;
                case 3:
                    bp.Stop(4);
                    bp.Play(3);
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
                    bp.Stop(soundType);
                }
                DetectKey(craneStatus);     //右移動ボタン有効化;
                switch (soundType)
                {
                    case 1:
                        if (!sp.audioSource[6].isPlaying)
                            sp.Play(7, 2147483647);
                        break;
                    case 3:
                        bp.Play(4);
                        break;
                }

            }

            if (craneStatus == 2)
            { //右移動中
                bp.Stop(soundType);
                DetectKey(craneStatus);
                //コイン投入無効化;
                switch (soundType)
                {
                    case 0:
                        sp.Play(1, 2147483647);
                        break;
                    case 1:
                        sp.Stop(7);
                        sp.Play(8, 2147483647);
                        break;
                    case 2:
                        sp.Play(14, 2147483647);
                        break;
                    case 3:
                        sp.Play(18, 2147483647);
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
                        sp.Play(2, 2147483647);
                        break;
                    case 1:
                        sp.Stop(8);
                        sp.Play(9, 2147483647);
                        break;
                    case 2:
                        sp.Play(14, 2147483647);
                        break;
                    case 3:
                        sp.Stop(18);
                        sp.Play(19, 2147483647);
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
                            sp.Play(3, 2147483647);
                            break;
                        case 1:
                            sp.Stop(9);
                            sp.Play(10, 2147483647);
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
                            sp.Play(21, 2147483647);
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
                    if (probability) armPower = catchArmpowersuccess;
                    else armPower = catchArmpower;
                    armController.SetMotorPower(armPower);
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
                            sp.Play(4, 2147483647);
                            break;
                        case 1:
                            sp.Stop(10);
                            sp.Play(11, 2147483647);
                            break;
                        case 3:
                            sp.Play(22, 2147483647);
                            break;
                    }
                    ropeManager.Up();
                    await Task.Delay(1500);
                }
                if (soundType == 2)
                    if (!sp.audioSource[15].isPlaying)
                        sp.Play(14, 2147483647);
                if (probability && armPower > upArmpowersuccess)
                {
                    armPower -= 0.5f;
                    armController.SetMotorPower(armPower);
                }
                else if (!probability && armPower > upArmpower)
                {
                    armPower -= 0.5f;
                    armController.SetMotorPower(armPower);
                }
                if (ropeManager.UpFinished() && craneStatus == 8) craneStatus = 9;
                //アーム上昇音再生;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {
                if (probability) armPower = upArmpowersuccess;
                else armPower = upArmpower;
                armController.SetMotorPower(armPower);
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 3:
                            sp.Stop(22);
                            break;
                    }
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
                            sp.Play(1, 2147483647);
                            break;
                        case 3:
                            sp.Play(23, 2147483647);
                            break;
                    }

                }
                if (soundType == 2)
                    if (!sp.audioSource[15].isPlaying)
                        sp.Play(14, 2147483647);

                if (probability && armPower > backArmpowersuccess)
                {
                    armPower -= 0.5f;
                    armController.SetMotorPower(armPower);
                }
                else if (!probability && armPower > backArmpower)
                {
                    armPower -= 0.5f;
                    armController.SetMotorPower(armPower);
                }
                else armController.SetMotorPower(100f);

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
        if (host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    public override void InsertCoinAuto()
    {
        if (craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
