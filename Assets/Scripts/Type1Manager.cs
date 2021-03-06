using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type1Manager : MonoBehaviour
{
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float catchArmpower = 100f;
    float armApertures = 100f; //開口率
    int catchTime = 2000; //キャッチに要する時間
    private bool[] instanceFlag = new bool[15];
    public bool buttonFlag = false; // trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false;
    [SerializeField] bool playable = true;
    [SerializeField] bool button3 = true;
    private Vector2 startPoint; // 開始位置
    private Vector2 homePoint; // 獲得口
    private Vector2 vec2offset = new Vector2(0.05f, 0.1f); // <=0.5, <=0.6 座標設定用Temp
    private int prizezoneType = 0; // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標（1P時）2Pは左右反転
    private BGMPlayer _BGMPlayer;
    private SEPlayer _SEPlayer;
    Type1ArmController _ArmController;
    Transform temp;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;
    ArmControllerSupport support;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    async void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        _GetPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;

        // ロープとアームコントローラに関する処理
        _RopeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        _ArmController = temp.Find("ArmUnit").GetComponent<Type1ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();

        // CraneBoxに関する処理
        _CraneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        _CraneBox.GetManager(1);

        // ロープにマネージャー情報をセット
        _RopeManager.SetManagerToPoint(1);
        creditSystem.GetSEPlayer(_SEPlayer);
        creditSystem.playable = playable;
        _GetPoint.GetManager(1);
        _RopeManager.ArmUnitUp();
        creditSystem.SetCreditSound(0);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(1);
        support.GetRopeManager(_RopeManager);

        for (int i = 0; i < 15; i++)
            instanceFlag[i] = false;

        if (!button3) this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);

        // イニシャル移動とinsertFlagを後に実行
        await Task.Delay(3000);
        _ArmController.ArmLimit(armApertures);
        if (!player2) _CraneBox.leftMoveFlag = true;
        else _CraneBox.rightMoveFlag = true;
        _CraneBox.forwardMoveFlag = true;
        if (!player2) _SEPlayer.PlaySE(6, 1);

        if (!player2)
        {
            startPoint = new Vector2(-0.65f + vec2offset.x, -0.3f + vec2offset.y);
            homePoint = new Vector2(-0.65f + vec2offset.x, -0.3f + vec2offset.y);
        }
        else
        {
            startPoint = new Vector2(0.65f - vec2offset.x, -0.3f + vec2offset.y);
            homePoint = new Vector2(0.65f - vec2offset.x, -0.3f + vec2offset.y);
        }

        while (true)
        {
            if (!player2 && _CraneBox.CheckPos(1))
            {
                _CraneBox.goPoint = startPoint;
                _CraneBox.goPositionFlag = true;
                break;
            }
            if (player2 && _CraneBox.CheckPos(3))
            {
                _CraneBox.goPoint = startPoint;
                _CraneBox.goPositionFlag = true;
                break;
            }
            await Task.Delay(1000);
        }

        await Task.Delay(500);

        while (true)
        {
            if (_CraneBox.CheckPos(9))
            {
                creditSystem.insertFlag = true;
                craneStatus = 0;
                break;
            }
            await Task.Delay(1000);
        }
    }

    async void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) && !player2) creditSystem.GetPayment(100);
        if ((Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus)) && player2) creditSystem.GetPayment(100);
        //craneStatusdisplayed.text = craneStatus.ToString();

        if (craneStatus == -2) // Test
        {
            if (!instanceFlag[0])
            {
                instanceFlag[0] = true;
                _CraneBox.goPoint = new Vector2(-0.3f, 0f);
                _CraneBox.goPositionFlag = true;
            }
            await Task.Delay(500);
            if (instanceFlag[0])
                if (_CraneBox.CheckPos(9))
                {
                    _ArmController.ArmOpen();
                    Debug.Log("OK");
                }
        }

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
            instanceFlag[14] = false;
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
            if (craneStatus == 7) _SEPlayer.PlaySE(3, 2); //アーム掴む音再生;
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                if (craneStatus == 7)
                {
                    _ArmController.MotorPower(catchArmpower);
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
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            _SEPlayer.StopSE(4);
            //アーム上昇停止音再生;
            //アーム上昇停止;
            craneStatus = 10;
        }

        if (craneStatus == 10)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                switch (prizezoneType)
                {
                    case 0:
                        if (!player2) _CraneBox.leftMoveFlag = true;
                        else _CraneBox.rightMoveFlag = true;
                        _CraneBox.forwardMoveFlag = true;
                        break;
                    case 1:

                    case 2:

                    case 3:

                    case 4:

                    case 5:

                    case 6:

                    case 7:

                    case 8:

                    case 9:

                        break;
                }
            }
            if (_CraneBox.CheckPos(9)) craneStatus = 11;
            //アーム獲得口ポジション移動音再生;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmLimit(100f); // アーム開口度を100に
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
                _ArmController.ArmClose();
            //アーム閉じる音再生;
            //アーム閉じる;
            //1秒待機;
            await Task.Delay(1000);
            if (craneStatus == 12) craneStatus = 13;
        }

        if (craneStatus == 13)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                if (!player2) _CraneBox.leftMoveFlag = true;
                else _CraneBox.rightMoveFlag = true;
                _CraneBox.forwardMoveFlag = true;
            }
            if (_CraneBox.CheckPos(1) && !player2) craneStatus = 14;
            if (_CraneBox.CheckPos(3) && player2) craneStatus = 14;
            //アーム初期位置帰還
        }
        if (craneStatus == 14)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                for (int i = 0; i < 14; i++)
                    instanceFlag[i] = false;
                _ArmController.ArmLimit(armApertures); //アーム開口度リセット
                if (!player2)
                {
                    _CraneBox.goPoint = startPoint;
                    _CraneBox.goPositionFlag = true;
                }
                if (player2)
                {
                    _CraneBox.goPoint = startPoint;
                    _CraneBox.goPositionFlag = true;
                }
            }
            if (instanceFlag[craneStatus])
            {
                if (_CraneBox.CheckPos(9))
                {
                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                }
            }
            //アームスタート位置へ
        }
    }

    public void GetPrize()
    {
        int getSoundNum = -1;
        getSoundNum = 5;

        /*switch (creditSystem.probabilityMode)
        {
            case 2:
            case 3:
                creditSystem.ResetCreditProbability();
                break;
            case 4:
            case 5:
                creditSystem.ResetCostProbability();
                break;
        }*/

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
                if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonFlag && !player2)
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
                if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonFlag && player2)
                {
                    buttonFlag = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                    }
                    craneStatus = 2;
                    _CraneBox.leftMoveFlag = true;
                }
                break;
            //投入を無効化
            case 2:
                if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonFlag && !player2)
                {
                    craneStatus = 3;
                    _CraneBox.rightMoveFlag = false;
                    buttonFlag = false;
                }
                if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonFlag && player2)
                {
                    craneStatus = 3;
                    _CraneBox.leftMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 3:
                if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonFlag && !player2)
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _CraneBox.backMoveFlag = true;
                }
                if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonFlag && player2)
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _CraneBox.backMoveFlag = true;
                }
                break;
            case 4:
                if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonFlag && !player2)
                {
                    craneStatus = 5;
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonFlag && player2)
                {
                    craneStatus = 5;
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 6:
                if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) && !player2 && button3)
                {
                    _RopeManager.ArmUnitDownForceStop();
                    craneStatus = 7;
                }
                if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) && player2 && button3)
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
                break;
            case 4: // player2 case 1:
                if (craneStatus == 1 && !buttonFlag)
                {
                    buttonFlag = true;
                    craneStatus = 2;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                }
                if (craneStatus == 2 && buttonFlag)
                    _CraneBox.leftMoveFlag = true;
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
            case 4: // player2 case 1:
                if (/*craneStatus == 1 ||*/ (craneStatus == 2 && buttonFlag))
                {
                    craneStatus = 3;
                    _CraneBox.leftMoveFlag = false;
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
