using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type3Manager : MonoBehaviour
{
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float catchArmpower = 100; //掴むときのアームパワー(%，未確率時)
    float upArmpower = 100; //上昇時のアームパワー(%，未確率時)
    float backArmpower = 0; //獲得口移動時のアームパワー(%，未確率時)
    float catchArmpowersuccess = 100; //同確率時
    float upArmpowersuccess = 100; //同確率時
    float backArmpowersuccess = 100; //同確率時
    int soundType = 1; //0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    float audioPitch = 1f; //サウンドのピッチ
    private bool[] instanceFlag = new bool[13];
    public bool buttonFlag = false; // trueならボタンをクリックしているかキーボードを押下している
    public bool probability; // 確率判定用
    float armPower; // 現在のアームパワー
    BGMPlayer _BGMPlayer;
    SEPlayer _SEPlayer;
    Type3ArmController _ArmController;
    Transform temp;
    GameObject craneBox;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {
        // 様々なコンポーネントの取得
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        _GetPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;
        // ロープとアームコントローラに関する処理
        /*_RopePoint = new RopePoint[7];
        _RopePoint[0] = temp.Find("Rope").Find("Sphere (1)").GetComponent<RopePoint>();
        _RopePoint[1] = temp.Find("Rope").Find("Sphere (2)").GetComponent<RopePoint>();
        _RopePoint[2] = temp.Find("Rope").Find("Sphere (3)").GetComponent<RopePoint>();
        _RopePoint[3] = temp.Find("Rope").Find("Sphere (4)").GetComponent<RopePoint>();
        _RopePoint[4] = temp.Find("Rope").Find("Sphere (5)").GetComponent<RopePoint>();
        _RopePoint[5] = temp.Find("Rope").Find("Sphere (6)").GetComponent<RopePoint>();
        _RopePoint[6] = temp.Find("Rope").Find("Sphere (7)").GetComponent<RopePoint>();*/
        _RopeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        _ArmController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").gameObject;
        _CraneBox = craneBox.GetComponent<CraneBox>();
        _CraneBox.GetManager(3);

        // ロープにマネージャー情報をセット
        /*for (int i = 0; i < 7; i++)
            _RopePoint[i].GetManager(3);*/
        _RopeManager.SetManagerToPoint(3);

        creditSystem.GetSEPlayer(_SEPlayer);
        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(13);
        if (soundType == 3) creditSystem.SetCreditSound(-1);
        _BGMPlayer.SetAudioPitch(audioPitch);
        _SEPlayer.SetAudioPitch(audioPitch);

        _GetPoint.GetManager(3);

        //await Task.Delay(300);
        _RopeManager.ArmUnitUp();
        //_ArmController.ArmOpen();
        //await Task.Delay(500);
        _CraneBox.leftMoveFlag = true;
        _CraneBox.forwardMoveFlag = true;

        for (int i = 0; i < 12; i++)
            instanceFlag[i] = false;
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) creditSystem.GetPayment(100);
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            _BGMPlayer.StopBGM(soundType);
            //await Task.Delay(1500);
            if (_CraneBox.CheckHomePos(1))
            {
                craneStatus = 0;
                _ArmController.ArmClose();
            }
            //コイン投入無効化;
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            /*if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;*/
            creditSystem.insertFlag = true;
            switch (soundType)
            {
                case 0:
                    _BGMPlayer.PlayBGM(0);
                    break;
                case 1:
                    _BGMPlayer.PlayBGM(1);
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
            //}
        }

        if (craneStatus == 1)
        {
            //コイン投入有効化;
            instanceFlag[12] = false;
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
            /*if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
            }*/
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
            /*if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;

            }*/
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
            /*if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;

            }*/
            //奥移動効果音ループ再生;
        }

        if (craneStatus == 5)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmOpen();
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
                await Task.Delay(1000);
                _RopeManager.ArmUnitDown();
                craneStatus = 6;
            }
            //奥移動効果音ループ再生停止;
            //アーム開く音再生;
            //アーム開く;
        }

        if (craneStatus == 6)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                switch (soundType)
                {
                    case 2:
                        _SEPlayer.PlaySE(15, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(21, 2147483647);
                        break;
                }
            }
            //アーム下降音再生
            //アーム下降;
        }

        if (craneStatus == 7)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                switch (soundType)
                {
                    case 0:
                        _SEPlayer.StopSE(3);
                        _SEPlayer.PlaySE(4, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.StopSE(21);
                        break;
                }
                if (probability) armPower = catchArmpowersuccess;
                else armPower = catchArmpower;
                _ArmController.MotorPower(armPower);
                _ArmController.ArmClose();
                await Task.Delay(1000);
                craneStatus = 8;
            }

            //アーム下降音再生停止;
            //アーム掴む音再生;
            //アーム掴む;
        }

        if (craneStatus == 8)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                switch (soundType)
                {
                    case 1:
                        _SEPlayer.StopSE(10);
                        _SEPlayer.PlaySE(11, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(22, 2147483647);
                        break;
                }
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
                    case 3:
                        _SEPlayer.StopSE(22);
                        break;
                }
            }
            craneStatus = 10;
            //アーム上昇停止音再生;
            //アーム上昇停止;
        }

        if (craneStatus == 10)
        {
            //アーム獲得口ポジション移動音再生;
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _CraneBox.leftMoveFlag = true;
                _CraneBox.forwardMoveFlag = true;
                switch (soundType)
                {
                    case 0:
                        _SEPlayer.StopSE(4);
                        _SEPlayer.PlaySE(1, 2147483647);
                        break;
                    case 2:
                        _SEPlayer.StopSE(15);
                        _SEPlayer.PlaySE(14, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(23, 2147483647);
                        break;
                }
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
            if (_CraneBox.CheckHomePos(1)) craneStatus = 11;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmOpen();
                switch (soundType)
                {
                    case 3:
                        _SEPlayer.StopSE(23);
                        _SEPlayer.PlaySE(24, 1);
                        break;
                }
                await Task.Delay(2000);
                craneStatus = 12;
            }
            //アーム開く音再生;
            //アーム開く;
            //1秒待機;
        }

        if (craneStatus == 12)
        {

            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                //_ArmController.MotorPower(0f);
                _ArmController.ArmFinalClose();
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
                    instanceFlag[i] = false;
                await Task.Delay(2000);
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