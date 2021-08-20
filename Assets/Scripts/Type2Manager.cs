using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type2Manager : MonoBehaviour
{
    public int craneStatus = -2; //-1:初期化動作，0:待機状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] float catchArmpower = 80f; //掴むときのアームパワー(%，未確率時)
    [SerializeField] float upArmpower = 0f; //上昇時のアームパワー(%，未確率時)
    [SerializeField] float backArmpower = 0f; //獲得口移動時のアームパワー(%，未確率時)
    [SerializeField] float catchArmpowersuccess = 100f; //同確率時
    [SerializeField] float upArmpowersuccess = 100f; //同確率時
    [SerializeField] float backArmpowersuccess = 100f; //同確率時
    [SerializeField] int operationType = 1; //0:ボタン式，1:レバー式
    [SerializeField] int limitTimeSet = 15; //レバー式の場合，残り時間を設定
    [SerializeField] int soundType = 0; //DECACRE:0, DECACRE Alpha:1, TRIPLE CATCHER MEGA DASH:2
    bool timerFlag = false; //タイマーの起動はaプレイにつき1度のみ実行
    float audioPitch = 1.0f; //サウンドのピッチ
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool probability; //確率判定用
    float armPower; //現在のアームパワー

    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    BGMPlayer _BGMPlayer;
    SEPlayer _SEPlayer;
    Type2ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    Lever lever;
    Timer timer;
    MachineHost host;
    GameObject canvas;
    public Text limitTimedisplayed;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];

    async void Start()
    {
        Transform temp;
        // 様々なコンポーネントの取得
        host = this.transform.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
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
        armController = temp.Find("ArmUnit").GetComponent<Type2ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.GetSEPlayer(_SEPlayer);
        timer.limitTimeNow = limitTimeSet;
        timer.GetSEPlayer(_SEPlayer);

        if (soundType == 0)
            creditSystem.SetCreditSound(0);
        if (soundType == 1)
        {
            creditSystem.SetCreditSound(6);
            timer.SetAlertSound(7);
        }
        if (soundType == 2)
        {
            creditSystem.SetCreditSound(10);
            timer.SetAlertSound(11);
        }
        _BGMPlayer.SetAudioPitch(audioPitch);
        _SEPlayer.SetAudioPitch(audioPitch);

        getPoint.GetManager(2);
        ropeManager.ArmUnitUp();

        for (int i = 0; i < 12; i++)
            isExecuted[i] = false;

        // ControlGroupの制御
        if (operationType == 0)
        {
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever Hole").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").gameObject.SetActive(false);
            this.transform.Find("Floor").Find("Type2B").gameObject.SetActive(true);
        }
        else if (operationType == 1)
        {
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 1").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 2").gameObject.SetActive(false);
            this.transform.Find("Floor").Find("Type2L").gameObject.SetActive(true);
        }

        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        craneStatus = -1;
        armController.ArmOpen();
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
            _BGMPlayer.StopBGM(1 + 2 * soundType);
            _BGMPlayer.PlayBGM(2 * soundType);
            //コイン投入有効化;
        }
        else
        {
            if (operationType == 0)
            {
                if (craneStatus == 1)
                {
                    //コイン投入有効化;
                    _BGMPlayer.StopBGM(2 * soundType);
                    _BGMPlayer.PlayBGM(1 + 2 * soundType);
                    InputKeyCheck(craneStatus);     //右移動ボタン有効化;
                }

                if (craneStatus == 2)
                { //右移動中
                    InputKeyCheck(craneStatus);
                    if (craneBox.CheckPos(7))
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
                    InputKeyCheck(craneStatus);         //奥移動ボタン有効化;
                                                        //右移動効果音ループ再生停止;
                }

                if (craneStatus == 4)
                { //奥移動中
                    InputKeyCheck(craneStatus);
                    if (craneBox.CheckPos(8))
                    {
                        buttonPushed = false;
                        craneStatus = 6;
                    }
                    //クレーン奥移動;
                    //奥移動効果音ループ再生;
                }
            }

            if (operationType == 1)
            {
                if (craneStatus == 1)
                {
                    _BGMPlayer.StopBGM(2 * soundType);
                    _BGMPlayer.PlayBGM(1 + 2 * soundType);
                    //レバー操作有効化;
                    //降下ボタン有効化;
                }
                if (craneStatus == 3)
                {
                    if (!isExecuted[craneStatus] && !timerFlag)
                    {
                        isExecuted[craneStatus] = true;
                        timerFlag = true;
                        timer.StartTimer();
                        creditSystem.segUpdateFlag = false;
                    }
                    InputKeyCheck(5);
                    if (isExecuted[craneStatus] && timer.limitTimeNow <= 0) craneStatus = 6;
                }
            }

            if (craneStatus == 6)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                            _SEPlayer.PlaySE(1, 2147483647);
                            break;
                        case 1:
                            _SEPlayer.PlaySE(8, 2147483647);
                            break;
                        case 2:
                            _SEPlayer.PlaySE(12, 2147483647);
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
                    if (craneStatus == 6) ropeManager.ArmUnitDown(); //awaitによる時差実行を防止
                }
                if (craneStatus == 6 && isExecuted[6]) InputKeyCheck(craneStatus); //awaitによる時差実行を防止
                if (ropeManager.DownFinished() && craneStatus == 6) craneStatus = 7;
                //アーム下降音再生
                //アーム下降;
            }

            if (craneStatus == 7)
            {
                if (craneStatus == 7) //awaitによる時差実行を防止
                {
                    switch (soundType)
                    {
                        case 0:
                            _SEPlayer.StopSE(1); //アーム下降音再生停止;
                            _SEPlayer.PlaySE(2, 1); //アーム掴む音再生;
                            break;
                        case 1:
                            _SEPlayer.StopSE(8);
                            break;
                        case 2:
                            _SEPlayer.StopSE(12);
                            break;
                    }
                    if (probability) armPower = catchArmpowersuccess;
                    else armPower = catchArmpower;
                    armController.MotorPower(armPower);
                    armController.ArmClose();
                    await Task.Delay(1000);
                    if (craneStatus == 7) craneStatus = 8; //awaitによる時差実行を防止
                }
                //アーム掴む;
            }

            if (craneStatus == 8)
            {
                switch (soundType)
                {
                    case 1:
                        _SEPlayer.PlaySE(9, 2147483647);
                        break;
                    case 2:
                        _SEPlayer.PlaySE(13, 2147483647);
                        break;
                }

                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    ropeManager.ArmUnitUp();
                }
                if (probability && armPower > upArmpowersuccess)
                {
                    armPower -= 0.5f;
                    armController.MotorPower(armPower);
                }
                else if (!probability && armPower > upArmpower)
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
                if (probability) armPower = upArmpowersuccess;
                else armPower = upArmpower;
                armController.MotorPower(armPower);
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                            _SEPlayer.StopSE(2);
                            _SEPlayer.PlaySE(3, 1); //アーム上昇停止音再生;
                            break;
                        case 1:
                            _SEPlayer.StopSE(9);
                            break;
                        case 2:
                            _SEPlayer.StopSE(13);
                            break;
                    }
                    if (craneStatus == 9) craneStatus = 10;
                }
                //アーム上昇停止;
            }

            if (craneStatus == 10)
            {
                if (probability && armPower > backArmpowersuccess)
                {
                    armPower -= 0.5f;
                    armController.MotorPower(armPower);
                }
                else if (!probability && armPower > backArmpower)
                {
                    armPower -= 0.5f;
                    armController.MotorPower(armPower);
                }
                if (craneBox.CheckPos(1)) craneStatus = 11;
                //アーム獲得口ポジション移動音再生;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.ArmOpen();
                    if (soundType == 0) _SEPlayer.PlaySE(4, 1);
                    await Task.Delay(1000);
                    craneStatus = 12;
                }
                //アーム開く音再生;
                //アーム開く;
                //1秒待機;
            }

            if (craneStatus == 12)
            {
                //1秒待機;
                if (!isExecuted[craneStatus])
                {
                    timerFlag = false;
                    for (int i = 0; i < 12; i++)
                        isExecuted[i] = false;
                }

                if (creditSystem.creditDisplayed > 0)
                    craneStatus = 1;
                else
                    craneStatus = 0;
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
            if (craneStatus == -1 || craneStatus == 10)
            {
                craneBox.Left();
                craneBox.Forward();
            }
            if (operationType == 0)
            {
                if (craneStatus == 2) craneBox.Right();
                if (craneStatus == 4) craneBox.Back();
            }
            else
                if (craneStatus == 1 || craneStatus == 3)
                InputLeverCheck();
        }
    }

    public void GetPrize()
    {
        int getSoundNum = -1;

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                getSoundNum = 5;
                break;
        }

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
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            int credit = creditSystem.PlayStart();
                            if (credit < 100) credit3d.text = credit.ToString("D2");
                            else credit3d.text = "99.";
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
                        craneStatus = 6;
                        buttonPushed = false;
                    }
                    break;
                case 5: // レバー操作時に使用
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && craneStatus == 3)
                        craneStatus = 6;
                    break;
                case 6:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)))
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                    break;
            }
        }
    }

    public void InputLeverCheck() // キーボード，UI共通のレバー処理
    {
        if (host.playable)
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
            || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) // 初動時にタイマーを起動
                if (craneStatus == 1)
                {
                    craneStatus = 3;
                    creditSystem.ResetPayment();
                    int credit = creditSystem.PlayStart();
                    if (credit < 100) credit3d.text = credit.ToString("D2");
                    else credit3d.text = "99.";
                    isExecuted[12] = false;
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
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
                        int credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString("D2");
                        else credit3d.text = "99.";
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
                case 3:
                    if (craneStatus == 6)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                    else if (craneStatus == 3)
                    {
                        buttonPushed = true;
                        craneStatus = 6;
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
                        craneStatus = 6;
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
            if (credit < 100) credit3d.text = credit.ToString("D2");
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
