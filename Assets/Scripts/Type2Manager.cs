using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type2Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float catchArmpower = 60f; //掴むときのアームパワー(%，未確率時)
    float upArmpower = 1f; //上昇時のアームパワー(%，未確率時)
    float backArmpower = 1f; //獲得口移動時のアームパワー(%，未確率時)
    float catchArmpowersuccess = 100f; //同確率時
    float upArmpowersuccess = 100f; //同確率時
    float backArmpowersuccess = 100f; //同確率時
    int operationType = 1; //0:ボタン式，1:レバー式
    int limitTimeSet = 20; //レバー式の場合，残り時間を設定
    int limitTimeCount = 0; //実際のカウントダウン
    int soundType = 0; //DECACRE:0, DECACRE Alpha:1, TRIPLE CATCHER MEGA DASH:2
    bool timerFlag = false; //タイマーの起動はaプレイにつき1度のみ実行
    float audioPitch = 1f; //サウンドのピッチ
    private bool[] instanceFlag = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonFlag = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool probability; //確率判定用
    [SerializeField] bool playable = true; //playableがtrueのとき操作可能
    float armPower; //現在のアームパワー

    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    BGMPlayer _BGMPlayer;
    SEPlayer _SEPlayer;
    Type2ArmController _ArmController;
    Transform temp;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;
    Lever lever;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;
    public Text limitTimedisplayed;

    //-------------------------------------------------

    async void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        _GetPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;

        // ロープとアームコントローラに関する処理
        _RopeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        _ArmController = temp.Find("ArmUnit").GetComponent<Type2ArmController>();

        // CraneBoxに関する処理
        _CraneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        _CraneBox.GetManager(2);

        // ロープにマネージャー情報をセット
        _RopeManager.SetManagerToPoint(2);
        creditSystem.GetSEPlayer(_SEPlayer);
        creditSystem.playable = playable;

        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(10);
        _BGMPlayer.SetAudioPitch(audioPitch);
        _SEPlayer.SetAudioPitch(audioPitch);

        _GetPoint.GetManager(2);
        _RopeManager.ArmUnitUp();

        for (int i = 0; i < 12; i++)
            instanceFlag[i] = false;

        // ControlGroupの制御
        if (operationType == 0)
        {
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever Hole").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").gameObject.SetActive(false);
        }
        else if (operationType == 1)
        {
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 1").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 2").gameObject.SetActive(false);
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);
        }

        _CraneBox.leftMoveFlag = true;
        _CraneBox.forwardMoveFlag = true;
        await Task.Delay(1000);
        _ArmController.ArmOpen();
        creditSystem.insertFlag = true;
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) creditSystem.GetPayment(100);
        //craneStatusdisplayed.text = craneStatus.ToString();
        //limitTimedisplayed.text = limitTimeCount.ToString();
        if (craneStatus == -1)
        {
            _BGMPlayer.StopBGM(2 * soundType);
            //クレーン位置初期化動作; DECACRE・CARINOタイプは不要
            //コイン投入無効化;
        }

        if (craneStatus == 0)
        {
            _BGMPlayer.StopBGM(1 + 2 * soundType);
            _BGMPlayer.PlayBGM(2 * soundType);
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
        }

        if (operationType == 0)
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                instanceFlag[12] = false;
                _BGMPlayer.StopBGM(2 * soundType);
                _BGMPlayer.PlayBGM(1 + 2 * soundType);
                InputKeyCheck(craneStatus);     //右移動ボタン有効化;
            }

            if (craneStatus == 2)
            { //右移動中
                InputKeyCheck(craneStatus);
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
                //クレーン奥移動;
                //奥移動効果音ループ再生;
            }

            if (craneStatus == 5)
            {
                craneStatus = 6;
            }
        }

        if (operationType == 1)
        {
            if (craneStatus == 1)
            {
                _BGMPlayer.StopBGM(2 * soundType);
                _BGMPlayer.PlayBGM(1 + 2 * soundType);
                instanceFlag[12] = false;
                if (!instanceFlag[craneStatus])
                {
                    instanceFlag[craneStatus] = true;
                    limitTimeCount = limitTimeSet;
                }

                //レバー操作有効化;
                //降下ボタン有効化;
                InputLeverCheck();
            }
            if (craneStatus == 3)
            {
                if (!instanceFlag[craneStatus] && !timerFlag)
                {
                    instanceFlag[craneStatus] = true;
                    timerFlag = true;
                    StartTimer();
                }
                InputLeverCheck();
                InputKeyCheck(5);
            }
        }

        if (craneStatus == 6)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _CraneBox.rightMoveFlag = false;
                _CraneBox.leftMoveFlag = false;
                _CraneBox.backMoveFlag = false;
                _CraneBox.forwardMoveFlag = false;
                //await Task.Delay(1000);
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
                CancelTimer();
                if (craneStatus == 6) _RopeManager.ArmUnitDown(); //awaitによる時差実行を防止
            }
            if (craneStatus == 6) InputKeyCheck(craneStatus); //awaitによる時差実行を防止
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
                _ArmController.MotorPower(armPower);
                _ArmController.ArmClose();
            }
            await Task.Delay(1000);
            if (craneStatus == 7) craneStatus = 8; //awaitによる時差実行を防止
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

            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _RopeManager.ArmUnitUp();
            }
            if (probability && armPower > upArmpowersuccess)
            {
                armPower -= 0.5f;
                _ArmController.MotorPower(armPower);
            }
            else if (!probability && armPower > upArmpower)
            {
                armPower -= 0.5f;
                _ArmController.MotorPower(armPower);
            }
            //アーム上昇音再生;
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            if (probability) armPower = upArmpowersuccess;
            else armPower = upArmpower;
            _ArmController.MotorPower(armPower);
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
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
            }
            craneStatus = 10;
            //アーム上昇停止;
        }

        if (craneStatus == 10)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _CraneBox.leftMoveFlag = true;
                _CraneBox.forwardMoveFlag = true;
            }
            if (probability && armPower > backArmpowersuccess)
            {
                armPower -= 0.5f;
                _ArmController.MotorPower(armPower);
            }
            else if (!probability && armPower > backArmpower)
            {
                armPower -= 0.5f;
                _ArmController.MotorPower(armPower);
            }
            if (_CraneBox.CheckPos(1)) craneStatus = 11;
            //アーム獲得口ポジション移動音再生;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmOpen();
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
            if (!instanceFlag[craneStatus])
            {
                timerFlag = false;
                for (int i = 0; i < 12; i++)
                    instanceFlag[i] = false;
            }

            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            else
                craneStatus = 0;
        }

        if (!creditSystem.segUpdateFlag) // Timer表示用
        {
            if (limitTimeCount >= 0)
                limitTimedisplayed.text = limitTimeCount.ToString();
            else
                limitTimedisplayed.text = "0";
        }
    }


    async void StartTimer()
    {
        creditSystem.segUpdateFlag = false;
        while (limitTimeCount >= 0)
        {
            if (limitTimeCount == 0)
            {
                craneStatus = 6;
                await Task.Delay(1000);
                //creditSystem.segUpdateFlag = true;
                break;
            }
            if (limitTimeCount <= 10)
            {
                switch (soundType)
                {
                    case 2:
                        _SEPlayer.PlaySE(11, 1);
                        break;
                    default:
                        _SEPlayer.PlaySE(7, 1);
                        break;
                }
            }
            await Task.Delay(1000);
            limitTimeCount--;
        }
    }

    void CancelTimer()
    {
        limitTimeCount = -1;
        creditSystem.segUpdateFlag = true;
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
        switch (num)
        {
            case 1:
                if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonFlag)
                {
                    buttonFlag = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
                    craneStatus = 2;
                    _CraneBox.rightMoveFlag = true;
                }
                break;
            //投入を無効化
            case 2:
                if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonFlag)
                {
                    craneStatus = 3;
                    _CraneBox.rightMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 3:
                if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonFlag)
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _CraneBox.backMoveFlag = true;
                }
                break;
            case 4:
                if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonFlag)
                {
                    craneStatus = 5;
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 5: // レバー操作時に使用
                if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && craneStatus == 3)
                    craneStatus = 6;
                break;
            case 6:
                if (operationType == 0)
                {
                    if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)))
                    {
                        _RopeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                }
                else if (operationType == 1) // レバー操作時
                {
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)))
                    {
                        _RopeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                }
                break;
        }
    }

    public void InputLeverCheck() // キーボード，UI共通のレバー処理
    {
        if (Input.GetKey(KeyCode.RightArrow) || lever.rightFlag)
            _CraneBox.rightMoveFlag = true;
        else if (Input.GetKeyUp(KeyCode.RightArrow) || !lever.rightFlag)
            _CraneBox.rightMoveFlag = false;
        if (Input.GetKey(KeyCode.LeftArrow) || lever.leftFlag)
            _CraneBox.leftMoveFlag = true;
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || !lever.leftFlag)
            _CraneBox.leftMoveFlag = false;
        if (Input.GetKey(KeyCode.UpArrow) || lever.backFlag)
            _CraneBox.backMoveFlag = true;
        else if (Input.GetKeyUp(KeyCode.UpArrow) || !lever.backFlag)
            _CraneBox.backMoveFlag = false;
        if (Input.GetKey(KeyCode.DownArrow) || lever.forwardFlag)
            _CraneBox.forwardMoveFlag = true;
        else if (Input.GetKeyUp(KeyCode.DownArrow) || !lever.forwardFlag)
            _CraneBox.forwardMoveFlag = false;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)
        || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) // 初動時にタイマーを起動
            if (craneStatus == 1)
            {
                craneStatus = 3;
                creditSystem.ResetNowPayment();
                creditSystem.AddCreditPlayed();
                probability = creditSystem.ProbabilityCheck();
                Debug.Log("Probability:" + probability);
            }
    }

    public void ButtonDown(int num)
    {
        switch (num)
        {
            case 1:
                if (craneStatus == 1 && !buttonFlag)
                {
                    buttonFlag = true;
                    craneStatus = 2;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
                }
                if (craneStatus == 2 && buttonFlag)
                    _CraneBox.rightMoveFlag = true;
                break;
            case 2:
                if ((craneStatus == 3 && !buttonFlag) || (craneStatus == 4 && buttonFlag))
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _CraneBox.backMoveFlag = true;
                }
                break;
            case 3:
                if (craneStatus == 6)
                {
                    buttonFlag = true;
                    _RopeManager.ArmUnitDownForceStop();
                    craneStatus = 7;
                }
                else if (craneStatus == 3)
                {
                    buttonFlag = true;
                    craneStatus = 6;
                }
                break;
        }
    }

    public void ButtonUp(int num)
    {
        switch (num)
        {
            case 1:
                if (/*craneStatus == 1 ||*/ (craneStatus == 2 && buttonFlag))
                {
                    craneStatus = 3;
                    _CraneBox.rightMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 2:
                if (/*craneStatus == 3 ||*/ (craneStatus == 4 && buttonFlag))
                {
                    craneStatus = 5;
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                break;
        }
    }

    public void Testadder()
    {
        Debug.Log("Clicked.");
        craneStatus++;
    }

    public void TestSubber()
    {
        Debug.Log("Clicked.");
        craneStatus--;
    }
}
