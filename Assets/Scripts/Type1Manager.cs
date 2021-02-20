using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type1Manager : MonoBehaviour
{
    public CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float catchArmpowersuccess = 100f; //同確率時
    float upArmpowersuccess = 100f; //同確率時
    float backArmpowersuccess = 100f; //同確率時
    float armApertures = 100f; //開口率
    int catchTime = 3000; //キャッチに要する時間
    private bool[] instanceFlag = new bool[13];
    public bool buttonFlag = false; // trueならボタンをクリックしているかキーボードを押下している
    float armPower; // 現在のアームパワー
    private BGMPlayer _BGMPlayer;
    private SEPlayer _SEPlayer;
    Type1ArmController _ArmController;
    Transform temp;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        _GetPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;

        // ロープとアームコントローラに関する処理
        _RopeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        _ArmController = temp.Find("ArmUnit").GetComponent<Type1ArmController>();

        // CraneBoxに関する処理
        _CraneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        _CraneBox.GetManager(1);

        // ロープにマネージャー情報をセット
        _RopeManager.SetManagerToPoint(1);
        creditSystem.GetSEPlayer(_SEPlayer);

        _GetPoint.GetManager(1);
        _RopeManager.ArmUnitUp();
        creditSystem.SetCreditSound(0);
        creditSystem.GetSEPlayer(_SEPlayer);

        for (int i = 0; i < 12; i++)
            instanceFlag[i] = false;

        // イニシャル移動とinsertFlagを後に実行
        _CraneBox.leftMoveFlag = true;
        _CraneBox.rightMoveFlag = false;
        _ArmController.ArmLimit(armApertures);
        creditSystem.insertFlag = true;
        craneStatus = 0;
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) creditSystem.GetPayment(100);
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            //クレーン位置初期化動作;
            //コイン投入無効化;
            _BGMPlayer.StopBGM(0);
        }

        if (craneStatus == 0)
        {
            _BGMPlayer.PlayBGM(0);
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
        }

        if (craneStatus == 1)
        {
            instanceFlag[12] = false;
            //コイン投入有効化;
            //右移動ボタン有効化;
            InputKeyCheck(craneStatus);
        }

        if (craneStatus == 2)
        { //右移動中
            //コイン投入無効化;
            InputKeyCheck(craneStatus);
            //クレーン右移動;
            _SEPlayer.PlaySE(1, 2); //右移動効果音ループ再生;
        }

        if (craneStatus == 3)
        {
            InputKeyCheck(craneStatus);
            _SEPlayer.StopSE(1); //右移動効果音ループ再生停止;
            //奥移動ボタン有効化;
        }

        if (craneStatus == 4)
        { //奥移動中
            //クレーン奥移動;
            InputKeyCheck(craneStatus);
            _SEPlayer.PlaySE(1, 2); //奥移動効果音ループ再生;
        }

        if (craneStatus == 5)
        {
            _SEPlayer.StopSE(1); //奥移動効果音ループ再生停止;
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmOpen();
            }
            //アーム開く音再生;
            //アーム開く;
            await Task.Delay(1000);
            if (craneStatus == 5) craneStatus = 6;
        }

        if (craneStatus == 6)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                if (craneStatus == 6) _RopeManager.ArmUnitDown(); //awaitによる時差実行を防止
            }
            InputKeyCheck(craneStatus);
            _SEPlayer.PlaySE(2, 2); //アーム下降音再生
            //アーム下降;
        }

        if (craneStatus == 7)
        {
            _SEPlayer.StopSE(2); //アーム下降音再生停止;
            await Task.Delay(1000);
            _SEPlayer.PlaySE(3, 2); //アーム掴む音再生;
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                if (craneStatus == 7)
                {
                    armPower = catchArmpowersuccess;
                    _ArmController.MotorPower(armPower);
                    _ArmController.ArmClose();
                }
            }
            await Task.Delay(catchTime);
            if (craneStatus == 7) craneStatus = 8; //awaitによる時差実行を防止
                                                   //アーム掴む;
        }

        if (craneStatus == 8)
        {
            _SEPlayer.StopSE(3);
            _SEPlayer.PlaySE(4, 2); //アーム上昇音再生;
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _RopeManager.ArmUnitUp();
            }

            if (armPower > upArmpowersuccess)
            {
                armPower -= 0.5f;
                _ArmController.MotorPower(armPower);
            }
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            _SEPlayer.StopSE(4);
            armPower = upArmpowersuccess;
            _ArmController.MotorPower(armPower);
            //アーム上昇停止音再生;
            //アーム上昇停止;
            craneStatus = 10;
        }

        if (craneStatus == 10)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _CraneBox.leftMoveFlag = true;
                _CraneBox.forwardMoveFlag = true;
            }
            if (armPower > backArmpowersuccess)
            {
                armPower -= 0.5f;
                _ArmController.MotorPower(armPower);
            }
            if (_CraneBox.CheckHomePos(1)) craneStatus = 11;
            //アーム獲得口ポジション移動音再生;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmOpen();
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
                _ArmController.ArmFinalClose();
                for (int i = 0; i < 12; i++)
                    instanceFlag[i] = false;
            }
            //アーム閉じる音再生;
            //アーム閉じる;
            //1秒待機;
            await Task.Delay(1000);
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            else
                craneStatus = 0;
        }
    }

    public void GetPrize()
    {
        int getSoundNum = -1;
        getSoundNum = 5;

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
            case 6:
                if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyUp(KeyCode.Alpha3)))
                {
                    _RopeManager.ArmUnitDownForceStop();
                    craneStatus = 7;
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