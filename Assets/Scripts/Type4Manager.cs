using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type4Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float leftCatchArmpower = 100f; //左アームパワー
    float rightCatchArmpower = 100f; //右アームパワー
    float armApertures = 100f; //開口率
    int operationType = 0; //0:ボタン式，1:レバー式
    int catchLong = 2000; //キャッチに要する時間(m秒)
    int catchTiming = 2000; //キャッチが始まるまでの時間(m秒)
    int backTime = 1000; //戻り動作が始まるまでの時間(m秒)
    private bool[] instanceFlag = new bool[16]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonFlag = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool leverFlag = false; //trueならレバーがアクティブ
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool playable = true; //playableがtrueのとき操作可能
    [SerializeField] bool rotation = true; //回転機能の使用可否
    [SerializeField] bool downStop = true; //下降停止機能の使用可否
    bool prizeGetFlag = false;
    Vector2 craneHost; //クレーンゲームの中心位置定義
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    SEPlayer _SEPlayer;
    Type1ArmController _ArmController;
    Transform temp;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];
    Lever lever;
    VideoPlay videoPlay;
    Type4ArmunitRoter roter;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    async void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        _GetPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.parent;
        craneHost = new Vector2(temp.position.x, temp.position.z);
        temp = this.transform.Find("CraneUnit").transform;

        // ロープとアームコントローラに関する処理
        _RopeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        _ArmController = temp.Find("ArmUnit").GetComponent<Type1ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        //nail[0] = temp.Find("ArmUnit").Find("Arm1").GetComponent<ArmNail>();
        //nail[1] = temp.Find("ArmUnit").Find("Arm2").GetComponent<ArmNail>();
        videoPlay = this.transform.Find("VideoPlay").GetComponent<VideoPlay>();
        roter = temp.Find("ArmUnit").Find("Main").GetComponent<Type4ArmunitRoter>();

        // CraneBoxに関する処理
        _CraneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        _CraneBox.GetManager(4);

        // ロープにマネージャー情報をセット
        _RopeManager.SetManagerToPoint(4);
        creditSystem.GetSEPlayer(_SEPlayer);
        creditSystem.playable = playable;
        _GetPoint.GetManager(4);
        _RopeManager.ArmUnitUp();
        creditSystem.SetCreditSound(0);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(4);
        support.GetRopeManager(_RopeManager);
        roter.GetSEPlayer(_SEPlayer);
        support.pushTime = 300; // 押し込みパワーの調整
        /*for (int i = 0; i < 2; i++)
        {
            nail[i].GetManager(4);
            nail[i].GetRopeManager(_RopeManager);
        }*/

        for (int i = 0; i < 16; i++)
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

        // イニシャル移動とinsertFlagを後に実行
        await Task.Delay(3000);
        _ArmController.ArmLimit(armApertures);
        _ArmController.ArmFinalClose();
        videoPlay.randomMode = true;
        if (!player2) _CraneBox.leftMoveFlag = true;
        else _CraneBox.rightMoveFlag = true;
        _CraneBox.forwardMoveFlag = true;

        await Task.Delay(3000);

        creditSystem.insertFlag = true;
        craneStatus = 0;

    }

    // Update is called once per frame
    async void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) && !player2) creditSystem.GetPayment(100);
        if ((Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus)) && player2) creditSystem.GetPayment(100);
        //craneStatusdisplayed.text = craneStatus.ToString();

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
            {
                videoPlay.randomMode = false;
                craneStatus = 1;
            }
            else videoPlay.randomMode = true;
        }

        if (operationType == 0)
        {
            if (craneStatus == 1)
            {
                if (!instanceFlag[craneStatus])
                {
                    instanceFlag[craneStatus] = true;
                    if (Random.Range(0, 2) == 0 && !player2) videoPlay.PlayVideo(4);
                    else videoPlay.PlayVideo(0);
                }
                InputKeyCheck(craneStatus);         //右移動ボタン有効化;
            }

            if (craneStatus == 2)
            { //右移動中
                InputKeyCheck(craneStatus);
                //クレーン右移動;
            }

            if (craneStatus == 3)
            {
                InputKeyCheck(craneStatus);         //奥移動ボタン有効化;
            }

            if (craneStatus == 4)
            { //奥移動中
                InputKeyCheck(craneStatus);
                //クレーン奥移動;
            }
        }

        if (operationType == 1)
        {
            if (craneStatus == 1)
            {
                if (!instanceFlag[craneStatus])
                {
                    instanceFlag[craneStatus] = true;
                    videoPlay.PlayVideo(0);
                }
                InputLeverCheck();
            }
            if (craneStatus == 3)
            {
                InputLeverCheck();
                InputKeyCheck(4);
            }
        }

        if (craneStatus == 5)
        {
            InputKeyCheck(craneStatus);
        }
        if (craneStatus == 6)
        {
            await Task.Delay(10);
            if (craneStatus == 6) InputKeyCheck(craneStatus);
        }
        if (craneStatus == 7)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _ArmController.ArmOpen();
                await Task.Delay(1000);
                if (craneStatus == 7) craneStatus = 8;
            }
            //アーム開く音再生;
            //アーム開く;
        }
        if (craneStatus == 8)
        {   //アーム下降中
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                _SEPlayer.PlaySE(5, 1);
                if (craneStatus == 8) _RopeManager.ArmUnitDown(); //awaitによる時差実行を防止
            }
            InputKeyCheck(craneStatus);
        }
        if (craneStatus == 9)
        {   //アーム掴む
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                if (catchTiming > 0) await Task.Delay(catchTiming);
                if (leftCatchArmpower >= 30 || rightCatchArmpower >= 30) //閉じるときのアームパワーは大きい方を採用．最低値は30f
                {
                    if (leftCatchArmpower >= rightCatchArmpower) _ArmController.ArmClose(leftCatchArmpower);
                    else _ArmController.ArmClose(rightCatchArmpower);
                }
                else _ArmController.ArmClose(30f);
                if (catchLong > 0) await Task.Delay(catchLong);
                if (craneStatus == 9) craneStatus = 10;
            }
        }
        if (craneStatus == 10)
        {   //アーム上昇中
            {
                instanceFlag[craneStatus] = true;
                _RopeManager.ArmUnitUp();
                await Task.Delay(1000);
                _ArmController.MotorPower(leftCatchArmpower, 0);
                _ArmController.MotorPower(rightCatchArmpower, 1);
            }
        }
        if (craneStatus == 11)
        {   //アーム上昇停止
            if (!instanceFlag[craneStatus])
            {
                if (backTime > 0) await Task.Delay(backTime);
                if (!player2) _CraneBox.leftMoveFlag = true;
                else _CraneBox.rightMoveFlag = true;
                _CraneBox.forwardMoveFlag = true;
                craneStatus = 12;
            }
        }
        if (craneStatus == 12)
        {
            if (_CraneBox.CheckPos(1) && craneStatus == 12 & !player2) craneStatus = 13;
            if (_CraneBox.CheckPos(3) && craneStatus == 12 & player2) craneStatus = 13;
        }
        if (craneStatus == 13)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                videoPlay.PlayVideo(5);
                _ArmController.ArmLimit(100f); // アーム開口度を100に
                _ArmController.ArmOpen();
                await Task.Delay(2000);
                if (craneStatus == 13) craneStatus = 14;
                //アーム開く音再生;
                //アーム開く;
                //1秒待機;
            }
        }
        if (craneStatus == 14)
        {
            if (!instanceFlag[craneStatus])
            {
                _ArmController.ArmFinalClose();
                await Task.Delay(1000);
                if (craneStatus == 14) craneStatus = 15;
            }
            //アーム閉じる音再生;
            //アーム閉じる;
            //1秒待機;
        }
        if (craneStatus == 15)
        {
            if (!instanceFlag[craneStatus])
            {
                instanceFlag[craneStatus] = true;
                for (int i = 0; i < 14; i++)
                    instanceFlag[i] = false;
                _ArmController.ArmLimit(armApertures); //アーム開口度リセット
                if (prizeGetFlag) _SEPlayer.PlaySE(6, 1);
                else _SEPlayer.PlaySE(7, 1);
                roter.RotateToHome();
                await Task.Delay(5000);

                prizeGetFlag = false;
                if (creditSystem.creditDisplayed > 0)
                    craneStatus = 1;
                else
                    craneStatus = 0;
            }
        }
    }

    public void GetPrize()
    {
        prizeGetFlag = true;
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
                        videoPlay.PlayVideo(1);
                        instanceFlag[15] = false;
                    }
                    craneStatus = 2;
                    _SEPlayer.PlaySE(1, 1);
                    if (!player2) _CraneBox.rightMoveFlag = true;
                }
                if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonFlag && player2)
                {
                    buttonFlag = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                        videoPlay.PlayVideo(1);
                        instanceFlag[15] = false;
                    }
                    craneStatus = 2;
                    _SEPlayer.PlaySE(1, 1);
                    _CraneBox.leftMoveFlag = true;
                }
                break;
            //投入を無効化
            case 2:
                if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonFlag && !player2)
                {
                    craneStatus = 3;
                    _CraneBox.rightMoveFlag = false;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    buttonFlag = false;
                }
                if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonFlag && player2)
                {
                    craneStatus = 3;
                    _CraneBox.leftMoveFlag = false;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    buttonFlag = false;
                }
                break;
            case 3:
                if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonFlag && !player2)
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _SEPlayer.PlaySE(1, 1);
                    _CraneBox.backMoveFlag = true;
                }
                if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonFlag && player2)
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _SEPlayer.PlaySE(1, 1);
                    _CraneBox.backMoveFlag = true;
                }
                break;
            case 4:
                if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonFlag && !player2)
                {
                    craneStatus = 5;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonFlag && player2)
                {
                    craneStatus = 5;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 5:
                if (((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && player2) ||
                    (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && !player2)) && !buttonFlag)
                {
                    //buttonFlag = true;
                    craneStatus = 6;
                    roter.RotateStart();
                    videoPlay.PlayVideo(2);
                }
                /*else if (((Input.GetKeyUp(KeyCode.Keypad3) || Input.GetKeyUp(KeyCode.Alpha3) && player2) ||
                        (Input.GetKeyUp(KeyCode.Keypad9) || Input.GetKeyUp(KeyCode.Alpha9) && !player2)) && buttonFlag)
                    buttonFlag = false;*/
                break;
            case 6:
                if (((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && player2) ||
                    (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && !player2)) && !buttonFlag)
                {
                    //buttonFlag = true;
                    craneStatus = 7;
                    roter.RotateStop();
                    videoPlay.PlayVideo(3);
                }
                /*else if (((Input.GetKeyUp(KeyCode.Keypad3) || Input.GetKeyUp(KeyCode.Alpha3) && player2) ||
                        (Input.GetKeyUp(KeyCode.Keypad9) || Input.GetKeyUp(KeyCode.Alpha9) && !player2)) && buttonFlag)
                    buttonFlag = false; //バグりそう*/
                break;
            case 8:
                if (((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) && !player2) ||
                    ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) && player2) && downStop)
                {
                    _RopeManager.ArmUnitDownForceStop();
                    craneStatus = 9;
                }
                break;
        }
    }
    public void InputLeverCheck() // キーボード，UI共通のレバー処理
    {
        if (!player2)
        {
            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverFlag)
            {
                leverFlag = true;
                videoPlay.PlayVideo(1);
                _SEPlayer.StopSE(2);
                _SEPlayer.PlaySE(1, 1);
            }
            if ((!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)
            && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverFlag)
            {
                leverFlag = false;
                videoPlay.PlayVideo(0);
                _SEPlayer.StopSE(1);
                _SEPlayer.PlaySE(2, 1);
            }

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
                    instanceFlag[15] = false;
                }
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)
            || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) && !leverFlag)
            {
                leverFlag = true;
                videoPlay.PlayVideo(1);
                _SEPlayer.StopSE(2);
                _SEPlayer.PlaySE(1, 1);
            }
            if ((!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)
            && !lever.rightFlag && !lever.leftFlag && !lever.backFlag && !lever.forwardFlag) && leverFlag)
            {
                leverFlag = false;
                videoPlay.PlayVideo(0);
                _SEPlayer.StopSE(1);
                _SEPlayer.PlaySE(2, 1);
            }

            if (Input.GetKey(KeyCode.D) || lever.rightFlag)
                _CraneBox.rightMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.D) || !lever.rightFlag)
                _CraneBox.rightMoveFlag = false;
            if (Input.GetKey(KeyCode.A) || lever.leftFlag)
                _CraneBox.leftMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.A) || !lever.leftFlag)
                _CraneBox.leftMoveFlag = false;
            if (Input.GetKey(KeyCode.W) || lever.backFlag)
                _CraneBox.backMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.W) || !lever.backFlag)
                _CraneBox.backMoveFlag = false;
            if (Input.GetKey(KeyCode.S) || lever.forwardFlag)
                _CraneBox.forwardMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.S) || !lever.forwardFlag)
                _CraneBox.forwardMoveFlag = false;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag) // 初動時にタイマーを起動
                if (craneStatus == 1)
                {
                    craneStatus = 3;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                    instanceFlag[15] = false;
                }
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
                    videoPlay.PlayVideo(1);
                    _SEPlayer.PlaySE(1, 1);
                    instanceFlag[15] = false;
                }
                if (craneStatus == 2 && buttonFlag)
                {
                    _SEPlayer.PlaySE(1, 1);
                    _CraneBox.rightMoveFlag = true;
                }
                break;
            case 2:
                if ((craneStatus == 3 && !buttonFlag) || (craneStatus == 4 && buttonFlag))
                {
                    buttonFlag = true;
                    craneStatus = 4;
                    _SEPlayer.PlaySE(1, 1);
                    _CraneBox.backMoveFlag = true;
                }
                break;
            case 3:
                if ((craneStatus == 5 && operationType == 0) || (craneStatus == 3 && operationType == 1 && !leverFlag))
                {
                    craneStatus = 6;
                    roter.RotateStart();
                    videoPlay.PlayVideo(2);
                }
                else if (craneStatus == 6)
                {
                    craneStatus = 7;
                    roter.RotateStop();
                    videoPlay.PlayVideo(3);
                }
                else if (craneStatus == 8)
                {
                    //buttonFlag = true;
                    _RopeManager.ArmUnitDownForceStop();
                    craneStatus = 9;
                }
                break;
            case 4: // player2 case 1:
                if (craneStatus == 1 && !buttonFlag)
                {
                    buttonFlag = true;
                    craneStatus = 2;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                    videoPlay.PlayVideo(1);
                    _SEPlayer.PlaySE(1, 1);
                    instanceFlag[15] = false;
                }
                if (craneStatus == 2 && buttonFlag)
                {
                    _SEPlayer.PlaySE(1, 1);
                    _CraneBox.leftMoveFlag = true;
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
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    _CraneBox.rightMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 2:
                if (/*craneStatus == 3 ||*/ (craneStatus == 4 && buttonFlag))
                {
                    craneStatus = 5;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    _CraneBox.backMoveFlag = false;
                    buttonFlag = false;
                }
                break;
            case 4: // player2 case 1:
                if (/*craneStatus == 1 ||*/ (craneStatus == 2 && buttonFlag))
                {
                    craneStatus = 3;
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 1);
                    _CraneBox.leftMoveFlag = false;
                    buttonFlag = false;
                }
                break;
        }
    }
}
