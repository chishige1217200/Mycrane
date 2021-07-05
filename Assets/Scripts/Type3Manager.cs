using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type3Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
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
    public bool probability; //確率判定用
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    public float armPower; //現在のアームパワー
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    BGMPlayer _BGMPlayer;
    SEPlayer _SEPlayer;
    Type3ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;
    MachineHost host;
    GameObject canvas;
    [SerializeField] TextMesh credit3d;

    async void Start()
    {
        Transform temp;
        // 様々なコンポーネントの取得
        host = this.transform.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
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
        //craneBox.GetManager(3);

        // ロープにマネージャー情報をセット
        ropeManager.SetManagerToPoint(3);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(3);
        support.GetRopeManager(ropeManager);
        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(13);
        if (soundType == 3) creditSystem.SetCreditSound(-1);
        _BGMPlayer.SetAudioPitch(audioPitch);
        _SEPlayer.SetAudioPitch(audioPitch);
        armController.GetManager(3);

        getPoint.GetManager(3);

        await Task.Delay(300);
        ropeManager.ArmUnitUp();
        if (soundType == 2) armController.ArmOpen();
        else armController.ArmClose();
        craneBox.leftMoveFlag = true;
        craneBox.forwardMoveFlag = true;

        for (int i = 0; i < 12; i++)
            isExecuted[i] = false;

        await Task.Delay(4000);

        craneStatus = 0;
    }

    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();

        if (creditSystem.creditDisplayed < 10) credit3d.text = creditSystem.creditDisplayed.ToString();
        else credit3d.text = "9.";

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;

            switch (soundType)
            {
                case 0:
                    if (!_SEPlayer._AudioSource[5].isPlaying) _BGMPlayer.PlayBGM(0);
                    break;
                case 1:
                    if (!_SEPlayer._AudioSource[12].isPlaying) _BGMPlayer.PlayBGM(1);
                    break;
                case 2:
                    if (!_SEPlayer._AudioSource[16].isPlaying && !_SEPlayer._AudioSource[17].isPlaying)
                        _BGMPlayer.PlayBGM(2);
                    break;
                case 3:
                    _BGMPlayer.StopBGM(4);
                    _BGMPlayer.PlayBGM(3);
                    break;
            }
        }
        else
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                _BGMPlayer.StopBGM(soundType);
                InputKeyCheck(craneStatus);     //右移動ボタン有効化;
                switch (soundType)
                {
                    case 1:
                        if (!_SEPlayer._AudioSource[6].isPlaying)
                            _SEPlayer.PlaySE(7, 2147483647);
                        break;
                    case 3:
                        _BGMPlayer.PlayBGM(4);
                        break;
                }

            }

            if (craneStatus == 2)
            { //右移動中

                InputKeyCheck(craneStatus);
                //コイン投入無効化;
                switch (soundType)
                {
                    case 0:
                        _SEPlayer.PlaySE(1, 2147483647);
                        break;
                    case 1:
                        _SEPlayer.StopSE(7);
                        _SEPlayer.PlaySE(8, 2147483647);
                        break;
                    case 2:
                        _SEPlayer.PlaySE(14, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(18, 2147483647);
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
                InputKeyCheck(craneStatus);         //奥移動ボタン有効化;
                switch (soundType)
                {
                    case 2:
                        _SEPlayer.StopSE(14);
                        break;
                    case 3:
                        _SEPlayer.StopSE(18);
                        break;
                }
                //右移動効果音ループ再生停止;
            }

            if (craneStatus == 4)
            { //奥移動中
                InputKeyCheck(craneStatus);
                //クレーン奥移動;
                switch (soundType)
                {
                    case 0:
                        _SEPlayer.StopSE(1);
                        _SEPlayer.PlaySE(2, 2147483647);
                        break;
                    case 1:
                        _SEPlayer.StopSE(8);
                        _SEPlayer.PlaySE(9, 2147483647);
                        break;
                    case 2:
                        _SEPlayer.PlaySE(14, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(19, 2147483647);
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
                    if (soundType != 2) armController.ArmOpen();
                    switch (soundType)
                    {
                        case 0:
                            _SEPlayer.StopSE(2);
                            _SEPlayer.PlaySE(3, 2147483647);
                            break;
                        case 1:
                            _SEPlayer.StopSE(9);
                            _SEPlayer.PlaySE(10, 2147483647);
                            break;
                        case 2:
                            _SEPlayer.StopSE(14);
                            break;
                        case 3:
                            _SEPlayer.StopSE(19);
                            _SEPlayer.PlaySE(20, 1);
                            break;
                    }
                    if (soundType != 2)
                    {
                        if (soundType == 3) await Task.Delay(2000);
                        else await Task.Delay(1000);
                    }
                    ropeManager.ArmUnitDown();
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
                            _SEPlayer.PlaySE(15, 2147483647);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(21, 2147483647);
                            break;
                    }
                    if (downTime > 0 && downTime <= 4600)
                    {
                        await Task.Delay(downTime);
                        if (craneStatus == 6)
                        {
                            ropeManager.ArmUnitDownForceStop();
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
                            _SEPlayer.StopSE(21);
                            break;
                    }
                    if (probability) armPower = catchArmpowersuccess;
                    else armPower = catchArmpower;
                    armController.MotorPower(armPower);
                    armController.ArmClose();
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
                            _SEPlayer.StopSE(3);
                            _SEPlayer.PlaySE(4, 2147483647);
                            break;
                        case 1:
                            _SEPlayer.StopSE(10);
                            _SEPlayer.PlaySE(11, 2147483647);
                            break;
                        case 2:
                            _SEPlayer.StopSE(15);
                            _SEPlayer.PlaySE(14, 2147483647);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(22, 2147483647);
                            break;
                    }
                    ropeManager.ArmUnitUp();
                    await Task.Delay(1500);
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 8 && support.prizeFlag) armController.Release(); // 上昇中に離す振り分け
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
                if (!armController.releaseFlag)
                {
                    if (probability) armPower = upArmpowersuccess;
                    else armPower = upArmpower;
                    armController.MotorPower(armPower);
                }
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 3:
                            _SEPlayer.StopSE(22);
                            break;
                    }
                    if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 9 && support.prizeFlag) armController.Release(); // 上昇後に離す振り分け
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
                    craneBox.leftMoveFlag = true;
                    craneBox.forwardMoveFlag = true;
                    switch (soundType)
                    {
                        case 0:
                            _SEPlayer.StopSE(4);
                            _SEPlayer.PlaySE(1, 2147483647);
                            break;
                        case 2:
                            _SEPlayer.StopSE(14);
                            _SEPlayer.PlaySE(14, 2147483647);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(23, 2147483647);
                            break;
                    }
                }
                if (!armController.releaseFlag)
                {
                    if (support.prizeFlag)
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
                    armController.ArmOpen();
                    switch (soundType)
                    {
                        case 3:
                            _SEPlayer.StopSE(23);
                            _SEPlayer.PlaySE(24, 1);
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
                    if (soundType != 2) armController.ArmClose();
                    switch (soundType)
                    {
                        case 2:
                            _SEPlayer.StopSE(14);
                            if (!_SEPlayer._AudioSource[16].isPlaying)
                                _SEPlayer.PlaySE(17, 1);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(25, 1);
                            break;
                    }
                    for (int i = 0; i < 12; i++)
                        isExecuted[i] = false;
                    await Task.Delay(1000);
                    if (soundType == 3) await Task.Delay(1000);
                    switch (soundType)
                    {
                        case 0:
                            _SEPlayer.StopSE(1);
                            break;
                        case 1:
                            _SEPlayer.StopSE(11);
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

    public void GetPrize()
    {
        int getSoundNum = -1;

        switch (soundType)
        {
            case 0:
                getSoundNum = 5;
                _SEPlayer.StopSE(1);
                break;
            case 1:
                getSoundNum = 12;
                _SEPlayer.StopSE(11);
                break;
            case 2:
                getSoundNum = 16;
                _SEPlayer.StopSE(14);
                _SEPlayer.StopSE(17);
                break;
            case 3:
                getSoundNum = 26;
                _SEPlayer.StopSE(25);
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
                            creditSystem.PlayStart();
                            creditSystem.AddCreditPlayed();
                            isExecuted[12] = false;
                            probability = creditSystem.ProbabilityCheck();
                            Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                        craneBox.rightMoveFlag = true;
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed)
                    {
                        craneStatus = 3;
                        craneBox.rightMoveFlag = false;
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        craneBox.backMoveFlag = true;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed)
                    {
                        craneStatus = 5;
                        craneBox.backMoveFlag = false;
                        buttonPushed = false;
                    }
                    break;
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
                        creditSystem.PlayStart();
                        creditSystem.AddCreditPlayed();
                        isExecuted[12] = false;
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
                    if (craneStatus == 2 && buttonPushed)
                        craneBox.rightMoveFlag = true;
                    break;
                case 2:
                    if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                        craneBox.backMoveFlag = true;
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
                        craneBox.rightMoveFlag = false;
                        buttonPushed = false;
                    }
                    break;
                case 2:
                    if (craneStatus == 4 && buttonPushed)
                    {
                        craneStatus = 5;
                        craneBox.backMoveFlag = false;
                        buttonPushed = false;
                    }
                    break;
            }
        }
    }

    public void InsertCoin()
    {
        if (host.playable) creditSystem.Pay(100);
    }
}
