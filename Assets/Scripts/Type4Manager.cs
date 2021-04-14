using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type4Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float leftCatchArmpower = 100f; //左アームパワー
    float rightCatchArmpower = 100f; //右アームパワー
    float armApertures = 100f; //開口率
    int operationType = 1; //0:ボタン式，1:レバー式
    int soundType = 0; //BGMの切り替え．0・1
    int catchTime = 2000; //キャッチに要する時間(m秒)
    private bool[] instanceFlag = new bool[17]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonFlag = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool leverFlag = false; //trueならレバーがアクティブ
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool playable = true; //playableがtrueのとき操作可能
    [SerializeField] bool button3 = true; //button3の使用可否
    private Vector2 startPoint; // 開始位置座標定義
    private Vector2 homePoint; // 獲得口座標定義（prizezoneTypeが9のとき使用）
    private Vector2 vec2offset = new Vector2(0.05f, 0.1f); // <=0.5, <=0.6 座標設定用Temp
    public int prizezoneType = 9; // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標（1P時）2Pは左右反転
    bool prizeGetFlag = false;
    Vector2 craneHost; //クレーンゲームの中心位置定義
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    SEPlayer _SEPlayer;
    //Type4ArmController _ArmController;
    Transform temp;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];
    Lever lever;
    VideoPlay videoPlay;

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
        //_ArmController = temp.Find("ArmUnit").GetComponent<Type1ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        //nail[0] = temp.Find("ArmUnit").Find("Arm1").GetComponent<ArmNail>();
        //nail[1] = temp.Find("ArmUnit").Find("Arm2").GetComponent<ArmNail>();
        videoPlay = this.transform.Find("VideoPlay").GetComponent<VideoPlay>();

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
        support.pushTime = 300; // 押し込みパワーの調整
        /*for (int i = 0; i < 2; i++)
        {
            nail[i].GetManager(4);
            nail[i].GetRopeManager(_RopeManager);
        }*/

        for (int i = 0; i < 15; i++)
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

        if (!button3) this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);

        // イニシャル移動とinsertFlagを後に実行
        await Task.Delay(3000);
        //_ArmController.ArmLimit(armApertures);
        videoPlay.randomMode = true;
        if (!player2) _CraneBox.leftMoveFlag = true;
        else _CraneBox.rightMoveFlag = true;
        _CraneBox.forwardMoveFlag = true;

        if (!player2)
        {
            startPoint = new Vector2(-0.65f + vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
            homePoint = new Vector2(-0.65f + vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
        }
        else
        {
            startPoint = new Vector2(0.65f - vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
            homePoint = new Vector2(0.65f - vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
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
        }

        if (operationType == 0)
        {
            if (craneStatus == 1)
            {
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

            if (craneStatus == 5)
            {
                craneStatus = 6;
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
                //InputKeyCheck(5);
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
                if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonFlag)
                {
                    buttonFlag = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                        instanceFlag[12] = false;
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

    public async void InputLeverCheck() // キーボード，UI共通のレバー処理
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
                    instanceFlag[12] = false;
                }
        }
        if (player2)
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
                    instanceFlag[12] = false;
                }
        }
    }
}
