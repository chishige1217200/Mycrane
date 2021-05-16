using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type5Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    float leftCatchArmpower = 10f; //左アームパワー
    float rightCatchArmpower = 10f; //右アームパワー
    float armApertures = 80f; //開口率
    public int soundType = 0; //SEの切り替え 0,1: CATCHER 8,9 2: CATCHER 7 Selecterで指定すること
    private bool[] isExecuted = new bool[15]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool playable = true; //playableがtrueのとき操作可能
    [SerializeField] bool button3 = true; //button3の使用可否
    private Vector2 startPoint; // 開始位置座標定義
    private Vector2 homePoint; // 獲得口座標定義（prizezoneTypeが9のとき使用）
    private Vector2 vec2offset = new Vector2(0.05f, 0.1f); // <=0.5, <=0.6 座標設定用Temp
    public int prizezoneType = 9; // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標（1P時）2Pは左右反転
    Vector2 craneHost; //クレーンゲームの中心位置定義
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    SEPlayer _SEPlayer;
    Type5ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    async void Start()
    {
        Transform temp;
        // 様々なコンポーネントの取得
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.parent;
        craneHost = new Vector2(temp.position.x, temp.position.z);
        temp = this.transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];

        // ロープとアームコントローラに関する処理
        ropeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type5ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        /*nail[0] = temp.Find("ArmUnit").Find("Arm1").GetComponent<ArmNail>();
        nail[1] = temp.Find("ArmUnit").Find("Arm2").GetComponent<ArmNail>();*/

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        craneBox.GetManager(5);

        // ロープにマネージャー情報をセット
        ropeManager.SetManagerToPoint(5);
        creditSystem.GetSEPlayer(_SEPlayer);
        creditSystem.playable = playable;
        getPoint.GetManager(5);
        ropeManager.ArmUnitUp();
        await Task.Delay(500);
        if (soundType == 0 || soundType == 1) creditSystem.SetCreditSound(0);
        else creditSystem.SetCreditSound(8);
        creditSystem.GetSEPlayer(_SEPlayer);
        /*support.GetManager(5);
        support.GetRopeManager(ropeManager);
        support.pushTime = 300; // 押し込みパワーの調整
        /*for (int i = 0; i < 2; i++)
        {
            nail[i].GetManager(5);
            nail[i].GetRopeManager(ropeManager);
        }*/

        for (int i = 0; i < 15; i++)
            isExecuted[i] = false;

        if (!button3) this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);

        // イニシャル移動とinsertFlagを後に実行
        await Task.Delay(3000);
        armController.ArmLimit(armApertures);
        if (!player2) craneBox.leftMoveFlag = true;
        else craneBox.rightMoveFlag = true;
        craneBox.forwardMoveFlag = true;

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
            if (!player2 && craneBox.CheckPos(1))
            {
                craneBox.goPoint = startPoint;
                craneBox.goPositionFlag = true;
                break;
            }
            if (player2 && craneBox.CheckPos(3))
            {
                craneBox.goPoint = startPoint;
                craneBox.goPositionFlag = true;
                break;
            }
            await Task.Delay(1000);
        }

        await Task.Delay(500);

        while (true)
        {
            if (craneBox.CheckPos(9))
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
            if (!isExecuted[0])
            {
                isExecuted[0] = true;
                craneBox.goPoint = new Vector2(-0.3f, 0f);
                craneBox.goPositionFlag = true;
            }
            await Task.Delay(500);
            if (isExecuted[0])
                if (craneBox.CheckPos(9))
                {
                    armController.ArmOpen();
                    Debug.Log("OK");
                }
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
        }

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
                    _SEPlayer.PlaySE(1, 2147483647);
                    break;
                case 2:
                    _SEPlayer.PlaySE(9, 2147483647);
                    break;
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
                    _SEPlayer.StopSE(1);
                    break;
                case 2:
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
                    _SEPlayer.PlaySE(2, 2147483647);
                    break;
                case 2:
                    _SEPlayer.PlaySE(10, 2147483647);
                    break;
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
                        _SEPlayer.StopSE(2);
                        _SEPlayer.PlaySE(3, 1);
                        break;
                    case 2:
                        _SEPlayer.StopSE(10);
                        _SEPlayer.PlaySE(11, 1);
                        break;
                }
                await Task.Delay(1000);
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
                        _SEPlayer.PlaySE(4, 2147483647);
                        break;
                    case 2:
                        _SEPlayer.PlaySE(12, 2147483647);
                        break;
                }
                if (craneStatus == 6) ropeManager.ArmUnitDown(); //awaitによる時差実行を防止
            }
            InputKeyCheck(craneStatus);
            //アーム下降音再生
            //アーム下降;
        }

        if (craneStatus == 7)
        {
            switch (soundType)
            {
                case 0:
                case 1:
                    _SEPlayer.StopSE(4);
                    break;
                case 2:
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
                        _SEPlayer.PlaySE(5, 1);
                        break;
                    case 2:
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
                        _SEPlayer.PlaySE(4, 2147483647);
                        break;
                    case 2:
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
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            //アーム上昇停止音再生;
            //アーム上昇停止;
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                if (prizezoneType == 9) craneBox.goPoint = homePoint;
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
                        _SEPlayer.StopSE(4);
                        _SEPlayer.PlaySE(6, 2147483647);
                        break;
                    case 2:
                        _SEPlayer.StopSE(14);
                        _SEPlayer.PlaySE(9, 2147483647);
                        break;
                }
                switch (prizezoneType) // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標（1P時）2Pは左右反転
                {
                    case 1:
                        if (!player2) craneBox.leftMoveFlag = true;
                        else craneBox.rightMoveFlag = true;
                        craneBox.forwardMoveFlag = true;
                        break;
                    case 2:
                        if (!player2) craneBox.leftMoveFlag = true;
                        else craneBox.rightMoveFlag = true;
                        craneBox.backMoveFlag = true;
                        break;
                    case 3:
                        if (!player2) craneBox.rightMoveFlag = true;
                        else craneBox.leftMoveFlag = true;
                        craneBox.forwardMoveFlag = true;
                        break;
                    case 4:
                        if (!player2) craneBox.rightMoveFlag = true;
                        else craneBox.leftMoveFlag = true;
                        craneBox.backMoveFlag = true;
                        break;
                    case 5:
                        if (!player2) craneBox.leftMoveFlag = true;
                        else craneBox.rightMoveFlag = true;
                        break;
                    case 6:
                        craneBox.forwardMoveFlag = true;
                        break;
                    case 7:
                        if (!player2) craneBox.rightMoveFlag = true;
                        else craneBox.leftMoveFlag = true;
                        break;
                    case 8:
                        craneBox.backMoveFlag = true;
                        break;
                    case 9:
                        craneBox.goPositionFlag = true;
                        break;
                }
            }
            await Task.Delay(500);
            if (isExecuted[craneStatus])
                if (craneBox.CheckPos(9) && craneStatus == 10) craneStatus = 11;
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
                        _SEPlayer.StopSE(6);
                        _SEPlayer.PlaySE(3, 1);
                        break;
                    case 2:
                        _SEPlayer.StopSE(9);
                        _SEPlayer.PlaySE(11, 1);
                        break;
                }
                armController.ArmLimit(100f); // アーム開口度を100に
                armController.ArmOpen();
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
                switch (soundType)
                {
                    case 0:
                    case 1:
                        _SEPlayer.PlaySE(5, 1);
                        break;
                    case 2:
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
                        _SEPlayer.PlaySE(6, 2147483647);
                        break;
                }
                if (!player2) craneBox.leftMoveFlag = true;
                else craneBox.rightMoveFlag = true;
                craneBox.forwardMoveFlag = true;
            }
            if ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2))
            {
                switch (soundType)
                {
                    case 0:
                    case 1:
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
                        _SEPlayer.PlaySE(6, 2147483647);
                        break;
                }
                for (int i = 0; i < 14; i++)
                    isExecuted[i] = false;
                armController.ArmLimit(armApertures); //アーム開口度リセット
                craneBox.goPoint = startPoint;
                craneBox.goPositionFlag = true;
            }
            await Task.Delay(500);
            if (isExecuted[craneStatus] && craneStatus == 14)
            {
                if (craneBox.CheckPos(9))
                {
                    switch (soundType)
                    {
                        case 0:
                        case 1:
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
                if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed && !player2)
                {
                    buttonPushed = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                        isExecuted[14] = false;
                    }
                    craneStatus = 2;
                    craneBox.rightMoveFlag = true;
                }
                if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                {
                    buttonPushed = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                        isExecuted[14] = false;
                    }
                    craneStatus = 2;
                    craneBox.leftMoveFlag = true;
                }
                break;
            //投入を無効化
            case 2:
                if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                {
                    craneStatus = 3;
                    craneBox.rightMoveFlag = false;
                    buttonPushed = false;
                }
                if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                {
                    craneStatus = 3;
                    craneBox.leftMoveFlag = false;
                    buttonPushed = false;
                }
                break;
            case 3:
                if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                {
                    buttonPushed = true;
                    craneStatus = 4;
                    craneBox.backMoveFlag = true;
                }
                if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                {
                    buttonPushed = true;
                    craneStatus = 4;
                    craneBox.backMoveFlag = true;
                }
                break;
            case 4:
                if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                {
                    craneStatus = 5;
                    craneBox.backMoveFlag = false;
                    buttonPushed = false;
                }
                if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                {
                    craneStatus = 5;
                    craneBox.backMoveFlag = false;
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

    public void ButtonDown(int num)
    {
        switch (num)
        {
            case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                    isExecuted[14] = false;
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
            case 3:
                if (craneStatus == 6)
                {
                    //buttonPushed = true;
                    ropeManager.ArmUnitDownForceStop();
                    craneStatus = 7;
                }
                break;
            case 4: // player2 case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                    isExecuted[14] = false;
                }
                if (craneStatus == 2 && buttonPushed)
                    craneBox.leftMoveFlag = true;
                break;
        }
    }

    public void ButtonUp(int num)
    {
        switch (num)
        {
            case 1:
                if (/*craneStatus == 1 ||*/ (craneStatus == 2 && buttonPushed))
                {
                    craneStatus = 3;
                    craneBox.rightMoveFlag = false;
                    buttonPushed = false;
                }
                break;
            case 2:
                if (/*craneStatus == 3 ||*/ (craneStatus == 4 && buttonPushed))
                {
                    craneStatus = 5;
                    craneBox.backMoveFlag = false;
                    buttonPushed = false;
                }
                break;
            case 4: // player2 case 1:
                if (/*craneStatus == 1 ||*/ (craneStatus == 2 && buttonPushed))
                {
                    craneStatus = 3;
                    craneBox.leftMoveFlag = false;
                    buttonPushed = false;
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
