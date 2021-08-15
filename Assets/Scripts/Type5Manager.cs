using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type5Manager : MonoBehaviour
{
    public int craneStatus = -3; //-3:初期化動作，0:待機状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] float leftCatchArmpower = 10f; //左アームパワー
    [SerializeField] float rightCatchArmpower = 10f; //右アームパワー
    [SerializeField] float armApertures = 80f; //開口率
    [SerializeField] float[] boxRestrictions = new float[2];
    [SerializeField] float downRestriction = 100f;
    public int soundType = 0; //SEの切り替え 0,1: CATCHER 8,9 2: CATCHER 7 Selecterで指定すること
    bool[] isExecuted = new bool[15]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool button3 = true; //button3の使用可否
    public Vector2 startPoint; // 開始位置座標定義
    public Vector2 homePoint; // 獲得口座標定義（prizezoneTypeが9のとき使用）
    public int prizezoneType = 9; // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    public SEPlayer _SEPlayer;
    Type5ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];
    MachineHost host;
    GameObject canvas;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];

    async void Start()
    {
        Transform temp;
        Transform xLimit = this.transform.Find("Floor").Find("XLimit");
        Transform zLimit = this.transform.Find("Floor").Find("ZLimit");
        // 様々なコンポーネントの取得
        host = this.transform.root.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        //_SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();

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
        armController = temp.Find("ArmUnit").GetComponent<Type5ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        nail[0] = temp.Find("ArmUnit").Find("Arm1").Find("Nail1").GetComponent<ArmNail>();
        nail[1] = temp.Find("ArmUnit").Find("Arm2").Find("Nail2").GetComponent<ArmNail>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        // ロープにマネージャー情報をセット
        creditSystem.GetSEPlayer(_SEPlayer);
        getPoint.GetManager(5);
        ropeManager.ArmUnitUp();
        await Task.Delay(500);
        if (soundType == 0 || soundType == 1 || soundType == 2) creditSystem.SetCreditSound(0);
        else creditSystem.SetCreditSound(8);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(5);
        support.GetRopeManager(ropeManager);
        support.pushTime = 300; // 押し込みパワーの調整
        for (int i = 0; i < 2; i++)
        {
            nail[i].GetManager(5);
            nail[i].GetRopeManager(ropeManager);
        }

        for (int i = 0; i < 15; i++)
            isExecuted[i] = false;

        if (!button3)
        {
            this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);
            this.transform.Find("Floor").Find("Button3").gameObject.SetActive(false);
        }

        // イニシャル移動とinsertFlagを後に実行
        while (!ropeManager.UpFinished())
        {
            await Task.Delay(100);
        }
        armController.ArmLimit(armApertures);

        if (!player2)
        {
            startPoint = new Vector2(-0.61f + startPoint.x, -0.31f + startPoint.y);
            homePoint = new Vector2(-0.61f + homePoint.x, -0.31f + homePoint.y);
            if (boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(-0.5f + 0.004525f * boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        }
        else
        {
            startPoint = new Vector2(0.61f - startPoint.x, -0.31f + startPoint.y);
            homePoint = new Vector2(0.61f - homePoint.x, -0.31f + homePoint.y);
            if (boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(0.5f - 0.004525f * boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        }
        if (boxRestrictions[1] < 100) zLimit.localPosition = new Vector3(zLimit.localPosition.x, zLimit.localPosition.y, -0.19f + 0.00605f * boxRestrictions[1]);
        craneBox.goPoint = startPoint;

        craneStatus = -2;
    }

    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2)) && craneStatus == -2)
        {
            craneStatus = -1;
            craneBox.goPositionFlag = true;
        }
        if (craneStatus == -1)
        {
            if (craneBox.CheckPos(9)) craneStatus = 0;
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
        }
        else
        {
            if (craneStatus == 1)
            {
                //コイン投入有効化;
                //右移動ボタン有効化;
                InputKeyCheck(craneStatus);
            }

            if (craneStatus == 2)
            { //右移動中
              //コイン投入無効化;
                InputKeyCheck(craneStatus);
                //クレーン右移動;
                switch (soundType)
                {
                    case 0:
                    case 1:
                    case 2:
                        _SEPlayer.PlaySE(1, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(9, 2147483647);
                        break;
                }
                if (!player2 & craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
                if (player2 & craneBox.CheckPos(5))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
                //右移動効果音ループ再生;
            }

            if (craneStatus == 3)
            {
                InputKeyCheck(craneStatus);
                switch (soundType)
                {
                    case 0:
                    case 1:
                    case 2:
                        _SEPlayer.StopSE(1);
                        break;
                    case 3:
                        _SEPlayer.StopSE(9);
                        break;
                }
                //右移動効果音ループ再生停止;
                //奥移動ボタン有効化;
            }

            if (craneStatus == 4)
            { //奥移動中
                InputKeyCheck(craneStatus);
                //クレーン奥移動;
                switch (soundType)
                {
                    case 0:
                    case 1:
                    case 2:
                        _SEPlayer.PlaySE(2, 2147483647);
                        break;
                    case 3:
                        _SEPlayer.PlaySE(10, 2147483647);
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
                _SEPlayer.StopSE(1); //奥移動効果音ループ再生停止;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    armController.ArmOpen();
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.StopSE(2);
                            _SEPlayer.PlaySE(3, 1);
                            break;
                        case 3:
                            _SEPlayer.StopSE(10);
                            _SEPlayer.PlaySE(11, 1);
                            break;
                    }
                    await Task.Delay(1700);
                    if (craneStatus == 5) craneStatus = 6;
                }
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
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(4, 2147483647);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(12, 2147483647);
                            break;
                    }
                    if (craneStatus == 6) ropeManager.ArmUnitDown(); //awaitによる時差実行を防止
                }
                InputKeyCheck(craneStatus);
                if (ropeManager.DownFinished() && craneStatus == 6) craneStatus = 7;
                //アーム下降音再生
                //アーム下降;
            }

            if (craneStatus == 7)
            {
                switch (soundType)
                {
                    case 0:
                    case 1:
                    case 2:
                        _SEPlayer.StopSE(4);
                        break;
                    case 3:
                        _SEPlayer.StopSE(12);
                        break;
                }
                //アーム下降音再生停止;
                await Task.Delay(1000);
                //アーム掴む音再生;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(5, 1);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(13, 1);
                            break;
                    }
                    if (craneStatus == 7)
                        if (leftCatchArmpower >= 30 || rightCatchArmpower >= 30) //閉じるときのアームパワーは大きい方を採用．最低値は30f
                        {
                            if (leftCatchArmpower >= rightCatchArmpower) armController.ArmClose(leftCatchArmpower);
                            else armController.ArmClose(rightCatchArmpower);
                        }
                        else armController.ArmClose(30f);

                    await Task.Delay(3000);
                    if (craneStatus == 7) craneStatus = 8; //awaitによる時差実行を防止
                }
                //アーム掴む;
            }

            if (craneStatus == 8)
            {
                //アーム上昇音再生;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(4, 2147483647);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(14, 2147483647);
                            break;
                    }
                    ropeManager.ArmUnitUp();
                    await Task.Delay(1000);
                    if (craneStatus < 11)
                    {
                        armController.MotorPower(leftCatchArmpower, 0);
                        armController.MotorPower(rightCatchArmpower, 1);
                    }
                }
                if (ropeManager.UpFinished() && craneStatus == 8) craneStatus = 9;
                //アーム上昇;
            }

            if (craneStatus == 9)
            {
                //アーム上昇停止音再生;
                //アーム上昇停止;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    if (prizezoneType == 9)
                    {
                        craneBox.goPoint = homePoint;
                        craneBox.goPositionFlag = true;
                    }
                }
                if (isExecuted[craneStatus] && craneStatus == 9) craneStatus = 10;
            }

            if (craneStatus == 10)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.StopSE(4);
                            _SEPlayer.PlaySE(6, 2147483647);
                            break;
                        case 3:
                            _SEPlayer.StopSE(14);
                            _SEPlayer.PlaySE(9, 2147483647);
                            break;
                    }
                }
                if (craneBox.CheckPos(prizezoneType)) craneStatus = 11; //ここがバグってる
                //アーム獲得口ポジション移動音再生;
                //アーム獲得口ポジションへ;
            }

            if (craneStatus == 11)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.StopSE(6);
                            _SEPlayer.PlaySE(3, 1);
                            break;
                        case 3:
                            _SEPlayer.StopSE(9);
                            _SEPlayer.PlaySE(11, 1);
                            break;
                    }
                    armController.ArmLimit(100f); // アーム開口度を100に
                    armController.ArmOpen();
                    await Task.Delay(2500);
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
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(5, 1);
                            break;
                        case 3:
                            _SEPlayer.PlaySE(13, 1);
                            break;
                    }
                    armController.ArmClose(100f);
                    await Task.Delay(1000);
                    if (craneStatus == 12) craneStatus = 13;
                }
                //アーム閉じる音再生;
                //アーム閉じる;
                //1秒待機;
            }

            if (craneStatus == 13)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(6, 2147483647);
                            break;
                    }
                }
                if ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2))
                {
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.StopSE(6);
                            break;
                    }
                    await Task.Delay(1000);
                    if (craneStatus == 13) craneStatus = 14;
                }
                //アーム初期位置帰還
            }
            if (craneStatus == 14)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    switch (soundType)
                    {
                        case 0:
                        case 1:
                        case 2:
                            _SEPlayer.PlaySE(6, 2147483647);
                            break;
                    }
                    for (int i = 0; i < 14; i++)
                        isExecuted[i] = false;
                    armController.ArmLimit(armApertures); //アーム開口度リセット

                    craneBox.goPoint = startPoint;
                    craneBox.goPositionFlag = true;
                }
                if (isExecuted[craneStatus])
                {
                    if (craneBox.CheckPos(9))
                    {
                        switch (soundType)
                        {
                            case 0:
                            case 1:
                            case 2:
                                _SEPlayer.StopSE(6);
                                break;
                        }
                        if (creditSystem.creditDisplayed > 0)
                            craneStatus = 1;
                        else
                            craneStatus = 0;
                    }
                }
                //アームスタート位置へ
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus == 0) ;
        else
        {
            if (craneStatus == -2 || craneStatus == 13)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
                craneBox.Forward();
            }
            else if (craneStatus == 10 || craneStatus == 15)
            {
                switch (prizezoneType) // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標
                {
                    case 1:
                        craneBox.Left();
                        craneBox.Forward();
                        break;
                    case 2:
                        craneBox.Left();
                        craneBox.Back();
                        break;
                    case 3:
                        craneBox.Right();
                        craneBox.Forward();
                        break;
                    case 4:
                        craneBox.Right();
                        craneBox.Back();
                        break;
                    case 5:
                        craneBox.Left();
                        break;
                    case 6:
                        craneBox.Forward();
                        break;
                    case 7:
                        craneBox.Right();
                        break;
                    case 8:
                        craneBox.Back();
                        break;
                }
            }
            else if (craneStatus == 2)
            {
                if (!player2) craneBox.Right();
                else craneBox.Left();
            }
            else if (craneStatus == 4) craneBox.Back();
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
                getSoundNum = 7;
                break;
            case 3:
                getSoundNum = 15;
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
            int credit = 0;
            switch (num)
            {
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            credit = creditSystem.PlayStart();
                            if (credit < 100) credit3d.text = credit.ToString();
                            else credit3d.text = "99.";
                            isExecuted[14] = false;
                        }
                        craneStatus = 2;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            credit = creditSystem.PlayStart();
                            if (credit < 100) credit3d.text = credit.ToString();
                            else credit3d.text = "99.";
                            isExecuted[14] = false;
                        }
                        craneStatus = 2;
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    break;
                case 6:
                    if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) && !player2 && button3)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) && player2 && button3)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                    break;
            }
        }
    }

    public void ButtonDown(int num)
    {
        if (host.playable)
        {
            int credit = 0;
            switch (num)
            {
                case 1:
                    if (craneStatus == 1 && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString();
                        else credit3d.text = "99.";
                        isExecuted[14] = false;
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
                    break;
                case 4: // player2 case 1:
                    if (craneStatus == 1 && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString();
                        else credit3d.text = "99.";
                        isExecuted[14] = false;
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
                case 4: // player2 case 1:
                    if (craneStatus == 2 && buttonPushed)
                    {
                        craneStatus = 3;
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
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
